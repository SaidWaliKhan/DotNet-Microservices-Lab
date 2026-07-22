using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using ProductService.Data;
using ProductService.DTOs;
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
    [Authorize]
    public async Task<ActionResult<Product>> Create(CreateProductRequest request)
    {

        var product = new Product
        {
            Name = request.Name,
            Price = request.Price,
            StockQuantity = request.StockQuantity
        };


        _db.Products.Add(product);
        await _db.SaveChangesAsync();
        return CreatedAtAction(nameof(GetById), new { id = product.Id }, product);
    }


    [HttpPut("{id}/reduce-stock/{quantity}")]
    [Authorize]
    public async Task<IActionResult> ReduceStock(int id, int quantity)
    {
        if (quantity <= 0)
            return BadRequest("Quntity Must be greater than Zero");

        var rowAffected = await _db.Products
        .Where(p => p.Id == id && p.StockQuantity >= quantity)
        .ExecuteUpdateAsync(set =>
        set.SetProperty(p => p.StockQuantity, p => p.StockQuantity - quantity));

        if (rowAffected == 0)
        {
            var exist = await _db.Products.AnyAsync(p => p.Id == id);
            return exist ? BadRequest("Not enough stock") : NotFound();
        }

        return Ok();

    }
    [HttpPut("{id}/restore-stock/{quantity}")]
    [Authorize]
    public async Task<IActionResult> RestoreStock(int id, int quantity)
    {
        if (quantity <= 0)
            return BadRequest("Quantity must be greater than 0.");

        var rowsAffected = await _db.Products
            .Where(p => p.Id == id)
            .ExecuteUpdateAsync(setters =>
                setters.SetProperty(p => p.StockQuantity, p => p.StockQuantity + quantity));

        return rowsAffected == 0 ? NotFound() : Ok();
    }




    [HttpDelete("{id}")]
    public async Task<IActionResult> Delete(int id)
    {
        var product = await _db.Products.FindAsync(id);
        if (product == null)
            return NotFound();

        _db.Products.Remove(product);
        await _db.SaveChangesAsync();

        return NoContent(); 
        
    }

    
}