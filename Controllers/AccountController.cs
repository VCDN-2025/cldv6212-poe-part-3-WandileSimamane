using BCrypt.Net;
using Microsoft.AspNetCore.Mvc;
using Microsoft.CodeAnalysis.Scripting;
using OrderSystem.Models;
using OrderSystem.Services;

namespace OrderSystem.Controllers
{
    public class AccountController : Controller
    {
        private readonly TableService _tableService;
        public AccountController(TableService tableService)
        {
            _tableService = tableService;
        }

        // Registration
        public IActionResult Register() => View();

        [HttpPost]
        public async Task<IActionResult> Register(string username, string password)
        {
            var existing = await _tableService.GetUserByUsernameAsync(username);
            if (existing != null)
            {
                ModelState.AddModelError("", "Username already exists.");
                return View();
            }

            var user = new User
            {
                Username = username,
                PasswordHash = BCrypt.Net.BCrypt.HashPassword(password),
                Role = "Customer"
            };

            await _tableService.AddUserAsync(user);

            HttpContext.Session.SetString("UserId", user.RowKey);
            HttpContext.Session.SetString("Role", user.Role);

            return RedirectToAction("Index", "Home");
        }

        // Login
        public IActionResult Login() => View();

        [HttpPost]
        public async Task<IActionResult> Login(string username, string password)
        {
            var user = await _tableService.GetUserByUsernameAsync(username);
            if (user == null || !BCrypt.Net.BCrypt.Verify(password, user.PasswordHash))
            {
                ModelState.AddModelError("", "Invalid credentials.");
                return View();
            }

            HttpContext.Session.SetString("UserId", user.RowKey);
            HttpContext.Session.SetString("Role", user.Role);

            if (user.Role == "Admin")
                return RedirectToAction("Dashboard", "Admin");
            else
                return RedirectToAction("Shop", "Customer");
        }

        public IActionResult Logout()
        {
            HttpContext.Session.Clear();
            return RedirectToAction("Login");
        }
    }
}
