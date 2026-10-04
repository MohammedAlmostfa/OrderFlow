using Microsoft.EntityFrameworkCore;
using OrderFlow.Api.Data;
using OrderFlow.Api.DTOs.Orders;
using OrderFlow.Api.Exceptions;
using OrderFlow.Api.Models;

namespace OrderFlow.Api.Services.Orders;

public class OrderService : IOrderService
{
    private readonly AppDbContext _context;
    private readonly OrderStatusService _orderStatusService;

    public OrderService(
        AppDbContext context,
        OrderStatusService orderStatusService)
    {
        _context = context;
        _orderStatusService = orderStatusService;
    }

public async Task<OrderResponse> CreateAsync(
    int userId,
    CreateOrderRequest request)
    {
        // 1. Verify that the user exists.
        var userExists = await _context.Users
            .AnyAsync(u => u.Id == userId);

    if (!userExists)
        throw new NotFoundException("User not found.");

        // 2. Combine duplicate product IDs.
        var requestedItems = request.Items
            .GroupBy(i => i.ProductId)
            .Select(g => new
            {
                ProductId = g.Key,
                Quantity = g.Sum(i => i.Quantity)
            })
            .ToList();

        var productIds = requestedItems
            .Select(i => i.ProductId)
            .ToList();

        // 3. Load the requested products.
        var products = await _context.Products
            .AsNoTracking()
            .Where(p => productIds.Contains(p.Id))
            .ToListAsync();

        if (products.Count != productIds.Count)
            throw new NotFoundException("One or more products were not found.");

        var productsById = products.ToDictionary(p => p.Id);

        // Start a transaction before reserving stock and creating the order.
        await using var transaction =
            await _context.Database.BeginTransactionAsync();

        var order = new Order
        {
            UserId = userId,
            Status = OrderStatus.Pending,
            TotalAmount = 0
        };

        foreach (var item in requestedItems)
        {
            var product = productsById[item.ProductId];
            var stockUpdated = await _context.Products
                .Where(p => p.Id == product.Id && p.StockQuantity >= item.Quantity)
                .ExecuteUpdateAsync(setters => setters
                    .SetProperty(
                        p => p.StockQuantity,
                        p => p.StockQuantity - item.Quantity));

            if (stockUpdated == 0)
                throw new ConflictException("Insufficient stock.");

            var orderItem = new OrderItem
            {
                ProductId = product.Id,
                Quantity = item.Quantity,

                // Snapshot of the current product price.
                UnitPrice = product.Price
            };

            order.Items.Add(orderItem);

            // Calculate the total on the server.
            order.TotalAmount += product.Price * item.Quantity;
        }

        _context.Orders.Add(order);

        await _context.SaveChangesAsync();

        await transaction.CommitAsync();

    return new OrderResponse
{
        Id = order.Id,
        UserId = order.UserId,
        Status = order.Status.ToString(),
        TotalAmount = order.TotalAmount,
        CreatedAt = order.CreatedAt,
        Items = order.Items.Select(item => new OrderItemResponse
        {
            ProductId = item.ProductId,
            ProductName = productsById[item.ProductId].Name,
            Quantity = item.Quantity,
            UnitPrice = item.UnitPrice,
            Subtotal = item.UnitPrice * item.Quantity
        }).ToList()
};
    }

public async Task<List<OrderResponse>> GetMyOrdersAsync(
    int userId)
{
    return await _context.Orders
        .AsNoTracking()
        .Where(o => o.UserId == userId)
        .OrderByDescending(o => o.CreatedAt)
        .Select(o => new OrderResponse
        {
            Id = o.Id,
            UserId = o.UserId,
            Status = o.Status.ToString(),
            TotalAmount = o.TotalAmount,
            CreatedAt = o.CreatedAt,

            Items = o.Items.Select(item => new OrderItemResponse
            {
                ProductId = item.ProductId,
                ProductName = item.Product.Name,
                Quantity = item.Quantity,
                UnitPrice = item.UnitPrice,
                Subtotal = item.UnitPrice * item.Quantity
            }).ToList()
        })
        .ToListAsync();
}


public async Task<OrderResponse> GetByIdAsync(
    int orderId,
    int userId)
{
    return await _context.Orders
        .AsNoTracking()
        .Where(o => o.Id == orderId && o.UserId == userId)
        .Select(o => new OrderResponse
        {
            Id = o.Id,
            UserId = o.UserId,
            Status = o.Status.ToString(),
            TotalAmount = o.TotalAmount,
            CreatedAt = o.CreatedAt,

            Items = o.Items.Select(item => new OrderItemResponse
            {
                ProductId = item.ProductId,
                ProductName = item.Product.Name,
                Quantity = item.Quantity,
                UnitPrice = item.UnitPrice,
                Subtotal = item.UnitPrice * item.Quantity
            }).ToList()
        })
        .FirstOrDefaultAsync()
        ?? throw new NotFoundException("Order not found.");
}

    public async Task<OrderResponse> UpdateStatusAsync(
    int orderId,
    OrderStatus newStatus)
{
    await using var transaction =
        await _context.Database.BeginTransactionAsync();

    var order = await _context.Orders
        .AsNoTracking()
        .Include(o => o.Items)
        .FirstOrDefaultAsync(o => o.Id == orderId);

    if (order is null)
        throw new NotFoundException("Order not found.");

    var isValid = _orderStatusService.IsValidTransition(
        order.Status,
        newStatus);

    if (!isValid)
        throw new ConflictException("Invalid order status transition.");

    var updatedAt = DateTime.UtcNow;
    var updated = await _context.Orders
        .Where(o => o.Id == orderId && o.Status == order.Status)
        .ExecuteUpdateAsync(setters => setters
            .SetProperty(o => o.Status, newStatus)
            .SetProperty(o => o.UpdatedAt, updatedAt));

    if (updated == 0)
        throw new ConflictException("Invalid order status transition.");

    if (newStatus == OrderStatus.Cancelled)
    {
        foreach (var item in order.Items.GroupBy(i => i.ProductId))
        {
            var quantity = item.Sum(i => i.Quantity);
            await _context.Products
                .Where(p => p.Id == item.Key)
                .ExecuteUpdateAsync(setters => setters
                    .SetProperty(
                        p => p.StockQuantity,
                        p => p.StockQuantity + quantity));
        }
    }

    await transaction.CommitAsync();

    var response = await GetByIdForAdminAsync(orderId);

    return response ?? throw new NotFoundException("Order not found.");
}

public async Task<List<OrderResponse>> GetAllAsync()
{
    return await _context.Orders
        .AsNoTracking()
        .OrderByDescending(o => o.CreatedAt)
        .Select(o => new OrderResponse
        {
            Id = o.Id,
            UserId = o.UserId,
            Status = o.Status.ToString(),
            TotalAmount = o.TotalAmount,
            CreatedAt = o.CreatedAt,

            Items = o.Items.Select(item => new OrderItemResponse
            {
                ProductId = item.ProductId,
                ProductName = item.Product.Name,
                Quantity = item.Quantity,
                UnitPrice = item.UnitPrice,
                Subtotal = item.UnitPrice * item.Quantity
            }).ToList()
        })
        .ToListAsync();
}

private async Task<OrderResponse?> GetByIdForAdminAsync(int orderId)
{
    return await _context.Orders
        .AsNoTracking()
        .Where(o => o.Id == orderId)
        .Select(o => new OrderResponse
        {
            Id = o.Id,
            UserId = o.UserId,
            Status = o.Status.ToString(),
            TotalAmount = o.TotalAmount,
            CreatedAt = o.CreatedAt,

            Items = o.Items.Select(item => new OrderItemResponse
            {
                ProductId = item.ProductId,
                ProductName = item.Product.Name,
                Quantity = item.Quantity,
                UnitPrice = item.UnitPrice,
                Subtotal = item.UnitPrice * item.Quantity
            }).ToList()
        })
            .FirstOrDefaultAsync();
}
}