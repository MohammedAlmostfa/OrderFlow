namespace OrderFlow.Api.DTOs.Orders;

public class OrderItemResponse
{
    public int ProductId { get; set; }

    public string ProductName { get; set; } = "";

    public int Quantity { get; set; }

    public decimal UnitPrice { get; set; }

    public decimal Subtotal { get; set; }
}