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
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Register(Login model)
        {
            if (!ModelState.IsValid)
                return View(model);

            model.PasswordHash = BCrypt.Net.BCrypt.HashPassword(model.Password);

            await _tableService.AddLoginAsync(model); // this saves to Azure Table

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
        public async Task<IActionResult> Login(string username, string password)
        {
            var user = await _tableService.GetLoginByUsernameAsync(username);
            if (user == null || !BCrypt.Net.BCrypt.Verify(password, user.PasswordHash))
            {
                ModelState.AddModelError("", "Invalid username or password.");
                return View();
            }

            // Set session
            HttpContext.Session.SetString("UserId", user.RowKey);
            HttpContext.Session.SetString("Role", user.Role);

            if (user.Role == "Admin")
                return RedirectToAction("Index", "Home", new { area = "Admin" });
            else
                return RedirectToAction("Index", "Home", new { area = "Customer" });
        }
    }
}
