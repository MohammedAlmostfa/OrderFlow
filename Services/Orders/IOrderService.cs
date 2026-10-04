using OrderFlow.Api.DTOs.Orders;
using OrderFlow.Api.Models;

namespace OrderFlow.Api.Services.Orders;

public interface IOrderService
{
    Task<OrderResponse> CreateAsync(
        int userId,
        CreateOrderRequest request);

    Task<List<OrderResponse>> GetMyOrdersAsync(
        int userId);

    Task<OrderResponse> GetByIdAsync(
        int orderId,
        int userId);

    Task<List<OrderResponse>> GetAllAsync();

    Task<OrderResponse> UpdateStatusAsync(
        int orderId,
        OrderStatus newStatus);
}