using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using TeaShop.Data;
using TeaShop.Helpers;
using TeaShop.Models;
using TeaShop.ViewModels;

namespace TeaShop.Controllers;

// [Authorize] on the whole controller means every action here requires login.
// If a guest tries to access /Order/Checkout, they get redirected to /Account/Login.
[Authorize]
public class OrderController : Controller
{
    private readonly TeaShopContext _context;
    private readonly UserManager<ApplicationUser> _userManager;
    private const string CartSessionKey = "Cart";

    public OrderController(TeaShopContext context, UserManager<ApplicationUser> userManager)
    {
        _context = context;
        _userManager = userManager;
    }

    private List<CartItem> GetCart()
    {
        return HttpContext.Session.Get<List<CartItem>>(CartSessionKey) ?? new List<CartItem>();
    }

    private void ClearCart()
    {
        HttpContext.Session.Remove(CartSessionKey);
    }

    // GET: /Order/Checkout
    // Shows the checkout form with cart summary and shipping fields.
    public IActionResult Checkout()
    {
        var cart = GetCart();

        if (!cart.Any())
            return RedirectToAction("Index", "Cart");

        var viewModel = new CheckoutViewModel
        {
            CartItems = cart
        };

        return View(viewModel);
    }

    // POST: /Order/Checkout
    // Processes the order: validates, creates Order + OrderItems in DB, clears cart.
    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Checkout(CheckoutViewModel model)
    {
        var cart = GetCart();

        if (!cart.Any())
            return RedirectToAction("Index", "Cart");

        // Re-populate cart items (form post doesn't include them).
        model.CartItems = cart;

        if (!ModelState.IsValid)
            return View(model);

        // Get the logged-in user.
        var user = await _userManager.GetUserAsync(User);
        if (user == null)
            return RedirectToAction("Login", "Account");

        // Create the order.
        // Here EF handles the SQL.
        var order = new Order
        {
            UserId = user.Id,
            Total = cart.Sum(i => i.Subtotal),
            Status = model.PaymentMethod == "PayNow" ? OrderStatus.Paid : OrderStatus.AwaitingPayment,
            ShippingAddress = model.ShippingAddress,
            ShippingCity = model.ShippingCity,
            ShippingPostalCode = model.ShippingPostalCode,
            StatusUpdatedAt = DateTime.UtcNow
        };

        // Add order items.
        foreach (var item in cart)
        {
            order.OrderItems.Add(new OrderItem
            {
                ProductId = item.ProductId,
                Quantity = item.Quantity,
                Price = item.Price
            });
        }

        _context.Orders.Add(order);
        
        // Decrease stock for each ordered product.
        foreach (var item in cart)
        {
            var product = await _context.Products.FindAsync(item.ProductId);
            if (product != null)
            {
                product.Stock = Math.Max(0, product.Stock - item.Quantity);
            }
        }
        
        await _context.SaveChangesAsync();

        // Clear the cart after successful order.
        ClearCart();

        // Redirect to confirmation page.
        return RedirectToAction("Confirmation", new { id = order.Id });
    }

    // POST: /Order/PayNow/orderID
    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> PayNow(int id)
    {
        var user = await _userManager.GetUserAsync(User);
        if (user == null)
            return RedirectToAction("Login", "Account");

        var order = await _context.Orders
            .FirstOrDefaultAsync(o => o.Id == id && o.UserId == user.Id && o.Status == OrderStatus.AwaitingPayment);

        if (order == null)
            return NotFound();

        order.Status = OrderStatus.Paid;
        order.StatusUpdatedAt = DateTime.UtcNow;
        await _context.SaveChangesAsync();

        return RedirectToAction("History");
    }

    // GET: /Order/Confirmation/orderID
    // Shows "thank you" page after placing an order.
    public async Task<IActionResult> Confirmation(int id)
    {
        var user = await _userManager.GetUserAsync(User);
        if (user == null)
            return RedirectToAction("Login", "Account");

        var order = await _context.Orders
            .Include(o => o.OrderItems)
            .ThenInclude(oi => oi.Product)
            .FirstOrDefaultAsync(o => o.Id == id && o.UserId == user.Id);

        if (order == null)
            return NotFound();

        return View(order);
    }

    // GET: /Order/History
    // Shows all past orders for the logged-in user.
    public async Task<IActionResult> History()
    {
        var user = await _userManager.GetUserAsync(User);
        if (user == null)
            return RedirectToAction("Login", "Account");

        // LINQ query: get all orders for this user, newest first, with their items.
        var orders = await _context.Orders
            .Where(o => o.UserId == user.Id)
            .Include(o => o.OrderItems)
            .ThenInclude(oi => oi.Product)
            .OrderByDescending(o => o.CreatedAt)
            .ToListAsync();

        return View(orders);
    }
}