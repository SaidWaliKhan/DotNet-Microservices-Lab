namespace UserService.DTOs;
public record AuthResponse(

string Token,
DateTime ExpiresAtUtc,
UserResponse User
);