namespace OrderService.Models;

public class Order
{
    public int Id { get; set; }
    public int ProductId { get; set; }  
    public int UserId { get; set; }
    public string ProductName { get; set; } = string.Empty; // copied at order time
    public int Quantity { get; set; }
    public decimal TotalPrice { get; set; }
    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
    public string Status { get; set; } = "Pending";
}


public class CreateOrderRequest
{
    public int ProductId { get; set; }
    public int Quantity { get; set; }
    public int UserId { get; set; }
}