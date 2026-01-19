using Microsoft.AspNetCore.Mvc;

namespace Do_An_Net_CuoiKy.Controllers
{
    public class CheckoutController : Controller
    {
        public IActionResult Index()
        {
            return View();
        }

        [HttpPost]
        public IActionResult ProcessOrder()
        {
            // Backend team sẽ implement logic thanh toán ở đây
            return RedirectToAction("Success");
        }

        public IActionResult Success()
        {
            return View();
        }
    }
}
