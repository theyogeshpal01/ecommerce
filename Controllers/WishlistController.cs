using Microsoft.AspNetCore.Mvc;
using GiftStoreMVC.Data;
using GiftStoreMVC.Models;
using Microsoft.EntityFrameworkCore;

namespace GiftStoreMVC.Controllers
{
    public class WishlistController : Controller
    {
        private readonly ApplicationDbContext _context;

        public WishlistController(ApplicationDbContext context)
        {
            _context = context;
        }

        private int? CurrentUserId => HttpContext.Session.GetInt32("UserId");

        public async Task<IActionResult> Index()
        {
            if (CurrentUserId == null) return RedirectToAction("Login", "Account");

            var wishlistItems = await _context.WishlistItems
                .Include(w => w.Product)
                .Where(w => w.UserId == CurrentUserId.Value)
                .ToListAsync();

            return View(wishlistItems);
        }

        public async Task<IActionResult> Add(int productId)
        {
            if (CurrentUserId == null)
            {
                TempData["Error"] = "Please login to add items to wishlist.";
                return RedirectToAction("Login", "Account");
            }

            var alreadyInWishlist = await _context.WishlistItems
                .AnyAsync(w => w.UserId == CurrentUserId.Value && w.ProductId == productId);

            if (!alreadyInWishlist)
            {
                var wishlistItem = new WishlistItem
                {
                    UserId = CurrentUserId.Value,
                    ProductId = productId
                };
                _context.WishlistItems.Add(wishlistItem);
                await _context.SaveChangesAsync();
                TempData["Success"] = "Item added to wishlist!";
            }
            else
            {
                TempData["Info"] = "Item is already in your wishlist.";
            }

            return RedirectToAction("Index");
        }

        public async Task<IActionResult> Remove(int id)
        {
            if (CurrentUserId == null) return RedirectToAction("Login", "Account");

            var item = await _context.WishlistItems
                .FirstOrDefaultAsync(w => w.Id == id && w.UserId == CurrentUserId.Value);

            if (item != null)
            {
                _context.WishlistItems.Remove(item);
                await _context.SaveChangesAsync();
                TempData["Success"] = "Item removed from wishlist.";
            }

            return RedirectToAction("Index");
        }
    }
}
