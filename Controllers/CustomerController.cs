using Microsoft.AspNetCore.Mvc;
using OrderSystem.Models;
using OrderSystem.Services;
using System.Threading.Tasks;

namespace OrderSystem.Controllers
{
    public class CustomerController : Controller
    {
        private readonly TableService _tableService;

        // Initializes the controller with TableService dependency
        public CustomerController(TableService tableService)
        {
            _tableService = tableService;
        }

        // Displays a list of all customers
        public async Task<IActionResult> Index()
        {
            var customers = await _tableService.GetAllCustomersAsync();
            return View(customers);
        }

        // Displays the details of a specific customer by ID
        public async Task<IActionResult> Details(string id)
        {
            if (string.IsNullOrEmpty(id))
                return NotFound();

            var customer = await _tableService.GetCustomerAsync(id);
            if (customer == null)
                return NotFound();

            return View(customer);
        }

        // Shows the form to create a new customer
        public IActionResult Create()
        {
            return View();
        }

        // Handles creating a new customer
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("CustomerName,CustomerEmail")] Customer customer)
        {
            if (ModelState.IsValid)
            {
                await _tableService.AddCustomerAsync(customer);
                return RedirectToAction(nameof(Index));
            }
            return View(customer);
        }

        // Shows the edit form for a customer
        public async Task<IActionResult> Edit(string id)
        {
            if (string.IsNullOrEmpty(id))
                return NotFound();

            var customer = await _tableService.GetCustomerAsync(id);
            if (customer == null)
                return NotFound();

            return View(customer);
        }

        // Handles updating an existing customer
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(string id, [Bind("RowKey,PartitionKey,CustomerName,CustomerEmail,ETag")] Customer customer)
        {
            if (id != customer.RowKey)
                return NotFound();

            if (ModelState.IsValid)
            {
                await _tableService.UpdateCustomerAsync(customer);
                return RedirectToAction(nameof(Index));
            }

            return View(customer);
        }

        // Shows the delete confirmation page for a customer
        public async Task<IActionResult> Delete(string id)
        {
            if (string.IsNullOrEmpty(id))
                return NotFound();

            var customer = await _tableService.GetCustomerAsync(id);
            if (customer == null)
                return NotFound();

            return View(customer);
        }

        // Handles deletion of a customer
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(string id)
        {
            await _tableService.DeleteCustomerAsync(id);
            return RedirectToAction(nameof(Index));
        }
        public async Task<IActionResult> Shop()
        {
            var products = await _tableService.GetAllProductsAsync();
            return View(products);
        }
    }
}
