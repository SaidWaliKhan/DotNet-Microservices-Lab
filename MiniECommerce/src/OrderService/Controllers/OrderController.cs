using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using OrderService.Data;
using OrderService.Models;
using OrderService.Services;

namespace OrderService.Controllers;

[ApiController]
[Route("api/[controller]")]
public class OrdersController : ControllerBase
{
    private readonly OrderDbContext _db;
    private readonly ProductServiceClient _productClient;

    public OrdersController(OrderDbContext db, ProductServiceClient productClient)
    {
        _db = db;
        _productClient = productClient;
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
            ProductId = product.Id,
            ProductName = product.Name,
            Quantity = request.Quantity,
            TotalPrice = product.Price * request.Quantity,
            Status = "Confirmed"
        };

        _db.Orders.Add(order);
        await _db.SaveChangesAsync();

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