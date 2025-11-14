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

        // GET: /Account/Register
        [HttpGet]
        public IActionResult Register()
        {
            return View();
        }

        // POST: /Account/Register
        [HttpPost]
        public async Task<IActionResult> Register(Login model)
        {
            if (!ModelState.IsValid)
                return View(model);

            // Check if username exists
            var existing = await _tableService.GetLoginByUsernameAsync(model.Username);
            if (existing != null)
            {
                ModelState.AddModelError("Username", "Username already taken.");
                return View(model);
            }

            model.PasswordHash = BCrypt.Net.BCrypt.HashPassword(model.Password);
            model.RowKey = Guid.NewGuid().ToString();
            await _tableService.AddLoginAsync(model);

            return RedirectToAction("Login");
        }


        // GET: /Account/Login
        [HttpGet]
        public IActionResult Login()
        {
            return View();
        }

        // POST: /Account/Login
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Login(Login model)
        {
            if (!ModelState.IsValid)
                return View(model);

            var user = await _tableService.GetLoginByUsernameAsync(model.Username);
            if (user == null || !BCrypt.Net.BCrypt.Verify(model.Password, user.PasswordHash))
            {
                ModelState.AddModelError("", "Invalid username or password.");
                return View(model);
            }

            HttpContext.Session.SetString("UserId", user.RowKey);
            HttpContext.Session.SetString("Username", user.Username);
            HttpContext.Session.SetString("UserRole", user.Role);

            return user.Role == "Admin"
                ? RedirectToAction("Index", "Home", new { area = "Admin" })
                : RedirectToAction("Index", "Home", new { area = "Customer" });
        }
    }
}
