using System.ComponentModel.DataAnnotations;

namespace UserService.Models;

public class User
{
    public int Id { get; set; }
    [Required(ErrorMessage = "Name is required")]
    [StringLength(100, MinimumLength = 2, ErrorMessage = ("Name must be 2-100 characters"))]
    public string Name { get; set; } = string.Empty;
    [Required(ErrorMessage = ("Email is reuired"))]
    [EmailAddress(ErrorMessage =("Email must be a valid email"))]
    public string Email { get; set; } = string.Empty;
}