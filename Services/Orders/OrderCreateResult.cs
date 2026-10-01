namespace OrderFlow.Api.Services.Orders;
using OrderFlow.Api.DTOs.Orders;
public enum OrderCreateStatus
{
    Success,
    UserNotFound,
    ProductNotFound,
    InsufficientStock
}

public class OrderCreateResult
{
    public OrderCreateStatus Status { get; init; }

    public OrderResponse? Order { get; init; }
}