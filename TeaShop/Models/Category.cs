using System.ComponentModel.DataAnnotations;

namespace TeaShop.Models;

// Maps the "categories" table from eshop.sql.
// For now it's one category - teas, but accessories, teaware... can be added later.

public class Category
{
    public int Id { get; set; }
    
    [Required] 
    [StringLength(100)]
    public string Name { get; set; }
    
    public string? Description { get; set; }
    
    public DateTime CreatedAt { get; set; } =  DateTime.UtcNow;
    
    public ICollection<Product> Products { get; set; } = new HashSet<Product>();
    
    
    
    
}