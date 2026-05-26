using Microsoft.AspNetCore.Mvc;
using GiftStoreMVC.Data;
using GiftStoreMVC.Models;
using Microsoft.EntityFrameworkCore;

namespace GiftStoreMVC.Controllers
{
    public class ProductController : Controller
    {
        private readonly ApplicationDbContext _context;

        public ProductController(ApplicationDbContext context)
        {
            _context = context;
        }

        public async Task<IActionResult> Index(int? category, string search, string sort)
        {
            var productsQuery = _context.Products.Include(p => p.Category).Include(p => p.Reviews.Where(r => r.IsApproved)).AsQueryable();

            // Filtering
            if (category.HasValue)
            {
                productsQuery = productsQuery.Where(p => p.CategoryId == category.Value);
            }

            if (!string.IsNullOrEmpty(search))
            {
                productsQuery = productsQuery.Where(p => p.Name.Contains(search) || p.Description.Contains(search));
            }

            // Sorting
            productsQuery = sort switch
            {
                "price_asc" => productsQuery.OrderBy(p => p.Price),
                "price_desc" => productsQuery.OrderByDescending(p => p.Price),
                "newest" => productsQuery.OrderByDescending(p => p.CreatedAt),
                _ => productsQuery.OrderByDescending(p => p.CreatedAt)
            };

            ViewBag.Categories = await _context.Categories.ToListAsync();
            ViewBag.CurrentCategory = category;
            ViewBag.CurrentSearch = search;
            ViewBag.CurrentSort = sort;

            return View(await productsQuery.ToListAsync());
        }

        public async Task<IActionResult> Details(int? id)
        {
            if (id == null) return NotFound();

            var product = await _context.Products
                .Include(p => p.Category)
                .Include(p => p.ProductImages)
                .Include(p => p.Reviews.Where(r => r.IsApproved))
                    .ThenInclude(r => r.User)
                .FirstOrDefaultAsync(m => m.Id == id);

            if (product == null) return NotFound();

            return View(product);
        }

        [HttpPost]
        public async Task<IActionResult> AddReview(int productId, int rating, string comment)
        {
            var userId = HttpContext.Session.GetInt32("UserId");
            if (userId == null)
            {
                TempData["Error"] = "You must be logged in to leave a review.";
                return RedirectToAction("Login", "Account");
            }

            if (rating < 1 || rating > 5 || string.IsNullOrWhiteSpace(comment))
            {
                TempData["Error"] = "Invalid review submitted.";
                return RedirectToAction(nameof(Details), new { id = productId });
            }

            var existingReview = await _context.Reviews
                .FirstOrDefaultAsync(r => r.ProductId == productId && r.UserId == userId.Value);

            if (existingReview != null)
            {
                TempData["Error"] = "You have already reviewed this product.";
                return RedirectToAction(nameof(Details), new { id = productId });
            }

            var review = new Review
            {
                ProductId = productId,
                UserId = userId.Value,
                Rating = rating,
                Comment = comment,
                CreatedAt = DateTime.Now,
                IsApproved = true // Auto-approve for demo
            };

            _context.Reviews.Add(review);
            await _context.SaveChangesAsync();

            TempData["Success"] = "Your review has been posted successfully!";
            return RedirectToAction(nameof(Details), new { id = productId });
        }
    }
}
