using System.ComponentModel.DataAnnotations;

namespace UserService.DTOs;
public record LoginRequest(
    [Required] string Email,
    [Required] string Password
);


public record RegisterRequest(
    [Required(ErrorMessage = "Name is required.")]
    [StringLength(100, MinimumLength = 2, ErrorMessage = "Name must be 2-100 characters.")]
    string Name,

    [Required(ErrorMessage = "Email is required.")]
    [EmailAddress(ErrorMessage = "Email must be a valid email address.")]
    string Email,

    [Required(ErrorMessage = "Password is required.")]
    [StringLength(100, MinimumLength = 8, ErrorMessage = "Password must be at least 8 characters.")]
    string Password
);