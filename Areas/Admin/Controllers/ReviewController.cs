using Microsoft.AspNetCore.Mvc;
using GiftStoreMVC.Data;
using GiftStoreMVC.Models;
using GiftStoreMVC.Filters;
using Microsoft.EntityFrameworkCore;

namespace GiftStoreMVC.Areas.Admin.Controllers
{
    [Area("Admin")]
    [AdminOnly]
    public class ReviewController : Controller
    {
        private readonly ApplicationDbContext _context;

        public ReviewController(ApplicationDbContext context)
        {
            _context = context;
        }

        public async Task<IActionResult> Index(int? categoryId, int? productId)
        {
            var query = _context.Reviews
                .Include(r => r.Product)
                .Include(r => r.User)
                .AsQueryable();

            if (categoryId.HasValue)
            {
                query = query.Where(r => r.Product.CategoryId == categoryId.Value);
            }

            if (productId.HasValue)
            {
                query = query.Where(r => r.ProductId == productId.Value);
            }

            var reviews = await query.OrderByDescending(r => r.CreatedAt).ToListAsync();

            ViewBag.Categories = await _context.Categories.ToListAsync();
            
            var productsQuery = _context.Products.AsQueryable();
            if (categoryId.HasValue)
            {
                productsQuery = productsQuery.Where(p => p.CategoryId == categoryId.Value);
            }
            ViewBag.Products = await productsQuery.ToListAsync();
            
            ViewBag.SelectedCategoryId = categoryId;
            ViewBag.SelectedProductId = productId;

            return View(reviews);
        }

        [HttpPost]
        public async Task<IActionResult> Approve(int id)
        {
            var review = await _context.Reviews.FindAsync(id);
            if (review != null)
            {
                review.IsApproved = true;
                _context.Update(review);
                await _context.SaveChangesAsync();
                return Json(new { success = true });
            }
            return Json(new { success = false });
        }

        [HttpPost]
        public async Task<IActionResult> Delete(int id)
        {
            var review = await _context.Reviews.FindAsync(id);
            if (review != null)
            {
                _context.Reviews.Remove(review);
                await _context.SaveChangesAsync();
                return Json(new { success = true });
            }
            return Json(new { success = false });
        }
    }
}
