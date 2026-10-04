using OrderFlow.Api.Models;

namespace OrderFlow.Api.Services.Orders;

public class OrderStatusService
{
    public bool IsValidTransition(
        OrderStatus current,
        OrderStatus next)
    {
        return current switch
        {
            OrderStatus.Pending =>
                next is OrderStatus.Confirmed
                    or OrderStatus.Cancelled,

            OrderStatus.Confirmed =>
                next is OrderStatus.Processing
                    or OrderStatus.Cancelled,

            OrderStatus.Processing =>
                next == OrderStatus.Shipped,

            OrderStatus.Shipped =>
                next == OrderStatus.Delivered,

            OrderStatus.Delivered =>
                false,

            OrderStatus.Cancelled =>
                false,

            _ => false
        };
    }
}