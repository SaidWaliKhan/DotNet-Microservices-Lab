using System.ComponentModel.DataAnnotations;

namespace ProductService.DTOs;

public record CreateProductRequest(
[property :Required(ErrorMessage = "product Name is required")]
[StringLength(100, MinimumLength =2, ErrorMessage = "Names must be 2-100 characters")]
string Name,
[property:Range(0.01, 1_000_000, ErrorMessage = "price must be greater than zero")]
decimal Price,
[property:Range(0, int.MaxValue, ErrorMessage = "Quantity cannot be negative")]
int StockQuantity);