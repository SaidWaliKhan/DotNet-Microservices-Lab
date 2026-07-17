namespace OrderService.Services;

public record ProductDto
(
    int Id,
    string Name,
    decimal Price,
    int StockQuantity
    
);