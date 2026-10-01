using OrderFlow.Api.DTOs.Orders;

namespace OrderFlow.Api.Services.Orders;

public interface IOrderService
{
Task<OrderCreateResult> CreateAsync(
    int userId,
    CreateOrderRequest request);
}