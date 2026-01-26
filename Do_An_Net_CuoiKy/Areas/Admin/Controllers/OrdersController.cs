using Do_An_Net_CuoiKy.Data;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace Do_An_Net_CuoiKy.Areas.Admin.Controllers
{
    [Area("Admin")]
    [Authorize(Roles = "Admin")]
    public class OrdersController : Controller
    {
        private readonly ApplicationDbContext _context;

        public OrdersController(ApplicationDbContext context)
        {
            _context = context;
        }

        // GET: /Admin/Orders
        public IActionResult Index(string search, string status)
        {
            var query = _context.Orders
                .Include(o => o.User)
                .Include(o => o.OrderDetails)
                    .ThenInclude(od => od.Product)
                .AsQueryable();

            // Search
            if (!string.IsNullOrEmpty(search))
            {
                query = query.Where(o =>
                    o.Id.ToString().Contains(search) ||
                    o.User.Email.Contains(search) ||
                    o.ShippingName.Contains(search));
            }

            // Filter status
            if (!string.IsNullOrEmpty(status))
            {
                query = query.Where(o => o.Status == status);
            }

            ViewBag.Search = search;
            ViewBag.Status = status;

            return View(query.OrderByDescending(o => o.OrderDate).ToList());
        }

        // GET: /Admin/Orders/Details/5
        public IActionResult Details(int id)
        {
            var order = _context.Orders
                .Include(o => o.User)
                .Include(o => o.OrderDetails)
                    .ThenInclude(od => od.Product)
                .FirstOrDefault(o => o.Id == id);

            if (order == null) return NotFound();

            return View(order);
        }

        // POST: Update status
        [HttpPost]
        public IActionResult UpdateStatus(int id, string status)
        {
            var order = _context.Orders.Find(id);
            if (order != null)
            {
                order.Status = status;
                _context.SaveChanges();
            }
            return RedirectToAction(nameof(Index));
        }
    }
}
