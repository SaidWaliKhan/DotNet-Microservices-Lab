using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using OrderService.Data;
using OrderService.Models;
using OrderService.Services.HttpServices;

namespace OrderService.Controllers;

[ApiController]
[Route("api/[controller]")]
public class OrdersController : ControllerBase
{
    private readonly OrderDbContext _db;
    private readonly ProductServiceClient _productClient;
    private readonly UserServiceClient _userServiceClient;

    public OrdersController(OrderDbContext db, ProductServiceClient productClient, UserServiceClient userServiceClient)
    {
        _db = db;
        _productClient = productClient;
        _userServiceClient = userServiceClient;
    }

    // GET /api/orders
    [HttpGet]
    public async Task<ActionResult<IEnumerable<Order>>> GetAll()
    {
        return Ok(await _db.Orders.ToListAsync());
    }

    [HttpPost]
    public async Task<ActionResult<Order>> Create(CreateOrderRequest request)
    {
        // Ask the user first: does this user exist?
        var user = await _userServiceClient.GetUserAsync(request.UserId);
        if (user is null)
            return NotFound($"User {request.UserId} not found.");

        // Does this product exist?
        var product = await _productClient.GetProductAsync(request.ProductId);
        if (product is null)
            return NotFound($"Product {request.ProductId} not found.");

        if (product.StockQuantity < request.Quantity)
            return BadRequest($"Not enough stock. Available: {product.StockQuantity}");

        // Reduce the stock
        var stockReduced = await _productClient.ReduceStockAsync(request.ProductId, request.Quantity);
        if (!stockReduced)
            return BadRequest("Could not reserve stock. Try again.");

        var order = new Order
        {
            UserId = user.Id,
            UserName = user.Name,
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
            // Compensating transaction: the local save failed AFTER we already
            // told Product Service to reduce stock remotely. We must undo that
            // remote side-effect, or stock permanently drifts with no order
            // to explain where it went. Note: this calls a DIFFERENT endpoint
            // (restore, not reduce) with the correct ProductId.
            await _productClient.RestoreStockAsync(request.ProductId, request.Quantity);
            throw; // re-throw so the global exception middleware still returns a proper 500
        }

        // This was the missing piece - every code path must return something.
        return CreatedAtAction(nameof(GetById), new { id = order.Id }, order);
    }

    // GET /api/orders/1
    [HttpGet("{id}")]
    public async Task<ActionResult<Order>> GetById(int id)
    {
        var order = await _db.Orders.FindAsync(id);
        if (order is null) return NotFound();
        return Ok(order);
    }
}