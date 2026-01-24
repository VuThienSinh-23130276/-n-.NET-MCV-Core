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
        public IActionResult Index()
        {
            return View();
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
            ViewBag.ProductId = id;
            return View();
        }
    }
}