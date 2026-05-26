using Microsoft.AspNetCore.Mvc;
using GiftStoreMVC.Data;
using Microsoft.EntityFrameworkCore;
using Microsoft.AspNetCore.Http;

namespace GiftStoreMVC.ViewComponents
{
    public class CartSummaryViewComponent : ViewComponent
    {
        private readonly ApplicationDbContext _context;

        public CartSummaryViewComponent(ApplicationDbContext context)
        {
            _context = context;
        }

        public async Task<IViewComponentResult> InvokeAsync()
        {
            var userId = HttpContext.Session.GetInt32("UserId");
            int count = 0;
            decimal total = 0;

            if (userId != null)
            {
                var cartItems = await _context.CartItems
                    .Include(c => c.Product)
                    .Where(c => c.UserId == userId)
                    .ToListAsync();

                count = cartItems.Sum(i => i.Quantity);
                total = cartItems.Sum(i => i.Product.Price * i.Quantity);
            }

            ViewBag.Count = count;
            ViewBag.Total = total;

            return View();
        }
    }
}
