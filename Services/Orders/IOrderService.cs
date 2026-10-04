using OrderFlow.Api.DTOs.Orders;
using OrderFlow.Api.Models;

namespace OrderFlow.Api.Services.Orders;

public interface IOrderService
{
    Task<OrderCreateResult> CreateAsync(
        int userId,
        CreateOrderRequest request);

    Task<List<OrderResponse>> GetMyOrdersAsync(
        int userId);

    Task<OrderResponse?> GetByIdAsync(
        int orderId,
        int userId);

    Task<List<OrderResponse>> GetAllAsync();

    Task<OrderStatusUpdateResult> UpdateStatusAsync(
        int orderId,
        OrderStatus newStatus);
}