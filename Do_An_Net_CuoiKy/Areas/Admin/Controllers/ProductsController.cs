using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Do_An_Net_CuoiKy.Data;
using Do_An_Net_CuoiKy.Models;

namespace Do_An_Net_CuoiKy.Areas.Admin.Controllers
{
    [Area("Admin")]
    public class ProductsController : Controller
    {
        private readonly ApplicationDbContext _context;
        private readonly IWebHostEnvironment _env;

        public ProductsController(ApplicationDbContext context, IWebHostEnvironment env)
        {
            _context = context;
            _env = env;
        }

        // GET: Admin/Products
        public async Task<IActionResult> Index()
        {   
            var products = await _context.Products
                .Include(p => p.Category)
                .OrderByDescending(p => p.Id)
                .ToListAsync();

            // Thống kê
            ViewBag.Total = products.Count;
            ViewBag.Active = products.Count(p => p.IsActive);
            ViewBag.LowStock = products.Count(p => p.Stock < 10);
            ViewBag.InventoryValue = products.Sum(p => p.Price * p.Stock);

            return View(products);
        }


        // POST: Admin/Products/CreateAjax
        [HttpPost]
        public async Task<IActionResult> CreateAjax(Product product, IFormFile? imageFile)
        {
            if (!ModelState.IsValid)
                return Json(new { success = false });

            if (imageFile != null && imageFile.Length > 0)
            {
                var folder = Path.Combine(_env.WebRootPath, "upload/products");
                Directory.CreateDirectory(folder);

                var fileName = Guid.NewGuid() + Path.GetExtension(imageFile.FileName);
                var path = Path.Combine(folder, fileName);

                using var stream = new FileStream(path, FileMode.Create);
                await imageFile.CopyToAsync(stream);

                product.ImageUrl = "/upload/products/" + fileName;
            }

            product.CreatedDate = DateTime.Now;
            _context.Products.Add(product);
            await _context.SaveChangesAsync();

            return Json(new { success = true });
        }

        // GET: Admin/Products/Get/5
        public async Task<IActionResult> Get(int id)
        {
            var product = await _context.Products.FindAsync(id);
            if (product == null) return NotFound();

            return Json(product);
        }

        // POST: Admin/Products/EditAjax
        [HttpPost]
        public async Task<IActionResult> EditAjax(Product product, IFormFile? imageFile)
        {
            var db = await _context.Products.FindAsync(product.Id);
            if (db == null) return Json(new { success = false });

            db.Name = product.Name;
            db.Description = product.Description;
            db.Price = product.Price;
            db.OldPrice = product.OldPrice;
            db.CategoryId = product.CategoryId;
            db.Stock = product.Stock;
            db.IsActive = product.IsActive;

            if (imageFile != null && imageFile.Length > 0)
            {
                var folder = Path.Combine(_env.WebRootPath, "upload/products");
                Directory.CreateDirectory(folder);

                var fileName = Guid.NewGuid() + Path.GetExtension(imageFile.FileName);
                var path = Path.Combine(folder, fileName);

                using var stream = new FileStream(path, FileMode.Create);
                await imageFile.CopyToAsync(stream);

                db.ImageUrl = "/upload/products/" + fileName;
            }

            await _context.SaveChangesAsync();
            return Json(new { success = true });
        }

        // POST: Admin/Products/DeleteAjax
        [HttpPost]
        public async Task<IActionResult> DeleteAjax(int id)
        {
            var product = await _context.Products.FindAsync(id);
            if (product == null) return Json(new { success = false });

            _context.Products.Remove(product);
            await _context.SaveChangesAsync();

            return Json(new { success = true });
        }

        [HttpGet]
        public async Task<IActionResult> GetCategories()
        {
            var categories = await _context.Categories
                .Where(c => c.IsActive)
                .Select(c => new
                {
                    c.Id,
                    c.Name
                })
                .ToListAsync();

            return Json(categories);
        }

        [HttpPost]
        public async Task<IActionResult> ToggleActive(int id)
        {
            var product = await _context.Products.FindAsync(id);
            if (product == null) return NotFound();

            product.IsActive = !product.IsActive;
            await _context.SaveChangesAsync();

            return Ok();
        }

    }
}
