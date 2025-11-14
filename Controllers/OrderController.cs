using Microsoft.AspNetCore.Mvc;
using OrderSystem.Models;
using OrderSystem.Services;
using System;
using System.Collections.Generic;
using System.Linq;
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
        public async Task<IActionResult> Index()
        {
            var userId = HttpContext.Session.GetString("UserId");
            if (string.IsNullOrEmpty(userId))
                return RedirectToAction("Login", "Account");

            var orders = await GetOrdersForUser(userId);
            return View(orders);
        }

        public async Task<IActionResult> Details(string id)
        {
            if (string.IsNullOrEmpty(id))
                return BadRequest("Order ID is required.");

            var order = await GetOrderById(id);
            if (order == null)
                return NotFound("Order not found.");

            var items = order.ProductId.Split(',')
                        .Select(p =>
                        {
                            var parts = p.Split('x');
                            return new
                            {
                                ProductId = parts[0],
                                Quantity = int.Parse(parts[1])
                            };
                        })
                        .ToList();

            ViewBag.OrderItems = items;
            return View(order);
        }


        private async Task<List<Order>> GetOrdersForUser(string userId)
        {
            var client = new Azure.Data.Tables.TableClient(
                _tableService.GetType()
                    .GetProperty("_connectionString", System.Reflection.BindingFlags.NonPublic | System.Reflection.BindingFlags.Instance)
                    .GetValue(_tableService).ToString(),
                "Order"
            );

            var list = new List<Order>();
            await foreach (var order in client.QueryAsync<Order>(o => o.CustomerId == userId))
                list.Add(order);

            return list;
        }

       
        private async Task<Order> GetOrderById(string orderId)
        {
            var client = new Azure.Data.Tables.TableClient(
                _tableService.GetType()
                    .GetProperty("_connectionString", System.Reflection.BindingFlags.NonPublic | System.Reflection.BindingFlags.Instance)
                    .GetValue(_tableService).ToString(),
                "Order"
            );

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
