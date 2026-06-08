using Microsoft.AspNetCore.Identity;
using TeaShop.Models;

namespace TeaShop.Data;

public static class SeedData
{
    public static async Task InitializeAsync(
        TeaShopContext context,
        UserManager<ApplicationUser> userManager,
        RoleManager<IdentityRole> roleManager)
    {
        // Create Admin role.
        if (!await roleManager.RoleExistsAsync("Admin"))
        {
            await roleManager.CreateAsync(new IdentityRole("Admin"));
        }

        // Create Admin user.
        const string adminEmail = "admin@teashop.com";
        const string adminPassword = "admin@teashop.com";

        var adminUser = await userManager.FindByEmailAsync(adminEmail);
        if (adminUser == null)
        {
            adminUser = new ApplicationUser
            {
                Name = "Administrator",
                UserName = adminEmail,
                Email = adminEmail,
                EmailConfirmed = true
            };

            var result = await userManager.CreateAsync(adminUser, adminPassword);
            if (result.Succeeded)
            {
                await userManager.AddToRoleAsync(adminUser, "Admin");
            }
        }

        // Seed products.
        if (context.Products.Any())
            return;

        var teas = new Category
        {
            Name = "Teas",
            Description = "Premium loose leaf teas from around the world."
        };
        context.Categories.Add(teas);
        await context.SaveChangesAsync();

        var products = new List<Product>
        {
            new Product { Name = "Pai Mu Tan", Description = "Delicate white tea with soft floral and honey notes.", Price = 8.99m, Image = "Pai_Mu_Tan.jpg", CategoryId = teas.Id, OriginCountry = "China", Caffeine = CaffeineType.Caffeinated },
            new Product { Name = "Souchong", Description = "Smoky black tea traditionally dried over pinewood fire.", Price = 7.99m, Image = "Lapsang_Souchong.jpg", CategoryId = teas.Id, OriginCountry = "China", Caffeine = CaffeineType.Caffeinated },
            new Product { Name = "Ming Feng Maocha", Description = "Aromatic raw leaf tea with a clean, lasting finish.", Price = 10.99m, Image = "Ming_Feng_Gushu_Maocha.jpg", CategoryId = teas.Id, OriginCountry = "China", Caffeine = CaffeineType.Caffeinated },
            new Product { Name = "Milky Oolong", Description = "Creamy oolong with buttery texture and sweetness.", Price = 9.99m, Image = "Milky_Oolong.jpg", CategoryId = teas.Id, OriginCountry = "China", Caffeine = CaffeineType.Caffeinated },
            new Product { Name = "Anxi Tie Guan Yin", Description = "Classic oolong with orchid aroma and bright, silky body.", Price = 9.49m, Image = "Anxi_Tie_Guan_Yin.jpeg", CategoryId = teas.Id, OriginCountry = "China", Caffeine = CaffeineType.CaffeineFree },
            new Product { Name = "Mao Jian", Description = "Fresh green tea with grassy notes and a crisp finish.", Price = 8.49m, Image = "Mao_Jian.jpg", CategoryId = teas.Id, OriginCountry = "China", Caffeine = CaffeineType.CaffeineFree },
            new Product { Name = "Ming Feng Gong Ting", Description = "Rich, earthy tea with deep body and warm sweetness.", Price = 11.49m, Image = "Ming_Feng_Gushu_Gong_Ting.jpeg", CategoryId = teas.Id, OriginCountry = "China", Caffeine = CaffeineType.Caffeinated },
            new Product { Name = "Gunpowder", Description = "Rolled green tea with bold character and a smoky edge.", Price = 7.49m, Image = "Gunpowder.jpg", CategoryId = teas.Id, OriginCountry = "China", Caffeine = CaffeineType.CaffeineFree },
            new Product { Name = "Hojicha", Description = "Roasted green tea with nutty aroma and low bitterness.", Price = 7.49m, Image = "Hojicha.jpeg", CategoryId = teas.Id, OriginCountry = "Japan", Caffeine = CaffeineType.Caffeinated },
            new Product { Name = "Genmaicha", Description = "Green tea blended with roasted rice for a toasty flavor.", Price = 7.99m, Image = "Genmaicha.jpg", CategoryId = teas.Id, OriginCountry = "Japan", Caffeine = CaffeineType.Caffeinated },
            new Product { Name = "Kukicha", Description = "Stem tea with mild sweetness and vegetal notes.", Price = 7.29m, Image = "Kukicha.jpg", CategoryId = teas.Id, OriginCountry = "Japan", Caffeine = CaffeineType.Caffeinated },
            new Product { Name = "Sayamakaori", Description = "Japanese green tea cultivar with rich umami aroma.", Price = 8.99m, Image = "Japan_Sayamakaori.jpeg", CategoryId = teas.Id, OriginCountry = "Japan", Caffeine = CaffeineType.Caffeinated },
            new Product { Name = "Bancha", Description = "Everyday green tea with light body and grassy notes.", Price = 6.99m, Image = "Bancha.jpg", CategoryId = teas.Id, OriginCountry = "Japan", Caffeine = CaffeineType.Caffeinated },
            new Product { Name = "Sencha Fukamushi", Description = "Deep-steamed sencha with umami and green liquor.", Price = 9.49m, Image = "Sencha_Fukamushi.jpeg", CategoryId = teas.Id, OriginCountry = "Japan", Caffeine = CaffeineType.CaffeineFree },
            new Product { Name = "Sencha Yabukita", Description = "Classic Japanese sencha with balanced sweetness.", Price = 8.99m, Image = "Sencha_Yabukita.jpeg", CategoryId = teas.Id, OriginCountry = "Japan", Caffeine = CaffeineType.CaffeineFree },
            new Product { Name = "Sencha Asamushi", Description = "Light-steamed sencha with clear taste and infusion.", Price = 8.79m, Image = "Sencha_Asamushi.jpeg", CategoryId = teas.Id, OriginCountry = "Japan", Caffeine = CaffeineType.Caffeinated },
            new Product { Name = "Gyokuro Asahina", Description = "Premium green tea with intense umami texture.", Price = 14.99m, Image = "Gyokuro_Asahina.jpeg", CategoryId = teas.Id, OriginCountry = "Japan", Caffeine = CaffeineType.Caffeinated },
            new Product { Name = "Darjeeling First Flush", Description = "Floral black tea with muscatel notes from spring harvest.", Price = 9.99m, Image = "Darjeeling_First_Flush.jpg", CategoryId = teas.Id, OriginCountry = "India", Caffeine = CaffeineType.Caffeinated },
            new Product { Name = "Darjeeling Second Flush", Description = "Darjeeling with muscatel character and strong aroma.", Price = 10.49m, Image = "Darjeeling_Second_Flush.jpg", CategoryId = teas.Id, OriginCountry = "India", Caffeine = CaffeineType.CaffeineFree },
            new Product { Name = "Assam Wild Forest", Description = "Bold, malty tea harvested of wild-growing tea.", Price = 8.99m, Image = "Assam_Wild_Forest.jpeg", CategoryId = teas.Id, OriginCountry = "India", Caffeine = CaffeineType.Caffeinated },
            new Product { Name = "Earl Grey", Description = "Classic black tea infused with natural bergamot oil.", Price = 7.99m, Image = "Earl_Grey.jpg", CategoryId = teas.Id, OriginCountry = "India", Caffeine = CaffeineType.CaffeineFree },
            new Product { Name = "Assam Tonganagaon", Description = "Strong Assam tea with deep malty notes and finish.", Price = 8.49m, Image = "Assam_Tonganagaon.jpeg", CategoryId = teas.Id, OriginCountry = "India", Caffeine = CaffeineType.Caffeinated },
            new Product { Name = "Assam Wild Oolong", Description = "Oolong-style tea with complex and gentle oxidation.", Price = 9.49m, Image = "Assam_Wild_Oolong.jpeg", CategoryId = teas.Id, OriginCountry = "India", Caffeine = CaffeineType.CaffeineFree },
            new Product { Name = "Sakhejung Golden", Description = "Hand-rolled tea with sweetness and spice.", Price = 10.99m, Image = "Nepal_Sakhejung_Golden_Tea.jpeg", CategoryId = teas.Id, OriginCountry = "Nepal", Caffeine = CaffeineType.Caffeinated },
            new Product { Name = "Yuchi Red Jade", Description = "Ruby black tea with minty spice and natural sweetness.", Price = 11.99m, Image = "Yuchi_Red_Jade.jpeg", CategoryId = teas.Id, OriginCountry = "Taiwan", Caffeine = CaffeineType.CaffeineFree },
            new Product { Name = "Jin Xuan Milky Oolong", Description = "Naturally creamy oolong with soft dairy notes.", Price = 10.49m, Image = "Taiwan_Jin_Xuan_Milky_Oolong.jpeg", CategoryId = teas.Id, OriginCountry = "Taiwan", Caffeine = CaffeineType.Caffeinated },
            new Product { Name = "Oolong Qing Xin", Description = "Fragrant high-altitude oolong with floral complexity.", Price = 13.99m, Image = "Taiwan_High_Mountain_Oolоng_Qing_Xin.jpeg", CategoryId = teas.Id, OriginCountry = "Taiwan", Caffeine = CaffeineType.CaffeineFree },
            new Product { Name = "Gui Fei Oolong", Description = "Honeyed oolong naturally bitten by leafhoppers.", Price = 12.99m, Image = "Gui_Fei_Oolong.jpeg", CategoryId = teas.Id, OriginCountry = "Taiwan", Caffeine = CaffeineType.Caffeinated },
            new Product { Name = "Oregano", Description = "Wild oregano with antibacterial properties.", Price = 4.99m, Image = "Oregano.jpeg", CategoryId = teas.Id, OriginCountry = "Bulgaria", Caffeine = CaffeineType.CaffeineFree },
            new Product { Name = "La Vie En Rose", Description = "Delicate rose flowers with calming effect.", Price = 6.49m, Image = "La_Vie_En_Rose.jpeg", CategoryId = teas.Id, OriginCountry = "Bulgaria", Caffeine = CaffeineType.CaffeineFree },
            new Product { Name = "Mursal Tea", Description = "Rare mountain herb used for immunity and vitality.", Price = 7.99m, Image = "Mural.jpg", CategoryId = teas.Id, OriginCountry = "Bulgaria", Caffeine = CaffeineType.CaffeineFree },
            new Product { Name = "Linden Blossom", Description = "Soft, honey-like herbal tea made from linden.", Price = 5.49m, Image = "Linden.jpeg", CategoryId = teas.Id, OriginCountry = "Bulgaria", Caffeine = CaffeineType.CaffeineFree },
            new Product { Name = "Chamomile", Description = "Classic chamomile with soothing properties.", Price = 4.49m, Image = "Matricaria_Chamomilla.jpeg", CategoryId = teas.Id, OriginCountry = "Bulgaria", Caffeine = CaffeineType.CaffeineFree }
        };

        context.Products.AddRange(products);
        
        // Set default stock for all seeded products.
        foreach (var p in products)
        {
            p.Stock = 20;
        }
        
        await context.SaveChangesAsync();
    }
}