using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using TeaShop.Data;
using TeaShop.Models;
using TeaShop.ViewModels;

namespace TeaShop.Controllers;

// In MVC, the Controller handles the request and query, then hands data to a View.
public class HomeController : Controller
{
    private readonly TeaShopContext _context;

    // Constructor injection - the framework gives us the DbContext automatically.
    // This is called "Dependency Injection" (DI) and the framework manages the connection.
    public HomeController(TeaShopContext context)
    {
        _context = context;
    }

    // GET: / or /Home or /Home/Index
    // ASP.NET MVC automatically maps query string parameters from the URL to method parameters (Model Binding).
    public IActionResult Index(string? country, string? caffeine, string? search, int? categoryId)
    {
        // Start with all products (LINQ).
        // IQueryable means the query isn't executed yet - it builds up and only hits the DB when .ToList() is called at the end.
        IQueryable<Product> query = _context.Products.Include(p => p.Category);

        // Apply filters.
        if (!string.IsNullOrEmpty(country))
        {
            query = query.Where(p => p.OriginCountry == country);
        }
        
        if (categoryId.HasValue)
        {
            query = query.Where(p => p.CategoryId == categoryId.Value);
        }

        if (!string.IsNullOrEmpty(caffeine))
        {
            // Parse the string "Caffeinated" or "CaffeineFree" back to the enum.
            if (Enum.TryParse<CaffeineType>(caffeine, out var caffeineType))
            {
                query = query.Where(p => p.Caffeine == caffeineType);
            }
        }

        if (!string.IsNullOrEmpty(search))
        {
            string searchLower = search.ToLower();
            query = query.Where(p =>
                p.Name.ToLower().Contains(searchLower) ||
                (p.Description != null && p.Description.ToLower().Contains(searchLower)));
        }

        // Get the list of unique countries for the filter dropdown.
        var countries = _context.Products
            .Select(p => p.OriginCountry)
            .Distinct()
            .OrderBy(c => c)
            .ToList();

        // Build the ViewModel and pass it to the View.
        var viewModel = new ProductCatalogViewModel
        {
            Products = query.OrderBy(p => p.Name).ToList(),
            AvailableCountries = countries,
            AvailableCategories = _context.Categories.OrderBy(c => c.Name).ToList(),
            SelectedCountry = country,
            SelectedCaffeine = caffeine,
            SelectedCategoryId = categoryId,
            SearchQuery = search
        };

        return View(viewModel);
    }
}