using Microsoft.AspNetCore.Mvc;
using GiftStoreMVC.Data;
using GiftStoreMVC.Models;
using GiftStoreMVC.ViewModels;
using Microsoft.EntityFrameworkCore;

namespace GiftStoreMVC.Controllers
{
    public class OrderController : Controller
    {
        private readonly ApplicationDbContext _context;

        public OrderController(ApplicationDbContext context)
        {
            _context = context;
        }

        private int? CurrentUserId => HttpContext.Session.GetInt32("UserId");

        [HttpGet]
        public async Task<IActionResult> Checkout()
        {
            if (CurrentUserId == null) return RedirectToAction("Login", "Account");

            var cartItems = await _context.CartItems
                .Include(c => c.Product)
                .Where(c => c.UserId == CurrentUserId.Value)
                .ToListAsync();

            if (!cartItems.Any())
            {
                TempData["Error"] = "Your cart is empty.";
                return RedirectToAction("Index", "Cart");
            }

            var user = await _context.Users.FindAsync(CurrentUserId.Value);

            var model = new CheckoutViewModel
            {
                CartItems = cartItems,
                TotalAmount = cartItems.Sum(i => i.Product.Price * i.Quantity),
                ShippingAddress = user.Address
            };

            return View(model);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Checkout(CheckoutViewModel model)
        {
            if (CurrentUserId == null) return RedirectToAction("Login", "Account");

            var cartItems = await _context.CartItems
                .Include(c => c.Product)
                .Where(c => c.UserId == CurrentUserId.Value)
                .ToListAsync();

            if (!cartItems.Any()) return RedirectToAction("Index", "Cart");

            if (ModelState.IsValid)
            {
                var totalAmount = cartItems.Sum(i => i.Product.Price * i.Quantity);
                if (totalAmount <= 50) totalAmount += 5; // Shipping

                var order = new Order
                {
                    UserId = CurrentUserId.Value,
                    TotalAmount = totalAmount,
                    ShippingAddress = model.ShippingAddress,
                    Status = OrderStatus.Pending,
                    PaymentMethod = "Cash on Delivery",
                    OrderItems = cartItems.Select(ci => new OrderItem
                    {
                        ProductId = ci.ProductId,
                        Quantity = ci.Quantity,
                        Price = ci.Product.Price
                    }).ToList()
                };

                // Update product stock
                foreach (var item in cartItems)
                {
                    item.Product.StockQuantity -= item.Quantity;
                    _context.Update(item.Product);
                }

                _context.Orders.Add(order);
                _context.CartItems.RemoveRange(cartItems);
                await _context.SaveChangesAsync();

                return RedirectToAction("Confirmation", new { id = order.Id });
            }

            model.CartItems = cartItems;
            model.TotalAmount = cartItems.Sum(i => i.Product.Price * i.Quantity);
            return View(model);
        }

        public async Task<IActionResult> Confirmation(int id)
        {
            var order = await _context.Orders
                .Include(o => o.OrderItems)
                    .ThenInclude(oi => oi.Product)
                .FirstOrDefaultAsync(o => o.Id == id && o.UserId == CurrentUserId);

            if (order == null) return NotFound();

            return View(order);
        }

        public async Task<IActionResult> History()
        {
            if (CurrentUserId == null) return RedirectToAction("Login", "Account");

            var orders = await _context.Orders
                .Where(o => o.UserId == CurrentUserId.Value)
                .OrderByDescending(o => o.OrderDate)
                .ToListAsync();

            return View(orders);
        }

        public async Task<IActionResult> Details(int id)
        {
            var order = await _context.Orders
                .Include(o => o.OrderItems)
                    .ThenInclude(oi => oi.Product)
                .FirstOrDefaultAsync(o => o.Id == id && o.UserId == CurrentUserId);

            if (order == null) return NotFound();

            return View(order);
        }
    }
}
