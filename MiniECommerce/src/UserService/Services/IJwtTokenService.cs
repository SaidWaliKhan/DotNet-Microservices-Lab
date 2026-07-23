using UserService.Models;

namespace UserService.Services;
public interface IJwtTokenService
{
    (string Token, DateTime ExpiresAtUtc) GenerateToken(User user);
}