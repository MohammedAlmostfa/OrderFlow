using Microsoft.AspNetCore.Mvc;
using OrderFlow.Api.DTOs.Auth;
using OrderFlow.Api.Services.Auth;
using Microsoft.AspNetCore.Authorization;
namespace OrderFlow.Api.Controllers;

[ApiController]
[Route("api/[controller]")]
public class AuthController : ControllerBase
{
    private readonly IAuthService _authService;

    public AuthController(IAuthService authService)
    {
        _authService = authService;
    }

    [HttpPost("register")]
    public async Task<IActionResult> Register(
        RegisterRequest request)
    {
        var result = await _authService.RegisterAsync(request);

        return Ok(result);
    }

    [HttpPost("login")]
    public async Task<IActionResult> Login(
        LoginRequest request)
    {
        var result = await _authService.LoginAsync(request);

        if (result is null)
        {
            return Unauthorized(new
            {
                message = "Invalid email or password."
            });
        }

        return Ok(result);
    }


   
[HttpGet("me")]
public IActionResult Me()
{
    return Ok(new
    {
        message = "You are authenticated.",
        userId = User.FindFirst("nameid")?.Value,
        name = User.Identity?.Name,
        email = User.FindFirst("email")?.Value,
        role = User.FindFirst("role")?.Value
    });
}
}