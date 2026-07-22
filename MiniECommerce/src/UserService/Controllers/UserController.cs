using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using UserService.Data;
using UserService.DTO;
using UserService.Models;

namespace UserService.Controllers;

[ApiController]
[Route("api/[controller]")]
[Authorize]   // <-- add this line; requires a valid token for every action in this controller

public class UsersController : ControllerBase
{
    private readonly UserDbContext _db;

    public UsersController(UserDbContext db)
    {
        _db = db;
    }

    [HttpGet]
    public async Task<ActionResult<IEnumerable<UserResponse>>> GetAll()
    {
        var user = await _db.Users.AsTracking().ToListAsync();
        return Ok(user.Select(UserResponse.FromEntity));
    }

    [HttpGet("{id}")]
    public async Task<ActionResult<UserResponse>> GetById(int id)
    {
        var user = await _db.Users.AsNoTracking().FirstOrDefaultAsync(u => u.Id == id);
        if (user is null) return NotFound();
        return Ok(UserResponse.FromEntity(user));
    }

    
    [HttpDelete("{id}")]
    public async Task<IActionResult> Delete(int id)
    {
        var user = await _db.Users.FindAsync(id);

        if (user is null)
            return NotFound();

        _db.Users.Remove(user);

        await _db.SaveChangesAsync();

        return NoContent();
    }

    [HttpPut("{id}")]
    public async Task<IActionResult> Update(int id, User updateUser)
    {
        var user = await _db.Users.FindAsync(id);
        if (user == null) return NotFound();

        user.Name = updateUser.Name;
        user.Email = updateUser.Email;

        await _db.SaveChangesAsync();
        return Ok(user);
    }
}