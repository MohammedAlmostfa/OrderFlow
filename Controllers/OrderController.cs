using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using OrderFlow.Api.DTOs.Orders;
using OrderFlow.Api.Services.Orders;
using System.Security.Claims;

namespace OrderFlow.Api.Controllers;

[ApiController]
[Route("api/[controller]")]
[Authorize]
public class OrderController : ControllerBase
{
    private readonly IOrderService _orderService;

    public OrderController(IOrderService orderService)
    {
        _orderService = orderService;
    }

    [HttpPost]
    public async Task<IActionResult> Create(
        CreateOrderRequest request)
    {
        var userIdClaim = User.FindFirst(
            ClaimTypes.NameIdentifier);

        if (userIdClaim is null)
        {
            return Unauthorized();
        }

        if (!int.TryParse(userIdClaim.Value, out var userId))
        {
            return Unauthorized();
        }

        var result = await _orderService.CreateAsync(
            userId,
            request);

        return result.Status switch
        {
            OrderCreateStatus.UserNotFound =>
                NotFound(new
                {
                    message = "User not found."
                }),

            OrderCreateStatus.ProductNotFound =>
                NotFound(new
                {
                    message = "One or more products were not found."
                }),

            OrderCreateStatus.InsufficientStock =>
                Conflict(new
                {
                    message = "Insufficient stock."
                }),

            OrderCreateStatus.Success =>
                Ok(result.Order),

            _ => StatusCode(500)
        };
    }
    [HttpGet("my-orders")]
public async Task<IActionResult> GetMyOrders()
{
    var userIdClaim = User.FindFirst(
        ClaimTypes.NameIdentifier);

    if (userIdClaim is null)
    {
        return Unauthorized();
    }

    if (!int.TryParse(userIdClaim.Value, out var userId))
    {
        return Unauthorized();
    }

    var orders = await _orderService
        .GetMyOrdersAsync(userId);

    return Ok(orders);
}

[HttpGet("{id:int}")]
public async Task<IActionResult> GetById(int id)
{
    var userIdClaim = User.FindFirst(
        ClaimTypes.NameIdentifier);

    if (userIdClaim is null)
    {
        return Unauthorized();
    }

    if (!int.TryParse(userIdClaim.Value, out var userId))
    {
        return Unauthorized();
    }

    var order = await _orderService.GetByIdAsync(
        id,
        userId);

    if (order is null)
    {
        return NotFound(new
        {
            message = "Order not found."
        });
    }

    return Ok(order);
}
}