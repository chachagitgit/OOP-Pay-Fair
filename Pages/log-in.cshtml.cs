using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using OOP_Fair_Fare.Models;
using System.ComponentModel.DataAnnotations;
using System.Security.Cryptography;
using System.Text;
using System.Linq;
using Microsoft.EntityFrameworkCore;
using System.Security.Claims;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.Cookies;

namespace OOP_Fair_Fare.Pages
{
    public class logInModel : PageModel
    {
        private readonly AppDbContext _db;
        public logInModel(AppDbContext db) { _db = db; }

        [BindProperty]
        public InputModel Input { get; set; } = new InputModel();

        public class InputModel
        {
            [Required]
            public string? EmailOrUsername { get; set; }
            [Required]
            public string? Password { get; set; }
        }

        public void OnGet() { }
        public async Task<IActionResult> OnPostAsync()
        {
            if (!ModelState.IsValid)
            {
                ModelState.AddModelError(string.Empty, "Please fill in all fields.");
                return Page();
            }

            var user = await _db.Users
                .Where(u => !u.IsDeleted)
                .FirstOrDefaultAsync(u =>
                    u.Email == Input.EmailOrUsername || u.Username == Input.EmailOrUsername);

            if (user == null)
            {
                ModelState.AddModelError(string.Empty, "Account not found.");
                return Page();
            }

            string hashed = HashPassword(Input.Password!);
            if (user.HashedPassword != hashed)
            {
                ModelState.AddModelError(string.Empty, "Wrong Password, try again.");
                return Page();
            }            // Check user role
            var role = await _db.Roles.FirstOrDefaultAsync(r => r.UserId == user.Id);
            if (role == null)
            {
                ModelState.AddModelError(string.Empty, "User role not found.");
                return Page();
            }

            // Create claims for the user
            var claims = new List<Claim>
            {
                new Claim(ClaimTypes.NameIdentifier, user.Id.ToString()),
                new Claim(ClaimTypes.Name, user.Username),
                new Claim(ClaimTypes.Email, user.Email),
                new Claim(ClaimTypes.Role, role.RoleName)
            };

            // Create claims identity
            var claimsIdentity = new ClaimsIdentity(claims, CookieAuthenticationDefaults.AuthenticationScheme);

            // Create claims principal
            var claimsPrincipal = new ClaimsPrincipal(claimsIdentity);

            // Set authentication properties
            var authProperties = new AuthenticationProperties
            {
                IsPersistent = true, // Remember the login
                ExpiresUtc = DateTimeOffset.UtcNow.AddHours(24)
            };

            // Sign in the user
            await HttpContext.SignInAsync(
                CookieAuthenticationDefaults.AuthenticationScheme,
                claimsPrincipal,
                authProperties
            );

            // Also set session data if needed
            HttpContext.Session.SetInt32("UserId", user.Id);
            HttpContext.Session.SetString("Username", user.Username);

            // Redirect based on role
            if (role.RoleName == "Admin")
            {
                return RedirectToPage("/admin");
            }
            
            // Check if there's a return URL
            var returnUrl = Request.Query["returnUrl"].ToString();
            if (!string.IsNullOrEmpty(returnUrl))
            {
                return LocalRedirect(returnUrl);
            }

            return RedirectToPage("/Index");
        }

        public async Task<IActionResult> OnGetLogoutAsync()
        {
            // Sign out the user
            await HttpContext.SignOutAsync(CookieAuthenticationDefaults.AuthenticationScheme);
            
            // Clear the session
            HttpContext.Session.Clear();
            
            return RedirectToPage("/Index");
        }

        private string HashPassword(string password)
        {
            using (var sha256 = SHA256.Create())
            {
                var bytes = sha256.ComputeHash(Encoding.UTF8.GetBytes(password));
                return BitConverter.ToString(bytes).Replace("-", "").ToLower();
            }
        }
    }
}