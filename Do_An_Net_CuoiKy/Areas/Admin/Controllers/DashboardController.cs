using Do_An_Net_CuoiKy.Data;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace Do_An_Net_CuoiKy.Areas.Admin.Controllers
{
    [Area("Admin")]
    [Authorize(Roles = "Admin")]
    public class DashboardController : Controller
    {
        private readonly ApplicationDbContext _context;

        public DashboardController(ApplicationDbContext context)
        {
            _context = context;
        }

        public IActionResult Index()
        {
            var today = DateTime.Today;

            ViewBag.TotalOrders = _context.Orders.Count();
            ViewBag.TotalRevenue = _context.Orders
                .Where(o => o.Status == "Completed")
                .Sum(o => (decimal?)o.TotalAmount) ?? 0;

            ViewBag.TodayOrders = _context.Orders
                .Count(o => o.OrderDate.Date == today);

            ViewBag.PendingOrders = _context.Orders.Count(o => o.Status == "Pending");
            ViewBag.ProcessingOrders = _context.Orders.Count(o => o.Status == "Processing");
            ViewBag.CompletedOrders = _context.Orders.Count(o => o.Status == "Completed");
            ViewBag.CancelledOrders = _context.Orders.Count(o => o.Status == "Cancelled");

            // Top sản phẩm bán chạy
            var topProducts = _context.OrderDetails
                .Include(od => od.Product)
                .GroupBy(od => new { od.ProductId, od.Product.Name })
                .Select(g => new
                {
                    ProductName = g.Key.Name,
                    Quantity = g.Sum(x => x.Quantity)
                })
                .OrderByDescending(x => x.Quantity)
                .Take(5)
                .ToList();

            ViewBag.TopProducts = topProducts;

            return View();
        }
    }
}
