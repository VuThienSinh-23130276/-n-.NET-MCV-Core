using Do_An_Net_CuoiKy.Data;
using Do_An_Net_CuoiKy.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Net.NetworkInformation;
using X.PagedList;
using X.PagedList.Extensions; // Nhớ có dòng này

namespace Do_An_Net_CuoiKy.Controllers
{
    public class ProductController : Controller
    {
        private readonly ApplicationDbContext _context;

        public ProductController(ApplicationDbContext context)
        {
            _context = context;
        }

        public async Task<IActionResult> Index(int? page, string searchString, int[] categoryIds, string priceRange, string sortOrder, int? minRating)
        {
            // 1. Lấy dữ liệu ban đầu
            var products = _context.Products.Include(p => p.Category).AsQueryable();
            ViewBag.Categories = _context.Categories.ToList(); // Gửi danh mục sang View

            // 2. Xử lý Tìm kiếm (Search)
            if (!string.IsNullOrEmpty(searchString))
            {
                products = products.Where(p => p.Name.Contains(searchString));
            }

            // 3. Xử lý Lọc Danh mục
            if (categoryIds != null && categoryIds.Length > 0)
            {
                products = products.Where(p => categoryIds.Contains(p.CategoryId));
            }

            // 4. Xử lý Lọc Giá
            if (!string.IsNullOrEmpty(priceRange))
            {
                switch (priceRange)
                {
                    case "duoi100": products = products.Where(p => p.Price < 100000); break;
                    case "100-300": products = products.Where(p => p.Price >= 100000 && p.Price <= 300000); break;
                    case "300-500": products = products.Where(p => p.Price >= 300000 && p.Price <= 500000); break;
                    case "tren500": products = products.Where(p => p.Price > 500000); break;
                }
            }

            // 5. Xử lý Lọc Đánh Giá (MỚI THÊM)
            if (minRating.HasValue)
            {
                products = products.Where(p => p.Rating >= minRating);
            }

            // 6. Xử lý Sắp xếp
            switch (sortOrder)
            {
                case "price_asc": products = products.OrderBy(p => p.Price); break;
                case "price_desc": products = products.OrderByDescending(p => p.Price); break;
                default: products = products.OrderByDescending(p => p.Id); break;
            }

            // 7. Lưu lại trạng thái bộ lọc (Để giữ nguyên khi chuyển trang)
            ViewBag.CurrentSearch = searchString;
            ViewBag.CurrentPrice = priceRange;
            ViewBag.CurrentSort = sortOrder;
            ViewBag.CurrentCategoryIds = categoryIds;
            ViewBag.CurrentMinRating = minRating; // <--- Lưu lại số sao đang lọc

            // 8. Phân trang & Trả về kết quả
            int pageSize = 9;
            int pageNumber = (page ?? 1);
            var pagedList = products.ToPagedList(pageNumber, pageSize);

            // Kiểm tra AJAX
            if (Request.Headers["X-Requested-With"] == "XMLHttpRequest")
            {
                return PartialView("_ProductList", pagedList);
            }

            return View(pagedList);
        }
        public IActionResult Details(int id)
        {
            var product = _context.Products.Include(p => p.Category).FirstOrDefault(p => p.Id == id);
            if (product == null) return NotFound();
            return View(product);
        }
    }
}