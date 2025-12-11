using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using PetStore.Models;
using PetStore.ViewModels;

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

        // GET: /Orders/Checkout
        public IActionResult Checkout()
        {
            return View();
        }

        // POST: /Orders/Checkout
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Checkout(CheckoutViewModel model)
        {
            if (!ModelState.IsValid)
            {
                return View(model);
            }

            // Optional: Save order data here
            // var order = new Order { ... };
            // _context.Orders.Add(order);
            // await _context.SaveChangesAsync();

            // 1. Get Cart ID from session
            int? cartId = HttpContext.Session.GetInt32(CartIdSessionKey);

            if (cartId.HasValue)
            {
                // 2. Load all cart items for this cart
                var items = await _context.CartItems
                    .Where(ci => ci.CartID == cartId.Value)
                    .ToListAsync();

                // 3. Remove all cart items
                if (items.Any())
                {
                    _context.CartItems.RemoveRange(items);
                    await _context.SaveChangesAsync();
                }

                // 4. Optionally, remove Cart itself
                var cart = await _context.ShoppingCarts
                    .FirstOrDefaultAsync(c => c.CartID == cartId.Value);

                if (cart != null)
                {
                    _context.ShoppingCarts.Remove(cart);
                    await _context.SaveChangesAsync();
                }

                // 5. Clear session
                HttpContext.Session.Remove(CartIdSessionKey);
            }

            // 6. Redirect to Cart page or show success message
            TempData["SuccessMessage"] = "Your order was successfully placed!";
            return RedirectToAction("Index", "Cart");
        }
    }
}
