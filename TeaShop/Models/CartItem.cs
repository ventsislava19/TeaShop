namespace TeaShop.Models;
 
// This is not a database table, it's a simple object stored in the session.
// In C#, a typed class is used instead of an associative array.
public class CartItem
{
    public int ProductId { get; set; }
    public string Name { get; set; } = string.Empty;
    public decimal Price { get; set; }
    public string? Image { get; set; }
    public int Quantity { get; set; }
    public decimal Subtotal => Price * Quantity;
}