using System.ComponentModel.DataAnnotations.Schema;

namespace TeaShop.Models;

// Maps to "order_items" table - the junction between orders and products.
public class OrderItem
{
    public int Id { get; set; }
    
    // FK to the order this item belongs to.
    public int OrderId { get; set; }
    
    // Nav prop to the parent order.
    public Order? Order { get; set; }
    
    // FK to the product.
    public int ProductId { get; set; }
    
    // Nav prop to the product.
    public Product? Product { get; set; }
    
    [Column(TypeName = "decimal(10, 2)")]
    public decimal Price { get; set; }
    
    public int Quantity { get; set; }
}