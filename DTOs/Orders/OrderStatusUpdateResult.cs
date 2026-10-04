using OrderFlow.Api.DTOs.Orders;

namespace OrderFlow.Api.Services.Orders;

public enum OrderStatusUpdateStatus
{
    NotFound,
    InvalidTransition,
    Success
}

public class OrderStatusUpdateResult
{
    public OrderStatusUpdateStatus Status { get; init; }
    public OrderResponse? Order { get; init; }
}