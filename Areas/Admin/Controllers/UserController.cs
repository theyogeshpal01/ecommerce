using Microsoft.AspNetCore.Mvc;
using GiftStoreMVC.Data;
using GiftStoreMVC.Models;
using GiftStoreMVC.Filters;
using Microsoft.EntityFrameworkCore;

namespace GiftStoreMVC.Areas.Admin.Controllers
{
    [Area("Admin")]
    [AdminOnly]
    public class UserController : Controller
    {
        private readonly ApplicationDbContext _context;

        public UserController(ApplicationDbContext context)
        {
            _context = context;
        }

        public async Task<IActionResult> Index()
        {
            var users = await _context.Users.ToListAsync();
            return View(users);
        }

        [HttpPost]
        public async Task<IActionResult> ToggleStatus(int id)
        {
            var currentUserId = HttpContext.Session.GetInt32("UserId");
            if (currentUserId == id)
            {
                return Json(new { success = false, message = "You cannot deactivate your own account." });
            }

            var user = await _context.Users.FindAsync(id);
            if (user != null)
            {
                user.IsActive = !user.IsActive;
                _context.Update(user);
                await _context.SaveChangesAsync();
                return Json(new { success = true, isActive = user.IsActive });
            }
            return Json(new { success = false });
        }

        [HttpPost]
        public async Task<IActionResult> Delete(int id)
        {
            var user = await _context.Users.FindAsync(id);
            if (user != null)
            {
                if (user.Role == UserRole.Admin) return Json(new { success = false, message = "Cannot delete admin user." });

                _context.Users.Remove(user);
                await _context.SaveChangesAsync();
                return Json(new { success = true });
            }
            return Json(new { success = false });
        }
    }
}
