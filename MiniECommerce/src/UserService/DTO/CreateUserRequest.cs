using System.ComponentModel.DataAnnotations;

namespace UserService.DTO;

public record CreateUserRequest(
    [property: Required(ErrorMessage = "Name is required.")]
    [property: StringLength(100, MinimumLength = 2, ErrorMessage = "Name must be 2-100 characters.")]
    string Name,

    [property: Required(ErrorMessage = "Email is required.")]
    [property: EmailAddress(ErrorMessage = "Email must be a valid email address.")]
    string Email
);