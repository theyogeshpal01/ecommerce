using Microsoft.AspNetCore.Mvc;
using GiftStoreMVC.Data;
using GiftStoreMVC.Models;
using GiftStoreMVC.Filters;
using Microsoft.EntityFrameworkCore;

namespace GiftStoreMVC.Areas.Admin.Controllers
{
    [Area("Admin")]
    [AdminOnly]
    public class ProductController : Controller
    {
        private readonly ApplicationDbContext _context;
        private readonly IWebHostEnvironment _hostEnvironment;

        public ProductController(ApplicationDbContext context, IWebHostEnvironment hostEnvironment)
        {
            _context = context;
            _hostEnvironment = hostEnvironment;
        }

        public async Task<IActionResult> Index()
        {
            var products = await _context.Products.Include(p => p.Category).ToListAsync();
            return View(products);
        }

        [HttpGet]
        public async Task<IActionResult> Create()
        {
            ViewBag.Categories = await _context.Categories.ToListAsync();
            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(Product product, IFormFile coverImage, List<IFormFile> productImages)
        {
            ModelState.Remove("Category");
            ModelState.Remove("ProductImages");
            ModelState.Remove("Reviews");
            ModelState.Remove("CoverImageUrl");

            if (ModelState.IsValid)
            {
                if (coverImage != null)
                {
                    product.CoverImageUrl = await SaveImage(coverImage);
                }

                _context.Add(product);
                await _context.SaveChangesAsync();

                if (productImages != null && productImages.Count > 0)
                {
                    foreach (var image in productImages)
                    {
                        var imageUrl = await SaveImage(image);
                        _context.ProductImages.Add(new ProductImage
                        {
                            ProductId = product.Id,
                            ImageUrl = imageUrl
                        });
                    }
                    await _context.SaveChangesAsync();
                }

                TempData["Success"] = "Product created successfully!";
                return RedirectToAction(nameof(Index));
            }
            ViewBag.Categories = await _context.Categories.ToListAsync();
            return View(product);
        }

        [HttpGet]
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null) return NotFound();

            var product = await _context.Products.Include(p => p.ProductImages).FirstOrDefaultAsync(p => p.Id == id);
            if (product == null) return NotFound();

            ViewBag.Categories = await _context.Categories.ToListAsync();
            return View(product);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, Product product, IFormFile? coverImage, List<IFormFile>? productImages)
        {
            if (id != product.Id) return NotFound();

            ModelState.Remove("Category");
            ModelState.Remove("ProductImages");
            ModelState.Remove("Reviews");
            ModelState.Remove("CoverImageUrl");

            if (ModelState.IsValid)
            {
                try
                {
                    var existingProduct = await _context.Products.AsNoTracking().FirstOrDefaultAsync(p => p.Id == id);
                    
                    if (coverImage != null)
                    {
                        product.CoverImageUrl = await SaveImage(coverImage);
                    }
                    else
                    {
                        product.CoverImageUrl = existingProduct.CoverImageUrl;
                    }

                    _context.Update(product);
                    await _context.SaveChangesAsync();

                    if (productImages != null && productImages.Count > 0)
                    {
                        foreach (var image in productImages)
                        {
                            var imageUrl = await SaveImage(image);
                            _context.ProductImages.Add(new ProductImage
                            {
                                ProductId = product.Id,
                                ImageUrl = imageUrl
                            });
                        }
                        await _context.SaveChangesAsync();
                    }

                    TempData["Success"] = "Product updated successfully!";
                    return RedirectToAction(nameof(Index));
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!ProductExists(product.Id)) return NotFound();
                    else throw;
                }
            }
            ViewBag.Categories = await _context.Categories.ToListAsync();
            return View(product);
        }

        [HttpPost]
        public async Task<IActionResult> Delete(int id)
        {
            var product = await _context.Products.FindAsync(id);
            if (product != null)
            {
                _context.Products.Remove(product);
                await _context.SaveChangesAsync();
                return Json(new { success = true });
            }
            return Json(new { success = false });
        }

        private async Task<string> SaveImage(IFormFile image)
        {
            string wwwRootPath = _hostEnvironment.WebRootPath;
            string fileName = Guid.NewGuid().ToString() + Path.GetExtension(image.FileName);
            string path = Path.Combine(wwwRootPath, @"images\products", fileName);

            using (var fileStream = new FileStream(path, FileMode.Create))
            {
                await image.CopyToAsync(fileStream);
            }

            return @"/images/products/" + fileName;
        }

        private bool ProductExists(int id)
        {
            return _context.Products.Any(e => e.Id == id);
        }
    }
}
