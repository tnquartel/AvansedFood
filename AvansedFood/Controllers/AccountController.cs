using AvansedFood.Web.ViewModels;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;

namespace AvansedFood.Web.Controllers
{
    public class AccountController : Controller
    {
        private readonly SignInManager<IdentityUser> _signInManager;
        private readonly UserManager<IdentityUser> _userManager;

        public AccountController(
            SignInManager<IdentityUser> signInManager,
            UserManager<IdentityUser> userManager)
        {
            _signInManager = signInManager;
            _userManager = userManager;
        }

        // GET: Account/Login
        [AllowAnonymous]
        public IActionResult Login(string? returnUrl = null)
        {
            ViewData["ReturnUrl"] = returnUrl;
            return View();
        }

        // POST: Account/Login
        [HttpPost]
        [AllowAnonymous]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Login(string email, string password, string? returnUrl = null)
        {
            ViewData["ReturnUrl"] = returnUrl;

            if (string.IsNullOrEmpty(email) || string.IsNullOrEmpty(password))
            {
                TempData["ErrorMessage"] = "Email en wachtwoord zijn verplicht";
                return View();
            }

            // Find user by email
            var user = await _userManager.FindByEmailAsync(email);

            if (user == null)
            {
                TempData["ErrorMessage"] = "Onbekende gebruiker";
                return View();
            }

            // Sign in
            var result = await _signInManager.PasswordSignInAsync(
                user.UserName!,
                password,
                isPersistent: false,
                lockoutOnFailure: false);

            if (result.Succeeded)
            {
                // Check user role and redirect accordingly
                var roles = await _userManager.GetRolesAsync(user);

                if (roles.Contains("Student"))
                {
                    return RedirectToAction("Index", "Package");
                }
                else if (roles.Contains("CanteenEmployee"))
                {
                    return RedirectToAction("Index", "Canteen");
                }

                // Default redirect if no specific role
                return RedirectToLocal(returnUrl);
            }
            else
            {
                TempData["ErrorMessage"] = "Ongeldige inloggegevens";
                return View();
            }
        }

        // POST: Account/Logout
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Logout()
        {
            await _signInManager.SignOutAsync();
            return RedirectToAction("Login", "Account");
        }

        // GET: Account/UserDetail
        [Authorize]
        public async Task<IActionResult> UserDetail()
        {
            var user = await _userManager.GetUserAsync(User);

            if (user == null)
            {
                return RedirectToAction("Login");
            }

            var roles = await _userManager.GetRolesAsync(user);
            var role = roles.FirstOrDefault() ?? "Unknown";

            var viewModel = new UserDetailViewModel
            {
                Email = user.Email ?? "",
                Role = role
            };

            return View(viewModel);
        }

        private IActionResult RedirectToLocal(string? returnUrl)
        {
            if (!string.IsNullOrEmpty(returnUrl) && Url.IsLocalUrl(returnUrl))
            {
                return Redirect(returnUrl);
            }
            else
            {
                return RedirectToAction("Index", "Home");
            }
        }
    }
}