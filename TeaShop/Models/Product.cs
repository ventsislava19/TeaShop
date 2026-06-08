using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace TeaShop.Models;

// Maps to "products" table.
public class Product
{
    public int Id { get; set; }
    
    [Required] 
    [StringLength(150)]
    public string Name { get; set; } = string.Empty;
    
    public string Description { get; set; }
    
    [Required]
    [Column(TypeName = "decimal(10, 2)")]
    public decimal Price { get; set; }
    
    [StringLength(255)]
    public string? Image { get; set; }
    
    // Foreign ket to Category (nullable).
    public int CategoryId { get; set; }
    
    // Nav property - EF automatically joins to categories using CategoryId.
    public Category? Category { get; set; }
    
    [Required]
    [StringLength(50)]
    public string OriginCountry { get; set; } = string.Empty;
    
    [Required]
    public CaffeineType Caffeine { get; set; } = CaffeineType.Caffeinated;
    
    [Required]
    [Range(0, int.MaxValue)]
    public int Stock { get; set; } = 0;
    
    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
}

