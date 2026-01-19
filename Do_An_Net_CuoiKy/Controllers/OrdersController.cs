using Do_An_Net_CuoiKy.Data;
using Do_An_Net_CuoiKy.ViewModels;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace Do_An_Net_CuoiKy.Controllers
{
    public class OrdersController : Controller
    {
        private readonly ApplicationDbContext _db;

        public OrdersController(ApplicationDbContext db)
        {
            _db = db;
        }

        public async Task<IActionResult> History()
        {
            var userId = HttpContext.Session.GetInt32("UserId");
            if (userId is null)
            {
                TempData["Error"] = "Vui lòng đăng nhập để xem đơn hàng.";
                return RedirectToAction("Login", "Account");
            }

            var orders = await _db.Orders
                .AsNoTracking()
                .Where(o => o.UserId == userId.Value)
                .OrderByDescending(o => o.OrderDate)
                .Select(o => new OrderHistoryItemViewModel
                {
                    OrderId = o.Id,
                    OrderDate = o.OrderDate,
                    TotalAmount = o.TotalAmount,
                    Status = o.Status,
                    ItemCount = o.OrderDetails.Count
                })
                .ToListAsync();

            return View(orders);
        }

        public async Task<IActionResult> Details(int id)
        {
            var userId = HttpContext.Session.GetInt32("UserId");
            if (userId is null)
            {
                TempData["Error"] = "Vui lòng đăng nhập để xem đơn hàng.";
                return RedirectToAction("Login", "Account");
            }

            var order = await _db.Orders
                .AsNoTracking()
                .Include(o => o.OrderDetails)
                .ThenInclude(od => od.Product)
                .FirstOrDefaultAsync(o => o.Id == id && o.UserId == userId.Value);

            if (order is null) return NotFound();

            var vm = new OrderDetailsViewModel
            {
                OrderId = order.Id,
                OrderDate = order.OrderDate,
                Status = order.Status,
                TotalAmount = order.TotalAmount,
                ShippingName = order.ShippingName,
                ShippingPhone = order.ShippingPhone,
                ShippingAddress = order.ShippingAddress,
                Notes = order.Notes,
                Lines = order.OrderDetails
                    .Where(od => od.Product != null)
                    .Select(od => new OrderLineViewModel
                    {
                        ProductId = od.ProductId,
                        ProductName = od.Product!.Name,
                        ImageUrl = od.Product!.ImageUrl,
                        Price = od.Price,
                        Quantity = od.Quantity
                    })
                    .ToList()
            };

            return View(vm);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Cancel(int id)
        {
            var userId = HttpContext.Session.GetInt32("UserId");
            if (userId is null)
            {
                TempData["Error"] = "Vui lòng đăng nhập để thao tác đơn hàng.";
                return RedirectToAction("Login", "Account");
            }

            var order = await _db.Orders.FirstOrDefaultAsync(o => o.Id == id && o.UserId == userId.Value);
            if (order is null) return NotFound();

            if (!string.Equals(order.Status, "Pending", StringComparison.OrdinalIgnoreCase))
            {
                TempData["Error"] = "Chỉ có thể hủy đơn ở trạng thái Pending.";
                return RedirectToAction(nameof(Details), new { id });
            }

            order.Status = "Cancel";
            await _db.SaveChangesAsync();

            TempData["Success"] = "Đã hủy đơn hàng.";
            return RedirectToAction(nameof(Details), new { id });
        }
    }
}

