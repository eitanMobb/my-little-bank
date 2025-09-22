using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.Cookies;
using System.Security.Claims;
using MyLittleBank.Models;
using MyLittleBank.Services;

namespace MyLittleBank.Controllers
{
    public class AccountController : Controller
    {
        private readonly DatabaseService _databaseService;

        public AccountController(DatabaseService databaseService)
        {
            _databaseService = databaseService;
        }

        [HttpGet]
        public IActionResult Login()
        {
            return View();
        }

        [HttpPost]
        public async Task<IActionResult> Login(string username, string password)
        {
            // Authenticate user with provided credentials
            var user = _databaseService.AuthenticateUser(username, password);

            if (user != null)
            {
                var claims = new List<Claim>
                {
                    new Claim(ClaimTypes.NameIdentifier, user.Id.ToString()),
                    new Claim(ClaimTypes.Name, user.Username),
                    new Claim("FirstName", user.FirstName),
                    new Claim("LastName", user.LastName)
                };

                var claimsIdentity = new ClaimsIdentity(claims, CookieAuthenticationDefaults.AuthenticationScheme);
                var authProperties = new AuthenticationProperties
                {
                    IsPersistent = true,
                    ExpiresUtc = DateTimeOffset.UtcNow.AddMinutes(30)
                };

                await HttpContext.SignInAsync(CookieAuthenticationDefaults.AuthenticationScheme,
                    new ClaimsPrincipal(claimsIdentity), authProperties);

                return RedirectToAction("Dashboard", "Banking");
            }

            ViewBag.ErrorMessage = "Invalid username or password";
            return View();
        }

        [HttpPost]
        public async Task<IActionResult> Logout()
        {
            await HttpContext.SignOutAsync(CookieAuthenticationDefaults.AuthenticationScheme);
            return RedirectToAction("Login");
        }
    }
}
