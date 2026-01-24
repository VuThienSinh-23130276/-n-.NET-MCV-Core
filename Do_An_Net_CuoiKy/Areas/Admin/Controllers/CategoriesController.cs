using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Do_An_Net_CuoiKy.Data;
using Do_An_Net_CuoiKy.Models;

namespace Do_An_Net_CuoiKy.Areas.Admin.Controllers
{
    [Area("Admin")]
    public class CategoriesController : Controller
    {
        private readonly ApplicationDbContext _context;
        private readonly IWebHostEnvironment _env;

        public CategoriesController(ApplicationDbContext context, IWebHostEnvironment env)
        {
            _context = context;
            _env = env;
        }

        // GET: /Admin/Categories
        public async Task<IActionResult> Index()
        {
            var categories = await _context.Categories
                .OrderByDescending(c => c.Id)
                .ToListAsync();

            return View(categories);
        }


        // POST: /Admin/Categories/CreateAjax
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> CreateAjax(
        [FromForm] Category category,
        [FromForm] IFormFile? imageFile)
        {
            if (string.IsNullOrWhiteSpace(category.Name))
                return Json(new { success = false, message = "Tên danh mục trống" });

            if (imageFile != null && imageFile.Length > 0)
            {
                var folder = Path.Combine(_env.WebRootPath, "upload/categories");
                Directory.CreateDirectory(folder);

                var fileName = Guid.NewGuid() + Path.GetExtension(imageFile.FileName);
                var path = Path.Combine(folder, fileName);

                using var stream = new FileStream(path, FileMode.Create);
                await imageFile.CopyToAsync(stream);

                category.ImageUrl = "/upload/categories/" + fileName;
            }

            _context.Categories.Add(category);
            await _context.SaveChangesAsync();

            return Json(new { success = true });
        }

        // GET: Admin/Categories/Get/5
        public async Task<IActionResult> Get(int id)
        {
            var category = await _context.Categories.FindAsync(id);
            if (category == null) return NotFound();

            return Json(category);
        }
        // POST: Admin/Categories/EditAjax

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> EditAjax(
        [FromForm] Category category,
        [FromForm] IFormFile? imageFile)
        {
            var db = await _context.Categories.FindAsync(category.Id);
            if (db == null)
                return Json(new { success = false });

            db.Name = category.Name;
            db.Description = category.Description;
            db.IsActive = category.IsActive;

            if (imageFile != null && imageFile.Length > 0)
            {
                var folder = Path.Combine(_env.WebRootPath, "upload/categories");
                Directory.CreateDirectory(folder);

                var fileName = Guid.NewGuid() + Path.GetExtension(imageFile.FileName);
                var path = Path.Combine(folder, fileName);

                using var stream = new FileStream(path, FileMode.Create);
                await imageFile.CopyToAsync(stream);

                db.ImageUrl = "/upload/categories/" + fileName;
            }

            await _context.SaveChangesAsync();
            return Json(new { success = true });
        }

        // POST: Admin/Categories/DeleteAjax/5
        [HttpPost]
        public async Task<IActionResult> DeleteAjax(int id)
        {
            var category = await _context.Categories.FindAsync(id);
            if (category == null)
                return Json(new { success = false });

            _context.Categories.Remove(category);
            await _context.SaveChangesAsync();

            return Json(new { success = true });
        }
        [HttpPost]
        public async Task<IActionResult> ToggleActive(int id)
        {
            var category = await _context.Categories.FindAsync(id);
            if (category == null)
                return Json(new { success = false });

            category.IsActive = !category.IsActive;
            await _context.SaveChangesAsync();

            return Json(new { success = true, isActive = category.IsActive });
        }
        [HttpGet]
        public async Task<IActionResult> GetAll()
        {
            var categories = await _context.Categories
                .Where(c => c.IsActive)
                .Select(c => new { c.Id, c.Name })
                .ToListAsync();

            return Json(categories);
        }
    }
}
