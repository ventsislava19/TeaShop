using System.ComponentModel.DataAnnotations;
using Microsoft.AspNetCore.Identity;

namespace TeaShop.Models;

// IdentityUser has id, email, password hash, username...

public class ApplicationUser : IdentityUser
{
    [Required] 
    [StringLength(100)]
    public string Name { get; set; } = string.Empty;
}