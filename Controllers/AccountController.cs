using Microsoft.AspNetCore.Mvc;
using OrderSystem.Services.Shared.Models;
using OrderSystem.Services.Services;
using System.Threading.Tasks;
using BCrypt.Net;

namespace OrderSystem.Controllers
{
    public class AccountController : Controller
    {
        private readonly TableService _tableService;

        public AccountController(TableService tableService)
        {
            _tableService = tableService;
        }

        [HttpGet]
        public IActionResult Register()
        {
            return View(new RegisterModel());
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Register(RegisterModel model)
        {
            if (!ModelState.IsValid) return View(model);

            var existing = await _tableService.GetLoginByUsernameAsync(model.Username);
            if (existing != null)
            {
                ModelState.AddModelError("Username", "Username already taken.");
                return View(model);
            }

            var login = new Login
            {
                Username = model.Username.Trim().ToLower(),
                PasswordHash = BCrypt.Net.BCrypt.HashPassword(model.Password),
                Role = "Customer"
            };
            await _tableService.AddLoginAsync(login);

            
            var customer = new Customer
            {
                PartitionKey = "Customer",
                RowKey = login.RowKey, 
                CustomerName = model.Username,  
                CustomerEmail = $"{model.Username}@example.com" 
            };
            await _tableService.AddCustomerAsync(customer);

            TempData["Success"] = "Registration successful! Please login.";
            return RedirectToAction("Login");
        }

        [HttpGet]
        public IActionResult Login()
        {
            return View(new LoginModel());
        }
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Login(LoginModel model)
        {
            if (!ModelState.IsValid) return View(model);

            var user = await _tableService.GetLoginByUsernameAsync(model.Username);
            if (user == null || !BCrypt.Net.BCrypt.Verify(model.Password, user.PasswordHash))
            {
                ModelState.AddModelError("", "Invalid username or password.");
                return View(model);
            }

            HttpContext.Session.SetString("UserId", user.RowKey);
            HttpContext.Session.SetString("Username", user.Username);
            HttpContext.Session.SetString("UserRole", user.Role);

            return RedirectToAction("Index", "Home");
        }
        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult Logout()
        {
            HttpContext.Session.Clear();
            return RedirectToAction("Login");
        }
    }
}