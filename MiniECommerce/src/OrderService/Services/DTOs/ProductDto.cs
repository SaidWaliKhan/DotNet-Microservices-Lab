namespace OrderService.Services.DTOs;

public record ProductDto
(
    int Id,
    string Name,
    decimal Price,
    int StockQuantity
    
);