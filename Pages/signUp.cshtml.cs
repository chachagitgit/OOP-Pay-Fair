using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using OOP_Fair_Fare.Models;
using System.ComponentModel.DataAnnotations;
using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;
using System.Linq;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Infrastructure;

namespace OOP_Fair_Fare.Pages
{
    public class signUpModel : PageModel
    {
        private readonly AppDbContext _db;
        public signUpModel(AppDbContext db) { _db = db; }        [BindProperty]
        public InputModel Input { get; set; } = new InputModel() 
        {
            FirstName = string.Empty,
            LastName = string.Empty,
            Username = string.Empty,
            Email = string.Empty,
            Password = string.Empty,
            ConfirmPassword = string.Empty
        };public string? ErrorMessage { get; set; }

        public class InputModel
        {
            [Required]
            public required string FirstName { get; set; }
            [Required]
            public required string LastName { get; set; }
            [Required]
            public required string Username { get; set; }
            [Required]
            [EmailAddress]
            public required string Email { get; set; }
            [Required]
            public required string Password { get; set; }
            [Required]
            [Compare("Password")]
            public required string ConfirmPassword { get; set; }
        }

        public void OnGet() { }

        public async Task<IActionResult> OnPostAsync()
        {
            try
            {
                if (!ModelState.IsValid)
                {
                    ErrorMessage = "Please fill all fields correctly.";
                    return Page();
                }
                if (_db.Users.Any(u => u.Email == Input.Email || u.Username == Input.Username))
                {
                    ErrorMessage = "Email or Username already exists.";
                    return Page();
                }
                var user = new AppUser
                {
                    FirstName = Input.FirstName,
                    LastName = Input.LastName,
                    Username = Input.Username,
                    Email = Input.Email,
                    HashedPassword = HashPassword(Input.Password),
                };
                _db.Users.Add(user);
                await _db.SaveChangesAsync();
                // Assign Regular role to new user
                _db.Roles.Add(new Role { UserId = user.Id, RoleName = "Regular" });
                await _db.SaveChangesAsync();
                ErrorMessage = "Account created successfully! You can now log in.";
                //ErrorMessage = $"DB Path: {_db.Database.GetDbConnection().DataSource}";
                return RedirectToPage("/log-in");
                //return Page(); // For debugging, show the error message on the same page
            }
            catch (Exception ex)
            {
                ErrorMessage = $"Sign up failed: {ex.Message}";
                return Page();
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