using System.ComponentModel.DataAnnotations;

namespace OrderFlow.Api.DTOs.Auth;

public class RegisterRequest
{
    [Required]
    [MaxLength(100)]
    public string Name { get; set; } = "";

    [Required]
    [EmailAddress]
    [MaxLength(255)]
    public string Email { get; set; } = "";

    [Required]
    [MinLength(8)]
    public string Password { get; set; } = "";
}