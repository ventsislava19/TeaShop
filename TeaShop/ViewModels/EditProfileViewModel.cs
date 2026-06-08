using System.ComponentModel.DataAnnotations;

namespace TeaShop.ViewModels;

public class EditProfileViewModel
{
    [Required]
    [StringLength(100)]
    [Display(Name = "Full Name")]
    public string Name { get; set; } = string.Empty;

    [Required]
    [EmailAddress]
    public string Email { get; set; } = string.Empty;
}

