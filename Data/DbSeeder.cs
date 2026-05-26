using GiftStoreMVC.Data;
using GiftStoreMVC.Models;
using Microsoft.EntityFrameworkCore;

namespace GiftStoreMVC.Data
{
    public static class DbSeeder
    {
        public static async Task Seed(ApplicationDbContext context)
        {
            if (!context.Categories.Any())
            {
                var categories = new List<Category>
                {
                    new Category { Name = "Birthday Gifts", Description = "Perfect for birthdays", ImageUrl = "https://images.unsplash.com/photo-1513151233558-d860c5398176?w=400" },
                    new Category { Name = "Anniversary", Description = "Celebrate love", ImageUrl = "https://images.unsplash.com/photo-1522673607200-164848374c08?w=400" },
                    new Category { Name = "Personalized", Description = "Made just for you", ImageUrl = "https://images.unsplash.com/photo-1549465220-1d8c9d9c6703?w=400" },
                    new Category { Name = "Home Decor", Description = "Beautify your living space", ImageUrl = "https://images.unsplash.com/photo-1513694203232-719a280e022f?w=400" },
                    new Category { Name = "Corporate Gifts", Description = "Professional gifting made easy", ImageUrl = "https://images.unsplash.com/photo-1521790797524-b2497295b8a0?w=400" }
                };
                context.Categories.AddRange(categories);
                await context.SaveChangesAsync();

                if (!context.Products.Any())
                {
                    var catBirthday = categories[0].Id;
                    var catAnniversary = categories[1].Id;
                    var catPersonalized = categories[2].Id;
                    var catHome = categories[3].Id;
                    var catCorporate = categories[4].Id;

                    var products = new List<Product>
                    {
                        new Product { Name = "Magic Mug", Description = "Changes color with hot water to reveal your hidden photo.", Price = 15.99m, StockQuantity = 50, CategoryId = catPersonalized, IsFeatured = true, CoverImageUrl = "https://images.unsplash.com/photo-1514432324607-a09d9b4aefdd?w=600", CreatedAt = DateTime.Now },
                        new Product { Name = "Classic Wooden Frame", Description = "Premium oak wood photo frame for your best memories.", Price = 25.00m, StockQuantity = 30, CategoryId = catPersonalized, IsFeatured = false, CoverImageUrl = "https://images.unsplash.com/photo-1583847268964-b28dc8f51f92?w=600", CreatedAt = DateTime.Now },
                        new Product { Name = "Lavender & Vanilla Candles", Description = "Hand-poured soy wax candles with a calming aroma.", Price = 12.50m, StockQuantity = 100, CategoryId = catHome, IsFeatured = true, CoverImageUrl = "https://images.unsplash.com/photo-1603006905003-be475563bc59?w=600", CreatedAt = DateTime.Now },
                        new Product { Name = "Premium Chronograph Watch", Description = "Elegant men's watch with leather strap and rose gold detailing.", Price = 150.00m, StockQuantity = 15, CategoryId = catAnniversary, IsFeatured = true, CoverImageUrl = "https://images.unsplash.com/photo-1524805444758-089113d48a6d?w=600", CreatedAt = DateTime.Now },
                        new Product { Name = "Genuine Leather Wallet", Description = "Minimalist RFID blocking wallet made of premium leather.", Price = 45.00m, StockQuantity = 40, CategoryId = catBirthday, IsFeatured = false, CoverImageUrl = "https://images.unsplash.com/photo-1627123424574-724758594e93?w=600", CreatedAt = DateTime.Now },
                        new Product { Name = "Minimalist Desk Organizer", Description = "Keep your workspace tidy with this elegant bamboo organizer.", Price = 35.00m, StockQuantity = 25, CategoryId = catCorporate, IsFeatured = true, CoverImageUrl = "https://images.unsplash.com/photo-1512314889357-e157c22f938d?w=600", CreatedAt = DateTime.Now },
                        new Product { Name = "Diamond Cut Crystal Vase", Description = "Luxurious crystal vase, perfect for fresh flower arrangements.", Price = 80.00m, StockQuantity = 20, CategoryId = catHome, IsFeatured = false, CoverImageUrl = "https://images.unsplash.com/photo-1581783898377-1c85bf937427?w=600", CreatedAt = DateTime.Now },
                        new Product { Name = "Engraved Executive Pen", Description = "Customizable metal pen in a premium gift box.", Price = 20.00m, StockQuantity = 60, CategoryId = catPersonalized, IsFeatured = true, CoverImageUrl = "https://images.unsplash.com/photo-1585336261022-680e295ce3fe?w=600", CreatedAt = DateTime.Now },
                        new Product { Name = "Italian Silk Tie", Description = "Handcrafted 100% silk tie with a modern subtle pattern.", Price = 55.00m, StockQuantity = 35, CategoryId = catBirthday, IsFeatured = false, CoverImageUrl = "https://images.unsplash.com/photo-1598532213005-59ac048d0ab4?w=600", CreatedAt = DateTime.Now },
                        new Product { Name = "Luxury Spa Gift Basket", Description = "Rejuvenating bath salts, oils, and lotions for a perfect spa day.", Price = 95.00m, StockQuantity = 10, CategoryId = catAnniversary, IsFeatured = true, CoverImageUrl = "https://images.unsplash.com/photo-1540555700478-4be289fbecef?w=600", CreatedAt = DateTime.Now },
                        new Product { Name = "Geometric Succulent Pot", Description = "Modern ceramic planter for small indoor plants.", Price = 18.00m, StockQuantity = 80, CategoryId = catHome, IsFeatured = false, CoverImageUrl = "https://images.unsplash.com/photo-1485955900006-10f4d324d411?w=600", CreatedAt = DateTime.Now },
                        new Product { Name = "Leather Bound Notebook", Description = "High-quality journal for the modern executive.", Price = 28.00m, StockQuantity = 45, CategoryId = catCorporate, IsFeatured = true, CoverImageUrl = "https://images.unsplash.com/photo-1531346878377-a541e4ab04ce?w=600", CreatedAt = DateTime.Now },
                        new Product { Name = "Silver Heart Pendant", Description = "Sterling silver necklace representing eternal love.", Price = 120.00m, StockQuantity = 12, CategoryId = catAnniversary, IsFeatured = true, CoverImageUrl = "https://images.unsplash.com/photo-1599643477874-5c866f571344?w=600", CreatedAt = DateTime.Now },
                        new Product { Name = "Portable Bluetooth Speaker", Description = "Waterproof speaker with 360-degree high fidelity sound.", Price = 65.00m, StockQuantity = 30, CategoryId = catBirthday, IsFeatured = true, CoverImageUrl = "https://images.unsplash.com/photo-1608043152269-423dbba4e7e1?w=600", CreatedAt = DateTime.Now },
                        new Product { Name = "Reusable Bamboo Cutlery", Description = "Eco-friendly dining set perfect for travel and office.", Price = 15.00m, StockQuantity = 100, CategoryId = catHome, IsFeatured = false, CoverImageUrl = "https://images.unsplash.com/photo-1584346133934-a3afd2a33c4c?w=600", CreatedAt = DateTime.Now },
                        new Product { Name = "Artisan Coffee Brew Kit", Description = "Complete pour-over coffee set for the caffeine enthusiast.", Price = 85.00m, StockQuantity = 18, CategoryId = catCorporate, IsFeatured = true, CoverImageUrl = "https://images.unsplash.com/photo-1495474472201-419b4fcb51be?w=600", CreatedAt = DateTime.Now },
                        new Product { Name = "Custom Photo Cushion", Description = "Soft decorative pillow printed with your favorite memory.", Price = 22.00m, StockQuantity = 50, CategoryId = catPersonalized, IsFeatured = false, CoverImageUrl = "https://images.unsplash.com/photo-1584100936595-c0654b55a2e2?w=600", CreatedAt = DateTime.Now },
                        new Product { Name = "Artisan Chocolate Truffles", Description = "Box of 24 handcrafted Belgian chocolate truffles.", Price = 40.00m, StockQuantity = 25, CategoryId = catAnniversary, IsFeatured = true, CoverImageUrl = "https://images.unsplash.com/photo-1548907040-4baa42d10919?w=600", CreatedAt = DateTime.Now },
                        new Product { Name = "Smart Fitness Tracker", Description = "Track steps, heart rate, and sleep with this sleek band.", Price = 50.00m, StockQuantity = 40, CategoryId = catBirthday, IsFeatured = false, CoverImageUrl = "https://images.unsplash.com/photo-1575311373937-040b8e1fd5b0?w=600", CreatedAt = DateTime.Now },
                        new Product { Name = "Architect LED Desk Lamp", Description = "Modern adjustable lamp with multiple color temperatures.", Price = 75.00m, StockQuantity = 22, CategoryId = catHome, IsFeatured = true, CoverImageUrl = "https://images.unsplash.com/photo-1507473885765-e6ed057f782c?w=600", CreatedAt = DateTime.Now }
                    };
                    context.Products.AddRange(products);
                    await context.SaveChangesAsync();
                }
            }

            if (!context.Users.Any(u => u.Email == "admin@gmail.com"))
            {
                var admin = new User
                {
                    FullName = "System Admin",
                    Email = "admin@gmail.com",
                    Password = BCrypt.Net.BCrypt.HashPassword("admin123"),
                    Role = UserRole.Admin,
                    IsActive = true,
                    Address = "Admin Office, Presento HQ",
                    PhoneNumber = "1234567890",
                    CreatedAt = DateTime.Now
                };
                context.Users.Add(admin);
                await context.SaveChangesAsync();
            }
        }
    }
}
