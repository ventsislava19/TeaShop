using System.ComponentModel.DataAnnotations;
using TeaShop.Models;

namespace TeaShop.ViewModels;

public class ProductFormViewModel
{
    public int Id { get; set; }

    [Required]
    [StringLength(150)]
    public string Name { get; set; } = string.Empty;

    public string? Description { get; set; }

    [Required]
    [Range(0.01, 9999.99)]
    public decimal Price { get; set; }

    [StringLength(255)]
    [Display(Name = "Image Filename")]
    public string? Image { get; set; }

    [Required(ErrorMessage = "Please select a category.")]
    [Display(Name = "Category")]
    public int CategoryId { get; set; }

    [Required]
    [StringLength(50)]
    [Display(Name = "Origin Country")]
    public string OriginCountry { get; set; } = string.Empty;

    [Required]
    public CaffeineType Caffeine { get; set; } = CaffeineType.NotApplicable;
    
    [Required]
    [Range(0, int.MaxValue)]
    [Display(Name = "Stock Quantity")]
    public int Stock { get; set; } = 0;
}


