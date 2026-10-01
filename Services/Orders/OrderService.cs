using Microsoft.EntityFrameworkCore;
using OrderFlow.Api.Data;
using OrderFlow.Api.DTOs.Orders;
using OrderFlow.Api.Models;

namespace OrderFlow.Api.Services.Orders;

public class OrderService : IOrderService
{
    private readonly AppDbContext _context;

    public OrderService(AppDbContext context)
    {
        _context = context;
    }

public async Task<OrderCreateResult> CreateAsync(
    int userId,
    CreateOrderRequest request)
    {
        // 1. Verify that the user exists.
        var userExists = await _context.Users
            .AnyAsync(u => u.Id == userId);

    if (!userExists)
{
    return new OrderCreateResult
    {
        Status = OrderCreateStatus.UserNotFound
    };
}

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
            .Where(p => productIds.Contains(p.Id))
            .ToListAsync();

        if (products.Count != productIds.Count)
{
    return new OrderCreateResult
    {
        Status = OrderCreateStatus.ProductNotFound
    };
}

        var productsById = products.ToDictionary(p => p.Id);

        // 4. Validate stock before creating the order.
        foreach (var item in requestedItems)
        {
            var product = productsById[item.ProductId];
if (product.StockQuantity < item.Quantity)
{
    return new OrderCreateResult
    {
        Status = OrderCreateStatus.InsufficientStock
    };
}

        }

        // 5. Start a database transaction.
        await using var transaction =
            await _context.Database.BeginTransactionAsync();

        // 6. Create the order.
        var order = new Order
        {
            UserId = userId,
            Status = "Pending",
            TotalAmount = 0
        };

        foreach (var item in requestedItems)
        {
            var product = productsById[item.ProductId];

            var orderItem = new OrderItem
            {
                ProductId = product.Id,
                Quantity = item.Quantity,

                // Snapshot of the current product price.
                UnitPrice = product.Price
            };

            order.Items.Add(orderItem);

            // Decrease the available stock.
            product.StockQuantity -= item.Quantity;

            // Calculate the total on the server.
            order.TotalAmount += product.Price * item.Quantity;
        }

        _context.Orders.Add(order);

        // 7. Save the order and stock changes together.
        await _context.SaveChangesAsync();

        // 8. Commit the transaction.
        await transaction.CommitAsync();

        // 9. Build the response.
      return new OrderCreateResult
{
    Status = OrderCreateStatus.Success,
    Order = new OrderResponse
    {
        Id = order.Id,
        UserId = order.UserId,
        Status = order.Status,
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
    }
};
    }
}