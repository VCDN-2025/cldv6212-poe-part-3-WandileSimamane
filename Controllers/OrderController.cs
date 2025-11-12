using System;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using OrderSystem.Models;
using OrderSystem.Services;

namespace OrderSystem.Controllers
{
    public class OrderController : Controller
    {
        private readonly TableService _tableService;
        private readonly QueueService _queueService;

        // Initializes the controller with TableService and QueueService dependencies
        public OrderController(TableService tableService, QueueService queueService)
        {
            _tableService = tableService;
            _queueService = queueService;
        }

        // Displays a list of all orders
        public async Task<IActionResult> Index()
        {
            var orders = await _tableService.GetAllOrdersAsync();
            return View(orders);
        }

        // Displays details for a single order
        public async Task<IActionResult> Details(string id)
        {
            if (string.IsNullOrEmpty(id))
                return NotFound();

            var order = await _tableService.GetOrderAsync(id);
            if (order == null)
                return NotFound();

            return View(order);
        }

        // Shows the create order form
        public IActionResult Create()
        {
            return View();
        }

        // Handles creating a new order and sending a message to the queue
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(Order order)
        {
            if (ModelState.IsValid)
            {
                // Set the OrderDate to the current UTC time
                order.OrderDate = DateTime.UtcNow;

                // Add order to Table Storage
                await _tableService.AddOrderAsync(order);

                // Send message to queue for further processing
                var message = $"Processing new order with ID: {order.RowKey}";
                await _queueService.SendMessageAsync("order-processing", message);

                return RedirectToAction(nameof(Index));
            }

            return View(order);
        }

        // Shows the edit order form
        public async Task<IActionResult> Edit(string id)
        {
            if (string.IsNullOrEmpty(id))
                return NotFound();

            var order = await _tableService.GetOrderAsync(id);
            if (order == null)
                return NotFound();

            return View(order);
        }

        // Handles updating an existing order
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(string id, Order order)
        {
            if (id != order.RowKey)
                return NotFound();

            if (ModelState.IsValid)
            {
              
                order.OrderDate = order.OrderDate.ToUniversalTime();

                await _tableService.UpdateOrderAsync(order);
                return RedirectToAction(nameof(Index));
            }

            return View(order);
        }


        // Shows the delete confirmation page for an order
        public async Task<IActionResult> Delete(string id)
        {
            if (string.IsNullOrEmpty(id))
                return NotFound();

            var order = await _tableService.GetOrderAsync(id);
            if (order == null)
                return NotFound();

            return View(order);
        }

        // Handles deletion of an order
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(string id)
        {
            await _tableService.DeleteOrderAsync(id);
            return RedirectToAction(nameof(Index));
        }
    }
}
