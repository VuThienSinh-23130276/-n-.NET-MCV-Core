using Microsoft.AspNetCore.Mvc;

namespace Do_An_Net_CuoiKy.Controllers
{
    public class ProductController : Controller
    {
        public IActionResult Index()
        {
            return View();
        }

        public IActionResult Details(int id)
        {
            ViewBag.ProductId = id;
            return View();
        }
    }
}
