using Microsoft.AspNetCore.Mvc;
using OrderSystem.Models;
using OrderSystem.Services;
using System.Threading.Tasks;

namespace OrderSystem.Controllers
{
    public class CartController : Controller
    {
        private readonly CartService _cartService;

        public CartController(CartService cartService)
        {
            _cartService = cartService;
        }

        public async Task<IActionResult> AddToCart(string productId)
        {
            if (string.IsNullOrEmpty(productId))
                return BadRequest("Invalid product ID.");

            var userId = HttpContext.Session.GetString("UserId");
            if (string.IsNullOrEmpty(userId))
                return RedirectToAction("Login", "Account");

            await _cartService.AddItemAsync(userId, productId);
            return RedirectToAction("ViewCart");
        }
        public async Task<IActionResult> ViewCart()
        {
            var userId = HttpContext.Session.GetString("UserId");
            if (string.IsNullOrEmpty(userId))
                return RedirectToAction("Login", "Account");

            var cartItems = await _cartService.GetCartItemsAsync(userId);

            
            var enrichedItems = new List<CartItem>();
            foreach (var item in cartItems)
            {
                var product = await _cartService.GetProductByIdAsync(item.ProductId); 
                if (product != null)
                {
                    item.ProductName = product.ProductName;
                    item.Price = product.Price;
                    enrichedItems.Add(item);
                }
            }

            return View(enrichedItems);
        }

        [HttpPost]
        public async Task<IActionResult> UpdateQuantity(string cartItemId, int quantity)
        {
            if (quantity <= 0)
                return BadRequest("Quantity must be greater than zero.");

            await _cartService.UpdateQuantityAsync(cartItemId, quantity);
            return RedirectToAction("ViewCart");
        }

        public async Task<IActionResult> RemoveItem(string cartItemId)
        {
            await _cartService.RemoveItemAsync(cartItemId);
            return RedirectToAction("ViewCart");
        }

        public async Task<IActionResult> Checkout()
        {
            var userId = HttpContext.Session.GetString("UserId");
            if (string.IsNullOrEmpty(userId))
                return RedirectToAction("Login", "Account");

            var orderId = await _cartService.CheckoutAsync(userId);
            return RedirectToAction("Details", "Order", new { id = orderId });
        }
    }
}
