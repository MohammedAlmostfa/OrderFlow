namespace OrderFlow.Api.DTOs.Orders;

public class OrderResponse
{
    public int Id { get; set; }

    public int UserId { get; set; }

    public string Status { get; set; } = "";

    public decimal TotalAmount { get; set; }

    public DateTime CreatedAt { get; set; }

    public List<OrderItemResponse> Items { get; set; } = [];
}