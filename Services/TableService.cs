using Azure;
using Azure.Data.Tables;
using Microsoft.Extensions.Configuration;
using OrderSystem.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace OrderSystem.Services
{
    public class TableService
    {
        private readonly string _connectionString;

        public TableService(IConfiguration configuration)
        {
            _connectionString = configuration.GetConnectionString("AzureTableStorage");

            if (string.IsNullOrEmpty(_connectionString))
                throw new InvalidOperationException("Azure Table Storage connection string is not configured.");
        }

        // Customer 
        public async Task<Customer> GetCustomerAsync(string id)
        {
            var client = new TableClient(_connectionString, "Customer");
            try { return (await client.GetEntityAsync<Customer>("Customer", id)).Value; }
            catch { return null; }
        }

        public async Task<List<Customer>> GetAllCustomersAsync()
        {
            var client = new TableClient(_connectionString, "Customer");
            var list = new List<Customer>();
            await foreach (var c in client.QueryAsync<Customer>()) list.Add(c);
            return list;
        }

        public async Task AddCustomerAsync(Customer customer)
        {
            var client = new TableClient(_connectionString, "Customer");
            await client.CreateIfNotExistsAsync();
            await client.AddEntityAsync(customer);
        }

        public async Task UpdateCustomerAsync(Customer customer)
        {
            var client = new TableClient(_connectionString, "Customer");
            await client.UpdateEntityAsync(customer, ETag.All, TableUpdateMode.Replace);
        }

        public async Task DeleteCustomerAsync(string id)
        {
            var client = new TableClient(_connectionString, "Customer");
            await client.DeleteEntityAsync("Customer", id);
        }

        // Product 
        public async Task<Product> GetProductAsync(string id)
        {
            var client = new TableClient(_connectionString, "Product");
            try { return (await client.GetEntityAsync<Product>("Product", id)).Value; }
            catch { return null; }
        }

        public async Task<List<Product>> GetAllProductsAsync()
        {
            var client = new TableClient(_connectionString, "Product");
            var list = new List<Product>();
            await foreach (var p in client.QueryAsync<Product>()) list.Add(p);
            return list;
        }

        public async Task AddProductAsync(Product product)
        {
            var client = new TableClient(_connectionString, "Product");
            await client.CreateIfNotExistsAsync();
            await client.AddEntityAsync(product);
        }

        public async Task UpdateProductAsync(Product product)
        {
            var client = new TableClient(_connectionString, "Product");
            await client.UpdateEntityAsync(product, ETag.All, TableUpdateMode.Replace);
        }

        public async Task DeleteProductAsync(string id)
        {
            var client = new TableClient(_connectionString, "Product");
            await client.DeleteEntityAsync("Product", id);
        }

        // Cart & Cart Items 
        public async Task<CartEntity> GetCartAsync(string userId)
        {
            var client = new TableClient(_connectionString, "Cart");
            await client.CreateIfNotExistsAsync();

            await foreach (var c in client.QueryAsync<CartEntity>(x => x.UserId == userId))
                return c;

            var newCart = new CartEntity { UserId = userId, PartitionKey = "Cart", RowKey = Guid.NewGuid().ToString() };
            await client.AddEntityAsync(newCart);
            return newCart;
        }

        public async Task<List<CartItem>> GetCartItemsAsync(string cartId)
        {
            var client = new TableClient(_connectionString, "CartItem");
            await client.CreateIfNotExistsAsync();

            var list = new List<CartItem>();
            await foreach (var item in client.QueryAsync<CartItem>(x => x.PartitionKey == cartId))
                list.Add(item);

            return list;
        }

        public async Task<List<CartItem>> GetAllCartItemsAsync()
        {
            var client = new TableClient(_connectionString, "CartItem");
            var items = new List<CartItem>();
            await foreach (var item in client.QueryAsync<CartItem>())
                items.Add(item);
            return items;
        }

        public async Task AddCartItemAsync(CartItem item)
        {
            var client = new TableClient(_connectionString, "CartItem");
            await client.CreateIfNotExistsAsync();
            await client.AddEntityAsync(item);
        }

        public async Task UpdateCartItemAsync(CartItem item)
        {
            var client = new TableClient(_connectionString, "CartItem");
            await client.UpdateEntityAsync(item, ETag.All, TableUpdateMode.Replace);
        }

        public async Task DeleteCartItemAsync(string cartId, string cartItemId)
        {
            var client = new TableClient(_connectionString, "CartItem");
            await client.DeleteEntityAsync(cartId, cartItemId);
        }

        //  Checkout: Convert Cart to Order 
        public async Task<string> ConvertCartToOrderAsync(string userId)
        {
            var cart = await GetCartAsync(userId);
            var items = await GetCartItemsAsync(cart.RowKey);

            if (!items.Any())
                throw new InvalidOperationException("Cart is empty.");

            var orderTable = new TableClient(_connectionString, "Order");
            await orderTable.CreateIfNotExistsAsync();

            var orderId = Guid.NewGuid().ToString();
            var order = new Order
            {
                PartitionKey = "Order",
                RowKey = orderId,
                CustomerId = userId,
                ProductId = string.Join(",", items.Select(i => $"{i.ProductId}x{i.Quantity}")),
                OrderDate = DateTimeOffset.UtcNow
            };

            await orderTable.AddEntityAsync(order);

            foreach (var i in items)
                await DeleteCartItemAsync(cart.RowKey, i.RowKey);

            return orderId;
        }

        //  Orders 
        public async Task AddOrderAsync(Order order)
        {
            var client = new TableClient(_connectionString, "Order");
            await client.CreateIfNotExistsAsync();
            await client.AddEntityAsync(order);
        }
    }
}
