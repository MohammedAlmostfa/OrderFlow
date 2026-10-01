using System.ComponentModel.DataAnnotations;

namespace OrderFlow.Api.DTOs.Orders;

public class CreateOrderRequest
{
    [Required]
    [MinLength(1)]
    public List<CreateOrderItemRequest> Items { get; set; } = [];
}