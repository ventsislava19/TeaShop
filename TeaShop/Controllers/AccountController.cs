using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using TeaShop.Models;
using TeaShop.ViewModels;

namespace TeaShop.Controllers;

// ASP.NET Identity handles all of hashing and sessions through UserManager and SignInManager.
public class AccountController : Controller
{
    private readonly UserManager<ApplicationUser> _userManager;
    private readonly SignInManager<ApplicationUser> _signInManager;

    // These are injected by the framework (DI), just like the DbContext was.
    public AccountController(
        UserManager<ApplicationUser> userManager,
        SignInManager<ApplicationUser> signInManager)
    {
        _userManager = userManager;
        _signInManager = signInManager;
    }

    // 1. REGISTER

    // GET: /Account/Register
    public IActionResult Register()
    {
        // If already logged in, redirect to home.
        if (User.Identity?.IsAuthenticated == true)
            return RedirectToAction("Index", "Home");

        return View();
    }

    // POST: /Account/Register
    [HttpPost]
    [ValidateAntiForgeryToken]  // CSRF protection  like a hidden token.
    public async Task<IActionResult> Register(RegisterViewModel model)
    {
        if (!ModelState.IsValid)
            return View(model);

        // Create the user object.
        var user = new ApplicationUser
        {
            Name = model.Name,
            UserName = model.Email, // Identity uses UserName for login; we set it to email.
            Email = model.Email
        };

        // CreateAsync hashes the password and saves to DB.
        var result = await _userManager.CreateAsync(user, model.Password);

        if (result.Succeeded)
        {
            // Sign in immediately after registration.
            await _signInManager.SignInAsync(user, isPersistent: false);
            return RedirectToAction("Index", "Home");
        }

        // If something went wrong (like email already taken) show errors.
        foreach (var error in result.Errors)
        {
            ModelState.AddModelError(string.Empty, error.Description);
        }

        return View(model);
    }

    // 2. LOGIN

    // GET: /Account/Login
    public IActionResult Login()
    {
        if (User.Identity?.IsAuthenticated == true)
            return RedirectToAction("Index", "Home");

        return View();
    }

    // POST: /Account/Login
    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Login(LoginViewModel model)
    {
        if (!ModelState.IsValid)
            return View(model);

        // PasswordSignInAsync finds the user, verifies the password hash,creates the cookie.
        var result = await _signInManager.PasswordSignInAsync(
            model.Email,
            model.Password,
            isPersistent: false,
            lockoutOnFailure: false);

        if (result.Succeeded)
        {
            return RedirectToAction("Index", "Home");
        }

        ModelState.AddModelError(string.Empty, "Invalid email or password.");
        return View(model);
    }

    // 3. LOGOUT

    // POST: /Account/Logout
    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Logout()
    {
        // SignOutAsync clears the auth cookie.
        await _signInManager.SignOutAsync();
        return RedirectToAction("Index", "Home");
    }

    // 4. ACCOUNT PAGE

    // GET: /Account
    [Authorize]  // If the user isn't logged in, they get redirected to /Account/Login.
    public async Task<IActionResult> Index()
    {
        var user = await _userManager.GetUserAsync(User);
        return View(user);
    }
    
    // 4.1 EDIT PROFILE

    // GET: /Account/EditProfile
    [Authorize]
    public async Task<IActionResult> EditProfile()
    {
        var user = await _userManager.GetUserAsync(User);
        if (user == null) return RedirectToAction("Login");

        // Admin can't edit profile from here.
        if (await _userManager.IsInRoleAsync(user, "Admin"))
            return RedirectToAction("Index");

        var model = new EditProfileViewModel
        {
            Name = user.Name,
            Email = user.Email ?? string.Empty
        };
        return View(model);
    }

    // POST: /Account/EditProfile
    [HttpPost]
    [Authorize]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> EditProfile(EditProfileViewModel model)
    {
        if (!ModelState.IsValid)
            return View(model);

        var user = await _userManager.GetUserAsync(User);
        if (user == null) return RedirectToAction("Login");

        if (await _userManager.IsInRoleAsync(user, "Admin"))
            return RedirectToAction("Index");

        user.Name = model.Name;

        // If email changed, update both Email and UserName.
        if (user.Email != model.Email)
        {
            var existingUser = await _userManager.FindByEmailAsync(model.Email);
            if (existingUser != null && existingUser.Id != user.Id)
            {
                ModelState.AddModelError("Email", "This email is already taken.");
                return View(model);
            }
            user.Email = model.Email;
            user.UserName = model.Email;
        }

        await _userManager.UpdateAsync(user);
        return RedirectToAction("Index");
    }

    // 4.2 CHANGE PASSWORD

    // GET: /Account/ChangePassword
    [Authorize]
    public async Task<IActionResult> ChangePassword()
    {
        var user = await _userManager.GetUserAsync(User);
        if (user == null) return RedirectToAction("Login");

        if (await _userManager.IsInRoleAsync(user, "Admin"))
            return RedirectToAction("Index");

        return View(new ChangePasswordViewModel());
    }

    // POST: /Account/ChangePassword
    [HttpPost]
    [Authorize]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> ChangePassword(ChangePasswordViewModel model)
    {
        if (!ModelState.IsValid)
            return View(model);

        var user = await _userManager.GetUserAsync(User);
        if (user == null) return RedirectToAction("Login");

        if (await _userManager.IsInRoleAsync(user, "Admin"))
            return RedirectToAction("Index");

        var result = await _userManager.ChangePasswordAsync(user, model.CurrentPassword, model.NewPassword);

        if (result.Succeeded)
        {
            await _signInManager.RefreshSignInAsync(user);
            TempData["SuccessMessage"] = "Password changed successfully.";
            return RedirectToAction("Index");
        }

        foreach (var error in result.Errors)
        {
            ModelState.AddModelError(string.Empty, error.Description);
        }
        return View(model);
    }

    // 4.3 DELETE ACCOUNT

    // POST: /Account/DeleteAccount
    [HttpPost]
    [Authorize]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> DeleteAccount()
    {
        var user = await _userManager.GetUserAsync(User);
        if (user == null) return RedirectToAction("Login");

        if (await _userManager.IsInRoleAsync(user, "Admin"))
            return RedirectToAction("Index");

        await _signInManager.SignOutAsync();
        await _userManager.DeleteAsync(user);

        return RedirectToAction("Index", "Home");
    }
}