using Microsoft.AspNetCore.Mvc;
using GiftStoreMVC.Data;
using GiftStoreMVC.Models;
using Microsoft.EntityFrameworkCore;

namespace GiftStoreMVC.Controllers
{
    public class CartController : Controller
    {
        private readonly ApplicationDbContext _context;

        public CartController(ApplicationDbContext context)
        {
            _context = context;
        }

        private int? CurrentUserId => HttpContext.Session.GetInt32("UserId");

        public async Task<IActionResult> Index()
        {
            if (CurrentUserId == null) return RedirectToAction("Login", "Account");

            var cartItems = await _context.CartItems
                .Include(c => c.Product)
                .Where(c => c.UserId == CurrentUserId.Value)
                .ToListAsync();

            return View(cartItems);
        }

        public async Task<IActionResult> Add(int productId, int quantity = 1)
        {
            if (CurrentUserId == null)
            {
                TempData["Error"] = "Please login to add items to cart.";
                return RedirectToAction("Login", "Account");
            }

            var product = await _context.Products.FindAsync(productId);
            if (product == null) return NotFound();

            if (product.StockQuantity < quantity)
            {
                TempData["Error"] = "Requested quantity not available.";
                return RedirectToAction("Details", "Product", new { id = productId });
            }

            var cartItem = await _context.CartItems
                .FirstOrDefaultAsync(c => c.UserId == CurrentUserId.Value && c.ProductId == productId);

            if (cartItem != null)
            {
                cartItem.Quantity += quantity;
                _context.Update(cartItem);
            }
            else
            {
                cartItem = new CartItem
                {
                    UserId = CurrentUserId.Value,
                    ProductId = productId,
                    Quantity = quantity
                };
                _context.CartItems.Add(cartItem);
            }

            await _context.SaveChangesAsync();
            TempData["Success"] = "Item added to cart!";
            return RedirectToAction("Index");
        }

        public async Task<IActionResult> UpdateQuantity(int cartItemId, int quantity)
        {
            if (CurrentUserId == null) return RedirectToAction("Login", "Account");

            var cartItem = await _context.CartItems
                .Include(c => c.Product)
                .FirstOrDefaultAsync(c => c.Id == cartItemId && c.UserId == CurrentUserId.Value);

            if (cartItem != null)
            {
                if (quantity <= 0)
                {
                    _context.CartItems.Remove(cartItem);
                }
                else if (cartItem.Product.StockQuantity >= quantity)
                {
                    cartItem.Quantity = quantity;
                    _context.Update(cartItem);
                }
                else
                {
                    TempData["Error"] = "Insufficient stock.";
                }

                await _context.SaveChangesAsync();
            }

            return RedirectToAction("Index");
        }

        public async Task<IActionResult> Remove(int id)
        {
            if (CurrentUserId == null) return RedirectToAction("Login", "Account");

            var cartItem = await _context.CartItems
                .FirstOrDefaultAsync(c => c.Id == id && c.UserId == CurrentUserId.Value);

            if (cartItem != null)
            {
                _context.CartItems.Remove(cartItem);
                await _context.SaveChangesAsync();
                TempData["Success"] = "Item removed from cart.";
            }

            return RedirectToAction("Index");
        }
    }
}
