using System.Diagnostics;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using PetStore.Models;

namespace PetStore.Controllers
{
    public class HomeController : Controller
    {
        private readonly ILogger<HomeController> _logger;
        private readonly AppDbContext _context;

        public HomeController(ILogger<HomeController> logger, AppDbContext context)
        {
            _logger = logger;
            _context = context;
        }

        public async Task<IActionResult> Index()
        {
            try
            {
                // Get featured products (latest 6 products with stock)
                var featuredProducts = await _context.Products
                    .Include(p => p.Category)
                    .Where(p => p.StockQuantity > 0)
                    .OrderByDescending(p => p.CreatedAt)
                    .Take(6)
                    .ToListAsync();

                // Get all categories
                var categories = await _context.Categories
                    .Include(c => c.Products)
                    .Where(c => c.Products != null && c.Products.Any(p => p.StockQuantity > 0))
                    .ToListAsync();

                // Get statistics for homepage
                var totalProducts = await _context.Products.CountAsync();
                var totalCategories = await _context.Categories.CountAsync();
                var inStockProducts = await _context.Products.Where(p => p.StockQuantity > 0).CountAsync();

                ViewBag.FeaturedProducts = featuredProducts;
                ViewBag.Categories = categories;
                ViewBag.TotalProducts = totalProducts;
                ViewBag.TotalCategories = totalCategories;
                ViewBag.InStockProducts = inStockProducts;

                return View();
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error loading homepage data");
                ViewBag.FeaturedProducts = new List<Product>();
                ViewBag.Categories = new List<Category>();
                ViewBag.TotalProducts = 0;
                ViewBag.TotalCategories = 0;
                ViewBag.InStockProducts = 0;
                return View();
            }
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

        public IActionResult About()
        {
            return View();
        }

        public IActionResult Contact()
        {
            return View();
        }

    }
}
