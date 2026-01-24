using Do_An_Net_CuoiKy.Data;
using Do_An_Net_CuoiKy.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using X.PagedList;

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
            var products = _context.Products.Include(p => p.Category).AsQueryable();
            ViewBag.Categories = _context.Categories.ToList();

            if (!string.IsNullOrWhiteSpace(searchString))
            {
                searchString = searchString.Trim();
                products = products.Where(p => EF.Functions.Like(p.Name, $"%{searchString}%"));
            }

            if (categoryIds != null && categoryIds.Length > 0)
            {
                products = products.Where(p => categoryIds.Contains(p.CategoryId));
            }

            if (!string.IsNullOrEmpty(priceRange))
            {
                switch (priceRange)
                {
                    case "duoi100":
                        products = products.Where(p => p.Price < 100000);
                        break;
                    case "100-300":
                        products = products.Where(p => p.Price >= 100000 && p.Price <= 300000);
                        break;
                    case "300-500":
                        products = products.Where(p => p.Price >= 300000 && p.Price <= 500000);
                        break;
                    case "tren500":
                        products = products.Where(p => p.Price > 500000);
                        break;
                }
            }

            if (minRating.HasValue)
            {
                products = products.Where(p => p.Rating >= minRating);
            }

            switch (sortOrder)
            {
                case "price_asc":
                    products = products.OrderBy(p => p.Price);
                    break;
                case "price_desc":
                    products = products.OrderByDescending(p => p.Price);
                    break;
                default:
                    products = products.OrderByDescending(p => p.Id);
                    break;
            }

            // 7. Lưu lại trạng thái bộ lọc (Để giữ nguyên khi chuyển trang)
            ViewBag.CurrentSearch = searchString;
            ViewBag.CurrentPrice = priceRange;
            ViewBag.CurrentSort = sortOrder;
            ViewBag.CurrentCategoryIds = categoryIds;
            ViewBag.CurrentMinRating = minRating;

            // 8. Phân trang & Trả về kết quả
            int pageSize = 9;
            int pageNumber = page ?? 1;
            var totalItemCount = products.Count();
            var items = products
                .Skip((pageNumber - 1) * pageSize)
                .Take(pageSize)
                .ToList();
            var pagedList = new StaticPagedList<Product>(items, pageNumber, pageSize, totalItemCount);

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