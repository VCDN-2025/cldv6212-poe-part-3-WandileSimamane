using Azure;
using Azure.Data.Tables;
using OrderSystem.Models;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace OrderSystem.Services
{
    public class TableService
    {
        private readonly string _connectionString;

        // Initializes TableService with Azure Table Storage connection string
        public TableService(IConfiguration configuration)
        {
            _connectionString = configuration.GetConnectionString("AzureTableStorage");

            if (string.IsNullOrEmpty(_connectionString))
            {
                throw new InvalidOperationException("Azure Table Storage connection string is not configured.");
            }
        }

        // ----------------- Customer Methods -----------------

        // Retrieves a single customer by ID
        public async Task<Customer> GetCustomerAsync(string customerId)
        {
            var tableClient = new TableClient(_connectionString, "Customer");
            try
            {
                var customer = await tableClient.GetEntityAsync<Customer>("Customer", customerId);
                return customer.Value;
            }
            catch (Azure.RequestFailedException)
            {
                return null;
            }
        }

        // Retrieves all customers
        public async Task<List<Customer>> GetAllCustomersAsync()
        {
            var tableClient = GetCustomerTableClient();
            var customers = new List<Customer>();
            await foreach (var customer in tableClient.QueryAsync<Customer>())
            {
                customers.Add(customer);
            }
            return customers;
        }

        // Adds a new customer
        public async Task AddCustomerAsync(Customer customer)
        {
            var tableClient = new TableClient(_connectionString, "Customer");
            await tableClient.AddEntityAsync(customer);
        }

        // Updates an existing customer
        public async Task UpdateCustomerAsync(Customer customer)
        {
            var tableClient = new TableClient(_connectionString, "Customer");
            await tableClient.UpdateEntityAsync(customer, ETag.All, TableUpdateMode.Replace);
        }

        // Deletes a customer by ID
        public async Task DeleteCustomerAsync(string customerId)
        {
            var tableClient = new TableClient(_connectionString, "Customer");
            await tableClient.DeleteEntityAsync("Customer", customerId);
        }

        // Ensures the Customer table exists and returns its client
        private TableClient GetCustomerTableClient()
        {
            var tableClient = new TableClient(_connectionString, "Customer");
            tableClient.CreateIfNotExists();
            return tableClient;
        }

        // ----------------- Product Methods -----------------

        // Retrieves a single product by ID
        public async Task<Product> GetProductAsync(string productId)
        {
            var tableClient = new TableClient(_connectionString, "Product");
            try
            {
                var product = await tableClient.GetEntityAsync<Product>("Product", productId);
                return product.Value;
            }
            catch (Azure.RequestFailedException)
            {
                return null;
            }
        }

        // Retrieves all products
        public async Task<List<Product>> GetAllProductsAsync()
        {
            var tableClient = GetProductTableClient();
            var products = new List<Product>();
            await foreach (var product in tableClient.QueryAsync<Product>())
            {
                products.Add(product);
            }
            return products;
        }

        // Ensures the Product table exists and returns its client
        private TableClient GetProductTableClient()
        {
            var tableClient = new TableClient(_connectionString, "Product");
            tableClient.CreateIfNotExists();
            return tableClient;
        }

        // Adds a new product
        public async Task AddProductAsync(Product product)
        {
            var tableClient = new TableClient(_connectionString, "Product");
            await tableClient.AddEntityAsync(product);
        }

        // Updates an existing product
        public async Task UpdateProductAsync(Product product)
        {
            var tableClient = new TableClient(_connectionString, "Product");
            await tableClient.UpdateEntityAsync(product, ETag.All, TableUpdateMode.Replace);
        }

        // Deletes a product by ID
        public async Task DeleteProductAsync(string productId)
        {
            var tableClient = new TableClient(_connectionString, "Product");
            await tableClient.DeleteEntityAsync("Product", productId);
        }

        // ----------------- Order Methods -----------------

        // Retrieves a single order by ID
        public async Task<Order> GetOrderAsync(string orderId)
        {
            var tableClient = new TableClient(_connectionString, "Order");
            try
            {
                var order = await tableClient.GetEntityAsync<Order>("Order", orderId);
                return order.Value;
            }
            catch (Azure.RequestFailedException)
            {
                return null;
            }
        }

        // Retrieves all orders
        public async Task<List<Order>> GetAllOrdersAsync()
        {
            var tableClient = GetOrderTableClient();
            var orders = new List<Order>();
            await foreach (var order in tableClient.QueryAsync<Order>())
            {
                orders.Add(order);
            }
            return orders;
        }

        // Adds a new order
        public async Task AddOrderAsync(Order order)
        {
            var tableClient = new TableClient(_connectionString, "Order");
            await tableClient.CreateIfNotExistsAsync();
            await tableClient.AddEntityAsync(order);
        }

        // Updates an existing order
        public async Task UpdateOrderAsync(Order order)
        {
            var tableClient = new TableClient(_connectionString, "Order");
            await tableClient.UpdateEntityAsync(order, ETag.All, TableUpdateMode.Replace);
        }

        // Deletes an order by ID
        public async Task DeleteOrderAsync(string orderId)
        {
            var tableClient = new TableClient(_connectionString, "Order");
            await tableClient.DeleteEntityAsync("Order", orderId);
        }

        // Ensures the Order table exists and returns its client
        private TableClient GetOrderTableClient()
        {
            var tableClient = new TableClient(_connectionString, "Order");
            tableClient.CreateIfNotExists();
            return tableClient;
        }
    }
}
