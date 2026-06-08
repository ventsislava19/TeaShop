using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace TeaShop.Models;

// Maps to "orders" table.

public class Order
{
    public int Id { get; set; }
    
    // Foreign key to the user who placed the order.
    [Required]
    public string UserId { get; set; } = string.Empty;
    
    // Nav prop to the user.
    public ApplicationUser? User { get; set; }
    
    [Required]
    [Column(TypeName = "decimal(10, 2)")]
    public decimal Total { get; set; }

    [Required] 
    [StringLength(30)] 
    public OrderStatus Status { get; set; } = OrderStatus.AwaitingPayment;
    
    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
    
    public DateTime StatusUpdatedAt { get; set; } = DateTime.UtcNow;
    
    [StringLength(200)]
    public string? ShippingAddress { get; set; }
    
    [StringLength(100)]
    public string? ShippingCity { get; set; }
    
    [StringLength(20)]
    public string? ShippingPostalCode{ get; set; }

    // Navigation property - one order has many order items (unique).
    public ICollection<OrderItem> OrderItems { get; set; } = new HashSet<OrderItem>();
}