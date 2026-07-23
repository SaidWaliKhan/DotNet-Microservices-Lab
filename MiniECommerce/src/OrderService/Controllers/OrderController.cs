using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using OrderService.Data;
using OrderService.Models;
using OrderService.Services.HttpService;

namespace OrderService.Controllers;

[ApiController]
[Route("api/[controller]")]
[Authorize] // every endpoint here requires a valid JWT
public class OrdersController : ControllerBase
{
    private readonly OrderDbContext _db;
    private readonly ProductServiceClient _productClient;

       public OrdersController(OrderDbContext db, ProductServiceClient productClient)
    {
        _db = db;
        _productClient = productClient;
    }

    [HttpGet]
    public async Task<ActionResult<IEnumerable<Order>>> GetAll()
    {
        var userId = GetUserIdFromToken();
        if (userId is null) return Unauthorized("Token did not contain a valid user id.");

        var orders = await _db.Orders
            .AsNoTracking()
            .Where(o => o.UserId == userId.Value)
            .ToListAsync();

        return Ok(orders);
    }

    // POST /api/orders
    [HttpPost]
    public async Task<ActionResult<Order>> Create(CreateOrderRequest request)
    {
        // Read the user's identity from the VALIDATED token's claims -
        // signature is genuine and it hasn't expired.
        var userId = GetUserIdFromToken();
        if (userId is null)
            return Unauthorized("Token did not contain a valid user id.");

        var userName = User.FindFirst(ClaimTypes.Name)?.Value ?? "Unknown";

        var product = await _productClient.GetProductAsync(request.ProductId);
        if (product is null)
            return NotFound($"Product {request.ProductId} not found.");

        if (product.StockQuantity < request.Quantity)
            return BadRequest($"Not enough stock. Available: {product.StockQuantity}");

        var stockReduced = await _productClient.ReduceStockAsync(request.ProductId, request.Quantity);
        if (!stockReduced)
            return BadRequest("Could not reserve stock. Try again.");

        var order = new Order
        {
            UserId = userId.Value,
            UserName = userName,
            ProductId = product.Id,
            ProductName = product.Name,
            Quantity = request.Quantity,
            TotalPrice = product.Price * request.Quantity,
            Status = "Confirmed"
        };

        _db.Orders.Add(order);

        try
        {
            await _db.SaveChangesAsync();
        }
        catch (Exception)
        {
            await _productClient.RestoreStockAsync(request.ProductId, request.Quantity);
            throw;
        }

        return CreatedAtAction(nameof(GetById), new { id = order.Id }, order);
    }

    [HttpGet("{id}")]
    public async Task<ActionResult<Order>> GetById(int id)
    {
        var userId = GetUserIdFromToken();
        if (userId is null) return Unauthorized("Token did not contain a valid user id.");

        var order = await _db.Orders.AsNoTracking().FirstOrDefaultAsync(o => o.Id == id);
        if (order is null) return NotFound();

        if (order.UserId != userId.Value)
            return Forbid();

        return Ok(order);
    }

    // Pulls the "sub" claim (the standard JWT claim for user ID) off the
    // current request's ClaimsPrincipal
    private int? GetUserIdFromToken()
    {
        var subClaim = User.FindFirst(JwtRegisteredClaimNames.Sub)?.Value;
        return int.TryParse(subClaim, out var userId) ? userId : null;
    }
}