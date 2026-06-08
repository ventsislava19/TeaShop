using System.ComponentModel.DataAnnotations;

namespace TeaShop.ViewModels;

public class LoginViewModel
{
    [Required]
    [EmailAddress]
    public string Email { get; set; } = string.Empty;
 
    [Required]
    [DataType(DataType.Password)]
    public string Password { get; set; } = string.Empty;
}


