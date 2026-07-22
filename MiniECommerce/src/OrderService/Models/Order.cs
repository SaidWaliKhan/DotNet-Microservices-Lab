using System.ComponentModel.DataAnnotations;

namespace OrderService.Models;

public class Order
{
    public int Id { get; set; }
    public int UserId { get; set; }

    public string UserName { get; set; } = string.Empty;
    public int ProductId { get; set; }  
    
    public string ProductName { get; set; } = string.Empty; 
    public int Quantity { get; set; }
    public decimal TotalPrice { get; set; }
    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
    public string Status { get; set; } = "Pending";
}


public class CreateOrderRequest
{
    [Range(1, int.MaxValue, ErrorMessage = "User must be a valid positive Id")]
    public int ProductId { get; set; }
    
    [Range(1, int.MaxValue, ErrorMessage = "Product must be a valid positive Id")]
    public int Quantity { get; set; }
}