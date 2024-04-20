using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using CourseWork_WebStore.Models;
using CourseWork_WebStore.ViewModels;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.Cookies;
using System.Security.Claims;

namespace CourseWork_WebStore.Controllers
{
    public class AccountController : Controller
    {
        private readonly StoreDbContext _context;

        public AccountController(StoreDbContext context)
        {
            _context = context;
        }

        public IActionResult Register()
        {
            var model = new RegisterViewModel();
            return View(model);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Register(RegisterViewModel model)
        {
            if (ModelState.IsValid)
            {
                if (UsernameExists(model.Username) || EmailExists(model.Email))
                {
                    ModelState.AddModelError(string.Empty, "Username or email already exists.");
                    return View(model);
                }
                var user = new User
                {
                    Username = model.Username,
                    Email = model.Email,
                    PasswordHash = model.Password,
                    FirstName = model.FirstName,
                    LastName = model.LastName,
                    Role = model.Role
                };
                _context.Users.Add(user);
                await _context.SaveChangesAsync();
                return RedirectToAction("Login", "Account");
            }
            return View(model);
        }


        public IActionResult Login()
        {
            var model = new LoginViewModel();
            return View(model);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Login(LoginViewModel model)
        {
            if (ModelState.IsValid)
            {
                var user = _context.Users.FirstOrDefault(u => u.PasswordHash == model.Password);

                if (user != null)
                {
                    var claims = new List<Claim>
                {
                new Claim(ClaimTypes.Name, user.Username)
                };

                    var claimsIdentity = new ClaimsIdentity(claims, CookieAuthenticationDefaults.AuthenticationScheme);
                    var authProperties = new AuthenticationProperties
                    {
                        IsPersistent = model.RememberMe
                    };
                    await HttpContext.SignInAsync(
                        CookieAuthenticationDefaults.AuthenticationScheme,
                        new ClaimsPrincipal(claimsIdentity),
                        authProperties);

                    TempData["UserName"] = user.Username;
                    return RedirectToAction("Index", "Home");
                }
                else
                {
                    ModelState.AddModelError(string.Empty, "Invalid username or password.");
                }
            }
            return View(model);
        }
        private bool UsernameExists(string username)
        {
            return _context.Users.Any(u => u.Username == username);
        }
        private bool EmailExists(string email)
        {
            return _context.Users.Any(u => u.Email == email);
        }
        public IActionResult Logout()
        {
            HttpContext.SignOutAsync(CookieAuthenticationDefaults.AuthenticationScheme);

            return RedirectToAction("Index", "Home");
        }

    }
}
