using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using OrderFlow.Api.DTOs.Orders;
using OrderFlow.Api.Models;
using OrderFlow.Api.Services.Orders;

namespace OrderFlow.Api.Controllers;

[ApiController]
[Route("api/admin/orders")]
[Authorize(Roles = "Admin")]
public class AdminOrdersController : ControllerBase
{
    private readonly IOrderService _orderService;

    public AdminOrdersController(IOrderService orderService)
    {
        _orderService = orderService;
    }

    [HttpGet]
    public async Task<IActionResult> GetAll()
    {
        var orders = await _orderService.GetAllAsync();

        return Ok(orders);
    }

    [HttpPatch("{id:int}/status")]
    public async Task<IActionResult> UpdateStatus(
        int id,
        UpdateOrderStatusRequest request)
    {
        var result = await _orderService.UpdateStatusAsync(
            id,
            request.Status);

        return result.Status switch
        {
            OrderStatusUpdateStatus.NotFound =>
                NotFound(new
                {
                    message = "Order not found."
                }),

            OrderStatusUpdateStatus.InvalidTransition =>
                Conflict(new
                {
                    message = "Invalid order status transition."
                }),

            OrderStatusUpdateStatus.Success =>
                Ok(result.Order),

            _ => StatusCode(500)
        };
    }
}