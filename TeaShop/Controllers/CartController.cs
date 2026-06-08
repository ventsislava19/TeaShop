using Microsoft.AspNetCore.Mvc;
using TeaShop.Data;
using TeaShop.Helpers;
using TeaShop.Models;

namespace TeaShop.Controllers;

// The session is stored in the ASP.NET session as a JSON-serialized List<CartItem>.
public class CartController : Controller
{
    private readonly TeaShopContext _context;
    private const string CartSessionKey = "Cart";

    public CartController(TeaShopContext context)
    {
        _context = context;
    }

    // Helper: get the cart from session (or empty list if none exists).
    private List<CartItem> GetCart()
    {
        return HttpContext.Session.Get<List<CartItem>>(CartSessionKey) ?? new List<CartItem>();
    }

    // Helper: save the cart back to session.
    private void SaveCart(List<CartItem> cart)
    {
        HttpContext.Session.Set(CartSessionKey, cart);
    }

    // GET: /Cart
    public IActionResult Index()
    {
        var cart = GetCart();
        return View(cart);
    }

    // AJAX call: /Cart/Add
    // Called when user clicks "Add to Cart" on a product card.
    [HttpPost]
    [ValidateAntiForgeryToken]
    public IActionResult Add(int productId)
    {
        var cart = GetCart();
        var existingItem = cart.FirstOrDefault(i => i.ProductId == productId);

        if (existingItem != null)
        {
            var product = _context.Products.Find(productId);
            if (product != null && existingItem.Quantity < product.Stock)
            {
                existingItem.Quantity++;
            }
        }
        else
        {
            var product = _context.Products.Find(productId);
            if (product == null)
                return NotFound();
            
            if (product.Stock <= 0)
            {
                if (Request.Headers["X-Requested-With"] == "XMLHttpRequest")
                    return Json(new { success = false, message = "Out of stock" });
                return RedirectToAction("Index", "Home");
            }

            cart.Add(new CartItem
            {
                ProductId = product.Id,
                Name = product.Name,
                Price = product.Price,
                Image = product.Image,
                Quantity = 1
            });
        }

        SaveCart(cart);

        // If it's an AJAX request, return JSON with the new cart count.
        if (Request.Headers["X-Requested-With"] == "XMLHttpRequest")
        {
            var totalCount = cart.Sum(i => i.Quantity);
            return Json(new { success = true, cartCount = totalCount });
        }

        // Fallback for non-JS browsers.
        var referer = Request.Headers["Referer"].ToString();
        if (!string.IsNullOrEmpty(referer))
            return Redirect(referer);
        return RedirectToAction("Index", "Home");
    }

    // POST: /Cart/Update
    // Called when user changes quantity in the cart view.
    [HttpPost]
    [ValidateAntiForgeryToken]
    public IActionResult Update(int productId, int quantity)
    {
        var cart = GetCart();
        var item = cart.FirstOrDefault(i => i.ProductId == productId);

        if (item != null)
        {
            if (quantity <= 0)
            {
                // Quantity 0 or negative - remove from cart.
                cart.Remove(item);
            }
            else
            {
                item.Quantity = quantity;
            }
        }

        SaveCart(cart);
        return RedirectToAction("Index");
    }

    // POST: /Cart/Remove
    // Called when user clicks the remove button.
    [HttpPost]
    [ValidateAntiForgeryToken]
    public IActionResult Remove(int productId)
    {
        var cart = GetCart();
        cart.RemoveAll(i => i.ProductId == productId);
        SaveCart(cart);
        return RedirectToAction("Index");
    }
}