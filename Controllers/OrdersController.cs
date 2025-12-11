using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using PetStore.Models;

namespace PetStore.Controllers
{
    public class OrdersController : Controller
    {
        private readonly AppDbContext _context;
        private const string CartIdSessionKey = "CartID";

        public OrdersController(AppDbContext context)
        {
            _context = context;
        }

        // Helper: Get Cart ID from Session or DB
        private async Task<int> GetCartIdAsync(int userId)
        {
            var cartId = HttpContext.Session.GetInt32(CartIdSessionKey);
            if (cartId.HasValue) return cartId.Value;

            var existingCart = await _context.ShoppingCarts.FirstOrDefaultAsync(c => c.UserID == userId);
            return existingCart?.CartID ?? 0;
        }

        // GET: /Orders/Checkout
        [HttpGet]
        public async Task<IActionResult> Checkout()
        {
            var userId = HttpContext.Session.GetInt32("UserID");
            if (!userId.HasValue)
            {
                return RedirectToAction("Login", "Account", new { returnUrl = Url.Action("Checkout", "Orders") });
            }

            var cartId = await GetCartIdAsync(userId.Value);
            var items = await _context.CartItems
                .Include(c => c.Product)
                .Where(c => c.CartID == cartId)
                .ToListAsync();

            if (!items.Any())
            {
                return RedirectToAction("Index", "Cart");
            }

            var model = new CheckoutModel
            {
                Total = items.Sum(i => i.Quantity * i.Product.Price) // Calculate REAL total
            };
            return View(model);
        }

        // POST: /Orders/Checkout
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Checkout(CheckoutModel model)
        {
            var userId = HttpContext.Session.GetInt32("UserID");
            if (!userId.HasValue) return RedirectToAction("Login", "Account");

            if (!ModelState.IsValid)
            {
                return View(model);
            }
            var cartId = await GetCartIdAsync(userId.Value);
            
            var cartItems = await _context.CartItems
                .Include(ci => ci.Product)
                .Where(ci => ci.CartID == cartId)
                .ToListAsync();

            if (cartItems == null || !cartItems.Any())
            {
                ModelState.AddModelError("", "Your cart is empty.");
                return View(model);
            }

            // Create and Save the Order
            var order = new Order
            {
                UserID = userId.Value,
                OrderDate = DateTime.Now,
                TotalAmount = cartItems.Sum(i => i.Quantity * i.Product.Price),
                Status = "Pending",
                ShippingStreetAddress = model.Address,
                ShippingCity = model.City,
                ShippingProvince = model.Province,
                ShippingPostalCode = model.Zip,
                ShippingLandmark = ""
            };

            _context.Orders.Add(order);
            await _context.SaveChangesAsync();

            // Move items to OrderedItems Table
            foreach (var item in cartItems)
            {
                var orderedItem = new OrderedItem
                {
                    OrderID = order.OrderID,
                    ProductID = item.ProductID,
                    Quantity = item.Quantity,
                    PriceAtPurchase = item.Product.Price
                };
                _context.OrderedItems.Add(orderedItem);
            }

            // Clear the Cart
            _context.CartItems.RemoveRange(cartItems);
            await _context.SaveChangesAsync();

            HttpContext.Session.Remove(CartIdSessionKey);

            ViewBag.OrderSuccess = true;
            return View(model);
        }
    }
}