namespace ProductService.Models;

// This is our "entity" - EF Core will turn this class into a database table.
// Each property becomes a column.
public class Product
{
    public int Id { get; set; }
    public string Name { get; set; } = string.Empty;
    public decimal Price { get; set; }
    public int StockQuantity { get; set; }
}