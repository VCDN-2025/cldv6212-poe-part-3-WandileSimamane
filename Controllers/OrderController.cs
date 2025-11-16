using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using OrderSystem.Services.Services;
using OrderSystem.Services.Shared.Models;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace OrderSystem.Controllers
{
    public class OrderController : Controller
    {
        private readonly TableService _tableService;

        public OrderController(TableService tableService)
        {
            _tableService = tableService;
        }
        // GET: /Order/Create
        [HttpGet]
        public async Task<IActionResult> Create()
        {
            if (HttpContext.Session.GetString("UserRole") != "Admin") return Unauthorized();

            // Load customers for dropdown
            var customers = await _tableService.GetAllCustomersAsync();
            ViewBag.Customers = customers.Select(c => new SelectListItem
            {
                Value = c.RowKey,
                Text = $"{c.CustomerName} ({c.CustomerEmail})"
            }).ToList();

            // Load products for selection
            var products = await _tableService.GetAllProductsAsync();
            ViewBag.Products = products.Select(p => new SelectListItem
            {
                Value = p.RowKey,
                Text = $"{p.ProductName} - R{p.Price} (Stock: {p.Stock})"
            }).ToList();

            return View(new Order());
        }

        // POST: /Order/Create
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(Order order, string[] selectedProductIds, int[] quantities)
        {
            if (HttpContext.Session.GetString("UserRole") != "Admin") return Unauthorized();

            if (selectedProductIds == null || selectedProductIds.Length == 0)
            {
                ModelState.AddModelError("", "Select at least one product.");
            }

            if (!ModelState.IsValid)
            {
                // Reload dropdowns
                var customers = await _tableService.GetAllCustomersAsync();
                ViewBag.Customers = customers.Select(c => new SelectListItem
                {
                    Value = c.RowKey,
                    Text = $"{c.CustomerName} ({c.CustomerEmail})"
                }).ToList();

                var products = await _tableService.GetAllProductsAsync();
                ViewBag.Products = products.Select(p => new SelectListItem
                {
                    Value = p.RowKey,
                    Text = $"{p.ProductName} - R{p.Price} (Stock: {p.Stock})"
                }).ToList();

                return View(order);
            }

            // Build order items
            var orderItems = new List<OrderItem>();
            decimal total = 0;

            for (int i = 0; i < selectedProductIds.Length; i++)
            {
                var productId = selectedProductIds[i];
                var qty = quantities[i];

                if (qty <= 0) continue;

                var product = await _tableService.GetProductAsync(productId);
                if (product == null) continue;

                orderItems.Add(new OrderItem
                {
                    ProductId = productId,
                    ProductName = product.ProductName,
                    Quantity = qty,
                    Price = product.Price
                });

                total += product.Price * qty;

                // Reduce stock
                product.Stock -= qty;
                await _tableService.UpdateProductAsync(product);
            }

            if (!orderItems.Any())
            {
                ModelState.AddModelError("", "No valid items in order.");
                return View(order);
            }

            // Finalize order
            order.RowKey = Guid.NewGuid().ToString();
            order.PartitionKey = "Order";
            order.OrderDate = DateTimeOffset.UtcNow;
            order.Status = "Pending";
            order.TotalAmount = total;
            order.Items = orderItems;

            await _tableService.AddOrderAsync(order);

            TempData["Success"] = $"Order created manually! ID: {order.RowKey}";
            return RedirectToAction(nameof(Index));
        }
        public async Task<IActionResult> Index()
        {
            var userRole = HttpContext.Session.GetString("UserRole");
            if (userRole == "Admin")
            {
                var orders = await GetAllOrdersAsync();
                return View(orders);
            }
            else if (userRole == "Customer")
            {
                var userId = HttpContext.Session.GetString("UserId");
                if (userId == null) return Unauthorized();
                var orders = await GetOrdersForUser(userId);
                return View("MyOrders", orders);
            }
            return Unauthorized();
        }

        public async Task<IActionResult> Details(string id)
        {
            if (string.IsNullOrEmpty(id)) return BadRequest("Order ID required.");

            var order = await GetOrderById(id);
            if (order == null) return NotFound("Order not found.");

            return View(order);
        }

        [HttpPost]
        public async Task<IActionResult> UpdateStatus(string id, string status)
        {
            var order = await GetOrderById(id);
            if (order == null) return NotFound();
            order.Status = status;
            await _tableService.UpdateOrderAsync(order);
            return RedirectToAction("Index");
        }

        private async Task<List<Order>> GetOrdersForUser(string userId)
        {
            var client = _tableService.GetTableClient("Order");
            var list = new List<Order>();
            await foreach (var order in client.QueryAsync<Order>(o => o.CustomerId == userId))
            {
                list.Add(order);
            }
            return list;
        }

        private async Task<List<Order>> GetAllOrdersAsync()
        {
            var client = _tableService.GetTableClient("Order");
            var list = new List<Order>();
            await foreach (var order in client.QueryAsync<Order>())
            {
                list.Add(order);
            }
            return list;
        }

        private async Task<Order?> GetOrderById(string orderId)
        {
            var client = _tableService.GetTableClient("Order");
            try
            {
                var response = await client.GetEntityAsync<Order>("Order", orderId);
                return response.Value;
            }
            catch
            {
                return null;
            }
        }
    }
}