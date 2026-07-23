// Controllers/AuthController.cs — final corrected version
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using UserService.Data;
using UserService.DTOs;
using UserService.Models;
using UserService.Services;

namespace UserService.Controllers;

[ApiController]
[Route("api/[controller]")] // -> /api/auth
public class AuthController : ControllerBase
{
    private readonly UserDbContext _db;
    private readonly IJwtTokenService _tokenService;
    private readonly IPasswordHasherService _passwordHasher;

    public AuthController(UserDbContext db, IJwtTokenService tokenService, IPasswordHasherService passwordHasher)
    {
        _db = db;
        _tokenService = tokenService;
        _passwordHasher = passwordHasher;
    }

    [HttpPost("register")]
    public async Task<ActionResult<AuthResponse>> Register(RegisterRequest request)
    {
        var emailExists = await _db.Users.AnyAsync(u => u.Email == request.Email);
        if (emailExists)
            return Conflict($"A user with email '{request.Email}' already exists.");

        var user = new User
        {
            Name = request.Name,
            Email = request.Email,
            PasswordHash = _passwordHasher.Hash(request.Password)
        };

        _db.Users.Add(user);

        try
        {
            await _db.SaveChangesAsync();
        }
        catch (DbUpdateException)
        {
            return Conflict($"A user with email '{request.Email}' already exists.");
        }

        var (token, expiresAtUtc) = _tokenService.GenerateToken(user);
        return Ok(new AuthResponse(token, expiresAtUtc, UserResponse.FromEntity(user)));
    }

    [HttpPost("login")]
    public async Task<ActionResult<AuthResponse>> Login(LoginRequest request)
    {
        var user = await _db.Users.FirstOrDefaultAsync(u => u.Email == request.Email);

        if (user is null || !_passwordHasher.Verify(request.Password, user.PasswordHash))
            return Unauthorized("Invalid email or password.");

        var (token, expiresAtUtc) = _tokenService.GenerateToken(user);
        return Ok(new AuthResponse(token, expiresAtUtc, UserResponse.FromEntity(user)));
    }
}