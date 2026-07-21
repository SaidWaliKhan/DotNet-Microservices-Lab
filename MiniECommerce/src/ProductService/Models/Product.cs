using System.ComponentModel.DataAnnotations;

namespace ProductService.Models;

public class Product
{
    public int Id { get; set; }
    [Required(ErrorMessage = "Product Name is required")]
    [StringLength(100, MinimumLength = 2, ErrorMessage = "Name  must be 2-100 characters")]
    public string Name { get; set; } = string.Empty;
    [Range(0.01, 1_000_000, ErrorMessage = "proce must be greater than zero")]
    public decimal Price { get; set; }
    [Range(0, int.MaxValue, ErrorMessage = "Stock Quantity cannot be negative")]
    public int StockQuantity { get; set; }
}