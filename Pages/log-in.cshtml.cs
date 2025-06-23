using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using OOP_Fair_Fare.Models;
using System.ComponentModel.DataAnnotations;
using System.Security.Cryptography;
using System.Text;
using System.Linq;
using Microsoft.EntityFrameworkCore;

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
            }            if (Input.Password == null)
            {
                ModelState.AddModelError(string.Empty, "Password is required.");
                return Page();
            }

            string hashed = HashPassword(Input.Password);
            if (user.HashedPassword != hashed)
            {
                ModelState.AddModelError(string.Empty, "Wrong Password, try again.");
                return Page();
            }            // Set session
            HttpContext.Session.SetInt32("UserId", user.Id);
            HttpContext.Session.SetString("Username", user.Username);

            // Check user role
            var role = await _db.Roles.FirstOrDefaultAsync(r => r.UserId == user.Id);
            if (role != null && role.RoleName == "Admin")
            {
                return RedirectToPage("/admin");
            }
            else
            {
                return RedirectToPage("/Index");
            }
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