using System.ComponentModel.DataAnnotations;
using OrderFlow.Api.Models;

namespace OrderFlow.Api.DTOs.Orders;

public class UpdateOrderStatusRequest
{
    [Required]
    public OrderStatus Status { get; set; }
}