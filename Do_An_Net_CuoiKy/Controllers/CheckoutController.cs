using Do_An_Net_CuoiKy.Data;
using Do_An_Net_CuoiKy.Models;
using Do_An_Net_CuoiKy.ViewModels;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace Do_An_Net_CuoiKy.Controllers
{
    public class CheckoutController : Controller
    {
        private readonly ApplicationDbContext _db;

        public CheckoutController(ApplicationDbContext db)
        {
            _db = db;
        }

        public async Task<IActionResult> Index()
        {

            var userId = HttpContext.Session.GetInt32("UserId");
            if (userId is null)
            {
                TempData["Error"] = "Vui lòng đăng nhập để thanh toán.";
                return RedirectToAction("Login", "Account");
            }

            var cartItems = await _db.Carts
                .AsNoTracking()
                .Include(c => c.Product)
                .Where(c => c.UserId == userId.Value)
                .ToListAsync();

            if (cartItems.Count == 0)
            {
                TempData["Error"] = "Giỏ hàng đang trống.";
                return RedirectToAction("Index", "Cart");
            }

            var user = await _db.Users.AsNoTracking().FirstOrDefaultAsync(u => u.Id == userId.Value);

            var vm = new CheckoutPageViewModel
            {
                Items = cartItems
                    .Where(c => c.Product != null)
                    .Select(c => new CheckoutItemViewModel
                    {
                        ProductId = c.ProductId,
                        ProductName = c.Product!.Name,
                        ImageUrl = c.Product!.ImageUrl,
                        Price = c.Product!.Price,
                        Quantity = c.Quantity
                    })
                    .ToList()
            };

            vm.Subtotal = vm.Items.Sum(i => i.LineTotal);
            vm.ShippingFee = 0;
            vm.Discount = 0;

            if (user != null)
            {
                vm.Input.FullName = user.FullName;
                vm.Input.Phone = user.Phone ?? string.Empty;
                vm.Input.Email = user.Email;
                vm.Input.Address = user.Address ?? string.Empty;
            }

            return View(vm);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> ProcessOrder([Bind(Prefix = "Input")] CheckoutInputModel input)
        {
            var userId = HttpContext.Session.GetInt32("UserId");
            if (userId is null)
            {
                TempData["Error"] = "Vui lòng đăng nhập để thanh toán.";
                return RedirectToAction("Login", "Account");
            }

            if (!ModelState.IsValid)
            {
                // Rebuild summary to show again
                var cartItemsInvalid = await _db.Carts
                    .AsNoTracking()
                    .Include(c => c.Product)
                    .Where(c => c.UserId == userId.Value)
                    .ToListAsync();

                var vmInvalid = new CheckoutPageViewModel
                {
                    Input = input,
                    Items = cartItemsInvalid
                        .Where(c => c.Product != null)
                        .Select(c => new CheckoutItemViewModel
                        {
                            ProductId = c.ProductId,
                            ProductName = c.Product!.Name,
                            ImageUrl = c.Product!.ImageUrl,
                            Price = c.Product!.Price,
                            Quantity = c.Quantity
                        })
                        .ToList()
                };
                vmInvalid.Subtotal = vmInvalid.Items.Sum(i => i.LineTotal);
                return View("Index", vmInvalid);
            }

            var cartItems = await _db.Carts
                .Include(c => c.Product)
                .Where(c => c.UserId == userId.Value)
                .ToListAsync();

            if (cartItems.Count == 0)
            {
                TempData["Error"] = "Giỏ hàng đang trống.";
                return RedirectToAction("Index", "Cart");
            }

            var lines = cartItems
                .Where(c => c.Product != null)
                .Select(c => new
                {
                    c.ProductId,
                    ProductName = c.Product!.Name,
                    Price = c.Product!.Price,
                    c.Quantity
                })
                .ToList();

            var subtotal = lines.Sum(l => l.Price * l.Quantity);

            var order = new Order
            {
                UserId = userId.Value,
                OrderDate = DateTime.Now,
                TotalAmount = subtotal,
                Status = "Pending",
                ShippingName = input.FullName,
                ShippingPhone = input.Phone,
                ShippingAddress = $"{input.Address} {input.Ward} {input.District} {input.City}".Trim(),
                Notes = input.Notes
            };

            foreach (var line in lines)
            {
                order.OrderDetails.Add(new OrderDetail
                {
                    ProductId = line.ProductId,
                    Quantity = line.Quantity,
                    Price = line.Price
                });
            }

            _db.Orders.Add(order);
            _db.Carts.RemoveRange(cartItems);
            await _db.SaveChangesAsync();

            return RedirectToAction("Success", new { id = order.Id });
        }

        public async Task<IActionResult> Success(int id)
        {
            var userId = HttpContext.Session.GetInt32("UserId");
            if (userId is null)
            {
                TempData["Error"] = "Vui lòng đăng nhập để xem đơn hàng.";
                return RedirectToAction("Login", "Account");
            }

            var order = await _db.Orders
                .AsNoTracking()
                .FirstOrDefaultAsync(o => o.Id == id && o.UserId == userId.Value);

            if (order is null) return NotFound();

            var vm = new OrderSuccessViewModel
            {
                OrderId = order.Id,
                OrderDate = order.OrderDate,
                TotalAmount = order.TotalAmount
            };

            return View(vm);
        }
    }
}
