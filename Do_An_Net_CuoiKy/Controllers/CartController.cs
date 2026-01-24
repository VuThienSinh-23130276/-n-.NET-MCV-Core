using Do_An_Net_CuoiKy.Data;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace Do_An_Net_CuoiKy.Controllers
{
    public class CartController : Controller
    {
        private readonly ApplicationDbContext _context;

        public CartController(ApplicationDbContext context)
        {
            _context = context;
        }

        public async Task<IActionResult> Index()
        {
            var userId = HttpContext.Session.GetInt32("UserId");
            if (userId is null)
            {
                TempData["Error"] = "Vui lòng đăng nhập để xem giỏ hàng.";
                return RedirectToAction("Login", "Account");
            }

            var items = await _context.Carts
                .AsNoTracking()
                .Include(c => c.Product)
                .Where(c => c.UserId == userId.Value)
                .ToListAsync();

            return View(items);
        }
    }
}
