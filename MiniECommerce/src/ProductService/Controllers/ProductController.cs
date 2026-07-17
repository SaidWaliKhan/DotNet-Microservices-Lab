using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using ProductService.Data;
using ProductService.Models;

namespace ProductService.Controllers;

[ApiController]
[Route("api/[controller]")] // becomes: /api/products
public class ProductsController : ControllerBase
{
    private readonly ProductDbContext _db;

    public ProductsController(ProductDbContext db)
    {
        _db = db;
    }

    // GET /api/products
    [HttpGet]
    public async Task<ActionResult<IEnumerable<Product>>> GetAll()
    {
        return Ok(await _db.Products.ToListAsync());
    }

    // GET /api/products/1
    [HttpGet("{id}")]
    public async Task<ActionResult<Product>> GetById(int id)
    {
        var product = await _db.Products.FindAsync(id);
        if (product is null) return NotFound();
        return Ok(product);
    }

    // POST /api/products
    [HttpPost]
    public async Task<ActionResult<Product>> Create(Product product)
    {
        _db.Products.Add(product);
        await _db.SaveChangesAsync();
        return CreatedAtAction(nameof(GetById), new { id = product.Id }, product);
    }

    // PUT /api/products/1/reduce-stock/2
    // This is the endpoint OrderService will call when someone places an order,
    // to check AND reduce stock. This is our first inter-service communication!
    [HttpPut("{id}/reduce-stock/{quantity}")]
    public async Task<IActionResult> ReduceStock(int id, int quantity)
    {
        var product = await _db.Products.FindAsync(id);
        if (product is null) return NotFound();

        if (product.StockQuantity < quantity)
            return BadRequest($"Not enough stock. Available: {product.StockQuantity}");

        product.StockQuantity -= quantity;
        await _db.SaveChangesAsync();
        return Ok(product);
    }
}