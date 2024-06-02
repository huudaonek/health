using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Http;
using System.Security.Claims;

namespace insure_fixlast.Controllers
{
    public class AdminsController : Controller
    {
        public IActionResult Index()
        {
            if (!User.Identity.IsAuthenticated)
            {
                return RedirectToAction("Login", "Accounts");
            }

            var roleClaim = User.FindFirst(ClaimTypes.Role)?.Value;
            if (roleClaim != "Admin")
            {
                return RedirectToAction("AccessDenied", "Accounts");
            }

            return View();
        }
    }
}
