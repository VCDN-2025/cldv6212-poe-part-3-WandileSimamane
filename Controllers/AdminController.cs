using Microsoft.AspNetCore.Mvc;
using OrderSystem.Services.Shared.Models;

namespace OrderSystem.Controllers
{
    public class AdminController : Controller
    {
        private bool IsAdmin() => HttpContext.Session.GetString("UserRole") == "Admin";

        public IActionResult Dashboard()
        {
            if (!IsAdmin()) return Unauthorized();
            return View();
        }
    }
}