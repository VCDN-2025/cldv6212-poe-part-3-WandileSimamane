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
            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Register(Login model)
        {
            if (!ModelState.IsValid)
                return View(model);

            model.Username = model.Username.Trim().ToLower(); // normalize

            try
            {
                // check if username exists
                var existing = await _tableService.GetLoginByUsernameAsync(model.Username);
                if (existing != null)
                {
                    ModelState.AddModelError("Username", "Username already taken.");
                    return View(model);
                }

                model.PasswordHash = BCrypt.Net.BCrypt.HashPassword(model.Password);
                model.Password = null;  // Clear plain text for security
                model.RowKey = Guid.NewGuid().ToString();
                await _tableService.AddLoginAsync(model);

                return RedirectToAction("Login");
            }
            catch (Exception ex)
            {
                // Handle Azure connection or other errors gracefully
                ModelState.AddModelError("", "Registration failed. Please check your connection and try again.");
                return View(model);
            }
        }

 
        [HttpGet]
        public IActionResult Login()
        {
            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Login(Login model)
        {
            if (!ModelState.IsValid)
                return View(model);

            model.Username = model.Username.Trim().ToLower(); // normalize

            try
            {
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
            catch (Exception ex)
            {
                // Handle Azure connection or other errors gracefully
                ModelState.AddModelError("", "Login failed. Please check your connection and try again.");
                return View(model);
            }
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