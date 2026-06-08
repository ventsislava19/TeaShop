using TeaShop.Models;

namespace TeaShop.ViewModels;

// A ViewModel is not a DB table - it's a "package" of data
// shaped specifically for what a view needs to display (the controller builds a ViewModel,
// passes it to the View, and the View renders it).

public class ProductCatalogViewModel
{
    public IEnumerable<Product> Products { get; set; } = Enumerable.Empty<Product>();

    // Available filter values (populated from the DB).
    public IEnumerable<string> AvailableCountries { get; set; } = Enumerable.Empty<string>();
    
    public IEnumerable<Category> AvailableCategories { get; set; } = Enumerable.Empty<Category>();

    // Currently selected filters (from the URL query string).
    public string? SelectedCountry { get; set; }
    
    public int? SelectedCategoryId { get; set; }
    public string? SelectedCaffeine { get; set; }
    public string? SearchQuery { get; set; }
}

