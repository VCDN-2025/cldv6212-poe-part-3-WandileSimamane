using Microsoft.AspNetCore.Mvc;
using OrderSystem.Services.Shared.Models;
using OrderSystem.Services.Services;
using System.Threading.Tasks;

namespace OrderSystem.Controllers
{
    public class CustomerController : Controller
    {
        private readonly TableService _tableService;

        public CustomerController(TableService tableService)
        {
            _tableService = tableService;
        }

        // GET: /Customer
        public async Task<IActionResult> Index()
        {
            if (HttpContext.Session.GetString("UserRole") != "Admin")
                return Unauthorized();

            var customers = await _tableService.GetAllCustomersAsync();
            return View(customers);
        }

        // GET: /Customer/Details/5
        public async Task<IActionResult> Details(string id)
        {
            if (HttpContext.Session.GetString("UserRole") != "Admin")
                return Unauthorized();

            if (string.IsNullOrEmpty(id)) return NotFound();
            var customer = await _tableService.GetCustomerAsync(id);
            if (customer == null) return NotFound();
            return View(customer);
        }

        // GET: /Customer/Edit/5
        public async Task<IActionResult> Edit(string id)
        {
            if (HttpContext.Session.GetString("UserRole") != "Admin")
                return Unauthorized();

            if (string.IsNullOrEmpty(id)) return NotFound();
            var customer = await _tableService.GetCustomerAsync(id);
            if (customer == null) return NotFound();
            return View(customer);
        }

        // POST: /Customer/Edit/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(string id, Customer customer)
        {
            if (HttpContext.Session.GetString("UserRole") != "Admin")
                return Unauthorized();

            if (id != customer.RowKey) return NotFound();
            if (!ModelState.IsValid) return View(customer);

            await _tableService.UpdateCustomerAsync(customer);
            TempData["Success"] = "Customer updated.";
            return RedirectToAction(nameof(Index));
        }

        // GET: /Customer/Delete/5
        public async Task<IActionResult> Delete(string id)
        {
            if (HttpContext.Session.GetString("UserRole") != "Admin")
                return Unauthorized();

            if (string.IsNullOrEmpty(id)) return NotFound();
            var customer = await _tableService.GetCustomerAsync(id);
            if (customer == null) return NotFound();
            return View(customer);
        }

        // POST: /Customer/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(string id)
        {
            if (HttpContext.Session.GetString("UserRole") != "Admin")
                return Unauthorized();

            await _tableService.DeleteCustomerAsync(id);
            TempData["Success"] = "Customer deleted.";
            return RedirectToAction(nameof(Index));
        }
    }
}