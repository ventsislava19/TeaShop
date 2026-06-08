using System.ComponentModel.DataAnnotations;
using TeaShop.Models;
 
namespace TeaShop.ViewModels;
 
public class CheckoutViewModel
{
    [Required]
    [Display(Name = "Payment")]
    public string PaymentMethod { get; set; } = "PayNow";
    
    // Shipping info.
    [Required(ErrorMessage = "Please enter your address.")]
    [Display(Name = "Street Address")]
    [StringLength(200)]
    public string ShippingAddress { get; set; } = string.Empty;
 
    [Required(ErrorMessage = "Please enter your city.")]
    [Display(Name = "City")]
    [StringLength(100)]
    public string ShippingCity { get; set; } = string.Empty;
 
    [Required(ErrorMessage = "Please enter your postal code.")]
    [Display(Name = "Postal Code")]
    [StringLength(20)]
    public string ShippingPostalCode { get; set; } = string.Empty;
 
    // Cart items (read-only, displayed on the checkout page for review).
    public List<CartItem> CartItems { get; set; } = new();
    public decimal Total => CartItems.Sum(i => i.Subtotal);
}

