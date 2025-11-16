using Microsoft.AspNetCore.Mvc;
using OrderSystem.Services.Shared.Models;
using OrderSystem.Services.Services;
using System.Collections.Generic;
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

        public async Task<IActionResult> AddItem(string productId, int quantity = 1)
        {
            var userId = HttpContext.Session.GetString("UserId");
            if (string.IsNullOrEmpty(userId)) return RedirectToAction("Login", "Account");

            await _cartService.AddItemAsync(userId, productId, quantity); 
            TempData["Success"] = "Added to cart.";
            return RedirectToAction("Shop", "Product");
        }

        // View Cart
        public async Task<IActionResult> ViewCart()
        {
            var userId = HttpContext.Session.GetString("UserId");
            if (string.IsNullOrEmpty(userId)) return RedirectToAction("Login", "Account");

            var items = await _cartService.GetCartItemsAsync(userId);
            return View(items);
        }

        // Update Quantities
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> UpdateCart(Dictionary<string, int> quantities)
        {
            var userId = HttpContext.Session.GetString("UserId");
            if (string.IsNullOrEmpty(userId)) return RedirectToAction("Login", "Account");

            foreach (var kvp in quantities)
            {
                if (kvp.Value <= 0)
                    await _cartService.RemoveItemAsync(userId, kvp.Key);
                else
                    await _cartService.UpdateQuantityAsync(kvp.Key, kvp.Value);
            }

            TempData["Success"] = "Cart updated.";
            return RedirectToAction("ViewCart");
        }

        // Remove Item
        public async Task<IActionResult> RemoveItem(string cartItemId)
        {
            var userId = HttpContext.Session.GetString("UserId");
            if (string.IsNullOrEmpty(userId)) return RedirectToAction("Login", "Account");

            await _cartService.RemoveItemAsync(userId, cartItemId);
            TempData["Success"] = "Item removed.";
            return RedirectToAction("ViewCart");
        }

        // Checkout
        public async Task<IActionResult> Checkout()
        {
            var userId = HttpContext.Session.GetString("UserId");
            if (string.IsNullOrEmpty(userId)) return RedirectToAction("Login", "Account");

            var orderId = await _cartService.CheckoutAsync(userId);
            TempData["Success"] = $"Order placed! ID: {orderId}";
            return RedirectToAction("Index", "Order");
        }
    }
}