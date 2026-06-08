using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using TeaShop.Data;
using TeaShop.Models;

var builder = WebApplication.CreateBuilder(args);

// 1. DATABASE
// Register TeaShopContext with EF Core, using SQL Server.
// This reads the "DefaultConnection" string from appsettings.json and the framework handles the connection.
builder.Services.AddDbContext<TeaShopContext>(options =>
    options.UseSqlServer(builder.Configuration.GetConnectionString("DefaultConnection")));

// 2. IDENTITY (Authentication)
// ASP.NET Identity handles:
// - Password hashing
// - Login/logout cookies
// - User management (registration, email uniqueness...)
builder.Services.AddIdentity<ApplicationUser, IdentityRole>(options =>
    {
        // Password rules.
        options.Password.RequireDigit = false;
        options.Password.RequireUppercase = false;
        options.Password.RequireNonAlphanumeric = false;
        options.Password.RequiredLength = 6;

        // Require unique email.
        options.User.RequireUniqueEmail = true;
    })
    .AddEntityFrameworkStores<TeaShopContext>()
    .AddDefaultTokenProviders();

// Configure the login/logout redirect paths.
// When a user tries to access a page that requires [Authorize],
// they get redirected to /Account/Login instead of getting a 403 error.
builder.Services.ConfigureApplicationCookie(options =>
{
    options.LoginPath = "/Account/Login";
    options.LogoutPath = "/Account/Logout";
    options.AccessDeniedPath = "/Account/Login";
});

// 3. SESSION (for the shopping cart)
// ASP.NET explicitly enabled session.
builder.Services.AddDistributedMemoryCache();
builder.Services.AddSession(options =>
{
    options.IdleTimeout = TimeSpan.FromMinutes(30);
    options.Cookie.HttpOnly = true;
    options.Cookie.IsEssential = true;
});

// 4. MVC
// Registers controllers + views.
builder.Services.AddControllersWithViews();

var app = builder.Build();

// MIDDLEWARE PIPELINE
// Оrder matters here - each request flows through these in sequence.
if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Home/Error");
    app.UseHsts();
}

app.UseHttpsRedirection();

// Serves static files from wwwroot/ (CSS, JS, images) like an assets folder.
app.UseStaticFiles();

app.UseRouting();

// Session must come before auth (cart needs to be loaded before checking login).
app.UseSession();

app.UseAuthentication();
app.UseAuthorization();

// Default route: {controller=Home}/{action=Index}/{id?}
// So visiting "/" goes to HomeController.Index(),
//    visiting "/Cart" goes to CartController.Index()...
// The routing system.
app.MapControllerRoute(
    name: "default",
    pattern: "{controller=Home}/{action=Index}/{id?}");

// The seed call is async.
using (var scope = app.Services.CreateScope())
{
    var services = scope.ServiceProvider;
    var context = services.GetRequiredService<TeaShopContext>();
    var userManager = services.GetRequiredService<UserManager<ApplicationUser>>();
    var roleManager = services.GetRequiredService<RoleManager<IdentityRole>>();
    await SeedData.InitializeAsync(context, userManager, roleManager);
}

app.Run();