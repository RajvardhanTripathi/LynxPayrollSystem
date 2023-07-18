using Lib.Repository;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;

namespace Web.Controllers
{
	public class LoginController : Controller
	{
		private readonly EmployeeMasRepository _employeeMasRepository;

		public LoginController(EmployeeMasRepository employeeMasRepository)
		{
			_employeeMasRepository = employeeMasRepository;
		}

        [HttpGet]
        public IActionResult Index()
        {
            return View();
        }

        [HttpPost]
        public async Task<IActionResult> Index(string empId, string password)
        {
            if (_employeeMasRepository.PerformLogin(empId, password))
            {
                // Get the role for the logged-in user
                int role = _employeeMasRepository.GetRole(empId);

                // Create claims for authentication
                var claims = new List<Claim>
                {
                    new Claim(ClaimTypes.Name, empId),
                    new Claim(ClaimTypes.Role, role.ToString())
                };

                // Create the identity for the authenticated user
                var identity = new ClaimsIdentity(claims, CookieAuthenticationDefaults.AuthenticationScheme);

                // Sign in the user
                var principal = new ClaimsPrincipal(identity);
                await HttpContext.SignInAsync(CookieAuthenticationDefaults.AuthenticationScheme, principal);

                // Redirect based on the role
                if (role == 1)
                {
                    // Role is 1 (HR), redirect to the Index action of the HomeController
                    return RedirectToAction("Index", "Home");
                }
                else if (role == 2)
                {
                    // Role is 2 (Employee), redirect to the Index action of the EmployeeHomeController
                    return RedirectToAction("Index", "EmployeeHome");
                }
                else
                {
                    // Invalid role, display an error message
                    ViewBag.ErrorMessage = "Invalid role.";
                    return View();
                }
            }
            else
            {
                // Login failed, display an error message
                ViewBag.ErrorMessage = "Invalid empId or password.";
                return View();
            }
        }

        public async Task<IActionResult> Logout()
        {
            // Sign out the user
            await HttpContext.SignOutAsync(CookieAuthenticationDefaults.AuthenticationScheme);

            // Redirect to the login page or any other page
            return RedirectToAction("Index", "Login");
        }
    }
}
