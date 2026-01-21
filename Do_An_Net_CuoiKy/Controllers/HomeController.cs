using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Do_An_Net_CuoiKy.Models;
using Do_An_Net_CuoiKy.Data; // Quan trọng: Để dùng được ApplicationDbContext
using System.Diagnostics;

namespace Do_An_Net_CuoiKy.Controllers
{
    public class HomeController : Controller
    {
        private readonly ILogger<HomeController> _logger;

        // 1. Khai báo biến để kết nối Database
        private readonly ApplicationDbContext _context;

        // 2. Nạp (Inject) Database vào Controller
        public HomeController(ILogger<HomeController> logger, ApplicationDbContext context)
        {
            _logger = logger;
            _context = context;
        }

        public IActionResult Index()
        {
            // 3. Lấy 8 sản phẩm mới nhất (Sắp xếp ID giảm dần) để gửi sang View
            var sanPhamNoiBat = _context.Products
                                .OrderByDescending(p => p.Id) // Mới nhất lên đầu
                                .Take(8) // Chỉ lấy 8 cái
                                .ToList();

            // 4. Gửi danh sách sản phẩm này sang file Index.cshtml
            return View(sanPhamNoiBat);
        }

        public IActionResult Privacy()
        {
            return View();
        }

        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error()
        {
            return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
        }
    }
}