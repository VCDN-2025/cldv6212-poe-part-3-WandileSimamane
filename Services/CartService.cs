using OrderSystem.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace OrderSystem.Services
{
    public class CartService
    {
        private readonly TableService _tableService;

        public CartService(TableService tableService)
        {
            _tableService = tableService;
        }

        public async Task<CartEntity> GetCartAsync(string userId)
        {
            return await _tableService.GetCartAsync(userId);
        }

        public async Task<List<CartItem>> GetCartItemsAsync(string userId)
        {
            var cart = await GetCartAsync(userId);
            return await _tableService.GetCartItemsAsync(cart.RowKey);
        }

        public async Task AddItemAsync(string userId, string productId)
        {
            var cart = await GetCartAsync(userId);
            var items = await _tableService.GetCartItemsAsync(cart.RowKey);

            var existing = items.FirstOrDefault(i => i.ProductId == productId);
            if (existing != null)
            {
                existing.Quantity++;
                await _tableService.UpdateCartItemAsync(existing);
            }
            else
            {
                var item = new CartItem
                {
                    PartitionKey = cart.RowKey,
                    RowKey = Guid.NewGuid().ToString(),
                    CartId = cart.RowKey,
                    ProductId = productId,
                    Quantity = 1
                };
                await _tableService.AddCartItemAsync(item);
            }
        }

        public async Task UpdateQuantityAsync(string cartItemId, int quantity)
        {
            var allItems = await _tableService.GetAllCartItemsAsync();
            var item = allItems.FirstOrDefault(i => i.RowKey == cartItemId);
            if (item == null) throw new Exception("Cart item not found.");

            item.Quantity = quantity;
            await _tableService.UpdateCartItemAsync(item);
        }

        public async Task RemoveItemAsync(string cartItemId)
        {
            var allItems = await _tableService.GetAllCartItemsAsync();
            var item = allItems.FirstOrDefault(i => i.RowKey == cartItemId);
            if (item == null) throw new Exception("Cart item not found.");

            await _tableService.DeleteCartItemAsync(item.PartitionKey, item.RowKey);
        }
        public async Task<Product> GetProductByIdAsync(string productId)
        {
            return await _tableService.GetProductAsync(productId);
        }


        public async Task<string> CheckoutAsync(string userId)
        {
            return await _tableService.ConvertCartToOrderAsync(userId);
        }
    }
}
