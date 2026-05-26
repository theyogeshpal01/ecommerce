using Microsoft.AspNetCore.Mvc;
using GiftStoreMVC.Data;
using GiftStoreMVC.Models;
using GiftStoreMVC.Filters;
using Microsoft.EntityFrameworkCore;

namespace GiftStoreMVC.Areas.Admin.Controllers
{
    [Area("Admin")]
    [AdminOnly]
    public class OrderController : Controller
    {
        private readonly ApplicationDbContext _context;

        public OrderController(ApplicationDbContext context)
        {
            _context = context;
        }

        public async Task<IActionResult> Index()
        {
            var orders = await _context.Orders
                .Include(o => o.User)
                .OrderByDescending(o => o.OrderDate)
                .ToListAsync();
            return View(orders);
        }

        public async Task<IActionResult> Details(int? id)
        {
            if (id == null) return NotFound();

            var order = await _context.Orders
                .Include(o => o.User)
                .Include(o => o.OrderItems)
                    .ThenInclude(oi => oi.Product)
                .FirstOrDefaultAsync(m => m.Id == id);

            if (order == null) return NotFound();

            return View(order);
        }

        [HttpPost]
        public async Task<IActionResult> UpdateStatus(int orderId, OrderStatus status)
        {
            var order = await _context.Orders.FindAsync(orderId);
            if (order != null)
            {
                order.Status = status;
                _context.Update(order);
                await _context.SaveChangesAsync();
                TempData["Success"] = "Order status updated successfully!";
            }
            return RedirectToAction(nameof(Details), new { id = orderId });
        }
    }
}
