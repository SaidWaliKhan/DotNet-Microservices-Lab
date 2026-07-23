using System.ComponentModel.DataAnnotations;
using UserService.Models;

namespace UserService.DTOs;

public record UserResponse(int Id, string Name, string Email)
{
    public static UserResponse FromEntity(User user) =>
    new(user.Id, user.Name, user.Email);

}
