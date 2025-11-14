using Microsoft.AspNetCore.Mvc;
using OrderSystem.Services.Shared.Models;
namespace OrderSystem.Controllers
{
    public class AdminController : Controller
    {
        // Role check
        private bool IsAdmin() => HttpContext.Session.GetString("Role") == "Admin";

        public IActionResult Dashboard()
        {
            if (!IsAdmin()) return Unauthorized();
            return View();
        }

    }
}
