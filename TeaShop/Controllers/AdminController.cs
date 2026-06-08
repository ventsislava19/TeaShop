using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using TeaShop.Data;
using TeaShop.Models;
using TeaShop.ViewModels;

namespace TeaShop.Controllers;

// [Authorize(Roles = "Admin")] means only users in the "Admin" role can access and anyone else will be redirected to the login page.
[Authorize(Roles = "Admin")]
public class AdminController : Controller
{
    private readonly TeaShopContext _context;

    public AdminController(TeaShopContext context)
    {
        _context = context;
    }

    // 1. DASHBOARD

    // GET: /Admin
    public IActionResult Index()
    {
        ViewBag.ProductCount = _context.Products.Count();
        ViewBag.OrderCount = _context.Orders.Count();
        ViewBag.CategoryCount = _context.Categories.Count();
        return View();
    }

    // 2. PRODUCTS

    // GET: /Admin/Products
    public IActionResult Products()
    {
        var products = _context.Products
            .Include(p => p.Category)
            .OrderBy(p => p.Name)
            .ToList();
        return View(products);
    }

    // GET: /Admin/CreateProduct
    public IActionResult CreateProduct()
    {
        PopulateCategoryDropdown();
    
        // Default to "Teas" category.
        var teasCategory = _context.Categories.FirstOrDefault(c => c.Name == "Teas");
    
        return View(new ProductFormViewModel
        {
            CategoryId = teasCategory?.Id ?? 0,
            Caffeine = CaffeineType.NotApplicable
        });
    }

    // POST: /Admin/CreateProduct
    [HttpPost]
    [ValidateAntiForgeryToken]
    public IActionResult CreateProduct(ProductFormViewModel model)
    {
        if (!ModelState.IsValid)
        {
            PopulateCategoryDropdown(model.CategoryId);
            return View(model);
        }

        var product = new Product
        {
            Name = model.Name,
            Description = model.Description,
            Price = model.Price,
            Image = model.Image,
            CategoryId = model.CategoryId,
            OriginCountry = model.OriginCountry,
            Caffeine = model.Caffeine
        };

        _context.Products.Add(product);
        _context.SaveChanges();

        return RedirectToAction("Products");
    }

    // GET: /Admin/EditProduct/5
    public IActionResult EditProduct(int id)
    {
        var product = _context.Products.Find(id);
        if (product == null) return NotFound();

        var model = new ProductFormViewModel
        {
            Id = product.Id,
            Name = product.Name,
            Description = product.Description,
            Price = product.Price,
            Image = product.Image,
            CategoryId = product.CategoryId,
            OriginCountry = product.OriginCountry,
            Caffeine = product.Caffeine,
            Stock = product.Stock
        };

        PopulateCategoryDropdown(product.CategoryId);
        return View(model);
    }

    // POST: /Admin/EditProduct/5
    [HttpPost]
    [ValidateAntiForgeryToken]
    public IActionResult EditProduct(ProductFormViewModel model)
    {
        if (!ModelState.IsValid)
        {
            PopulateCategoryDropdown(model.CategoryId);
            return View(model);
        }

        var product = _context.Products.Find(model.Id);
        if (product == null) return NotFound();

        product.Name = model.Name;
        product.Description = model.Description;
        product.Price = model.Price;
        product.Image = model.Image;
        product.CategoryId = model.CategoryId;
        product.OriginCountry = model.OriginCountry;
        product.Caffeine = model.Caffeine;
        product.Stock = model.Stock;

        _context.SaveChanges();
        return RedirectToAction("Products");
    }

    // POST: /Admin/DeleteProduct/5
    [HttpPost]
    [ValidateAntiForgeryToken]
    public IActionResult DeleteProduct(int id)
    {
        var product = _context.Products.Find(id);
        if (product != null)
        {
            _context.Products.Remove(product);
            _context.SaveChanges();
        }
        return RedirectToAction("Products");
    }

    // 4. ORDERS

    // GET: /Admin/Orders
    public IActionResult Orders()
    {
        var orders = _context.Orders
            .Include(o => o.User)
            .Include(o => o.OrderItems)
            .ThenInclude(oi => oi.Product)
            .OrderByDescending(o => o.CreatedAt)
            .ToList();
        return View(orders);
    }

    // POST: /Admin/UpdateOrderStatus
    [HttpPost]
    [ValidateAntiForgeryToken]
    public IActionResult UpdateOrderStatus(int orderId, OrderStatus status)
    {
        var order = _context.Orders.Find(orderId);
        if (order != null)
        {
            order.Status = status;
            order.StatusUpdatedAt = DateTime.UtcNow;
            _context.SaveChanges();
        }
        return RedirectToAction("Orders");
    }

    // 5. CATEGORIES

    // GET: /Admin/Categories
    public IActionResult Categories()
    {
        var categories = _context.Categories
            .Include(c => c.Products)
            .OrderBy(c => c.Name)
            .ToList();
        return View(categories);
    }

    // GET: /Admin/CreateCategory
    public IActionResult CreateCategory()
    {
        return View(new Category());
    }

    // POST: /Admin/CreateCategory
    [HttpPost]
    [ValidateAntiForgeryToken]
    public IActionResult CreateCategory(Category category)
    {
        if (!ModelState.IsValid)
            return View(category);

        _context.Categories.Add(category);
        _context.SaveChanges();
        return RedirectToAction("Categories");
    }

    // GET: /Admin/EditCategory/1
    public IActionResult EditCategory(int id)
    {
        var category = _context.Categories.Find(id);
        if (category == null) return NotFound();
        return View(category);
    }

    // POST: /Admin/EditCategory/1
    [HttpPost]
    [ValidateAntiForgeryToken]
    public IActionResult EditCategory(Category category)
    {
        if (!ModelState.IsValid)
            return View(category);

        var existing = _context.Categories.Find(category.Id);
        if (existing == null) return NotFound();

        existing.Name = category.Name;
        existing.Description = category.Description;
        _context.SaveChanges();
        return RedirectToAction("Categories");
    }

    // POST: /Admin/DeleteCategory/1
    [HttpPost]
    [ValidateAntiForgeryToken]
    public IActionResult DeleteCategory(int id)
    {
        var category = _context.Categories.Find(id);
        if (category != null)
        {
            _context.Categories.Remove(category);
            _context.SaveChanges();
        }
        return RedirectToAction("Categories");
    }

    // Helper: populates ViewBag with category list for dropdowns.
    private void PopulateCategoryDropdown(int? selectedId = null)
    {
        ViewBag.Categories = new SelectList(
            _context.Categories.OrderBy(c => c.Name),
            "Id", "Name", selectedId);
    }
}