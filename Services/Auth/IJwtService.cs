using OrderFlow.Api.Models;

namespace OrderFlow.Api.Services.Auth;

public interface IJwtService
{
    string GenerateToken(User user);
}