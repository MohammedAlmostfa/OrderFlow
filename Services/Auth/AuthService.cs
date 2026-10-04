using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using OrderFlow.Api.Data;
using OrderFlow.Api.DTOs.Auth;
using OrderFlow.Api.Exceptions;
using OrderFlow.Api.Models;

namespace OrderFlow.Api.Services.Auth;

public class AuthService : IAuthService
{
    private readonly AppDbContext _context;
    private readonly PasswordHasher<User> _passwordHasher;
private readonly IJwtService _jwtService;
    public AuthService(
    AppDbContext context,
    PasswordHasher<User> passwordHasher,
    IJwtService jwtService)
{
    _context = context;
    _passwordHasher = passwordHasher;
    _jwtService = jwtService;
}

    public async Task<AuthResponse> RegisterAsync(
        RegisterRequest request)
    {
        var email = request.Email.Trim().ToLowerInvariant();

        var emailExists = await _context.Users
            .AnyAsync(u => u.Email == email);

        if (emailExists)
            throw new ConflictException("Email is already registered.");

        var user = new User
        {
            Name = request.Name.Trim(),
            Email = email,
            Role = "Customer"
        };

        user.PasswordHash = _passwordHasher.HashPassword(
            user,
            request.Password);

        _context.Users.Add(user);

        await _context.SaveChangesAsync();

       return new AuthResponse
{
    Id = user.Id,
    Name = user.Name,
    Email = user.Email,
    Role = user.Role,
    Token = _jwtService.GenerateToken(user)
};
    }

    public async Task<AuthResponse?> LoginAsync(
        LoginRequest request)
    {
        var email = request.Email.Trim().ToLowerInvariant();

        var user = await _context.Users
            .FirstOrDefaultAsync(u => u.Email == email);

        if (user is null)
            return null;

        var result = _passwordHasher.VerifyHashedPassword(
            user,
            user.PasswordHash,
            request.Password);

        if (result == PasswordVerificationResult.Failed)
            return null;

      return new AuthResponse
{
    Id = user.Id,
    Name = user.Name,
    Email = user.Email,
    Role = user.Role,
    Token = _jwtService.GenerateToken(user)
};
    }
}