/*
 * Author: Gunpreet Singh
 * Id: 9022194
 */

using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using PetStore.Models;
using System;

namespace PetStore.Controllers
{
    public class ProductsController : Controller
    {
        private readonly AppDbContext _context;

        public ProductsController(AppDbContext context)
        {
            _context = context;
        }

        public async Task<IActionResult> Index(string searchTerm, int? categoryId, string sortBy = "name")
        {
            try
            {
                var query = _context.Products
                    .Include(p => p.Category)
                    .AsQueryable();

                // Search functionality 
                if (!string.IsNullOrEmpty(searchTerm))
                {
                    var searchLower = searchTerm.ToLower();
                    query = query.Where(p => 
                        (p.ProductName != null && p.ProductName.ToLower().Contains(searchLower)) ||
                        (p.Breed != null && p.Breed.ToLower().Contains(searchLower)));
                }

                // Category filter
                if (categoryId.HasValue && categoryId.Value > 0)
                {
                    query = query.Where(p => p.CategoryID == categoryId.Value);
                }

                // Sorting
                switch (sortBy?.ToLower() ?? "name")
                {
                    case "price_low":
                        query = query.OrderBy(p => p.Price);
                        break;
                    case "price_high":
                        query = query.OrderByDescending(p => p.Price);
                        break;
                    case "age":
                        query = query.OrderBy(p => p.AgeInMonths);
                        break;
                    case "name":
                    default:
                        query = query.OrderBy(p => p.ProductName);
                        break;
                }

                var products = await query.ToListAsync();
                var categories = await _context.Categories.OrderBy(c => c.CategoryName).ToListAsync();

                // Debug: Check if we have data
                System.Diagnostics.Debug.WriteLine($"Products count: {products.Count}");
                System.Diagnostics.Debug.WriteLine($"Categories count: {categories.Count}");

                ViewBag.Categories = categories ?? new List<Category>();
                ViewBag.SelectedCategoryId = categoryId;
                ViewBag.SearchTerm = searchTerm ?? "";
                ViewBag.SortBy = sortBy ?? "name";

                return View(products);
            }
            catch (Exception ex)
            {
                // Log error with full details
                System.Diagnostics.Debug.WriteLine($"Error loading products: {ex.Message}");
                System.Diagnostics.Debug.WriteLine($"Stack trace: {ex.StackTrace}");
                if (ex.InnerException != null)
                {
                    System.Diagnostics.Debug.WriteLine($"Inner exception: {ex.InnerException.Message}");
                }
                // Return empty list but log the error
                return View(new List<Product>());
            }
        }
    }
}

