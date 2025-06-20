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
        public string ErrorMessage { get; set; }

        public class InputModel
        {
            [Required]
            public string EmailOrUsername { get; set; }
            [Required]
            public string Password { get; set; }
        }

        public void OnGet() { }

        public async Task<IActionResult> OnPostAsync()
        {
            ErrorMessage = "Handler reached!";
            if (!ModelState.IsValid)
            {
                ErrorMessage = "Please fill in all fields.";
                return Page();
            }
        
            string hashed = HashPassword(Input.Password);
        
            var user = await _db.Users.FirstOrDefaultAsync(u =>
                (u.Email == Input.EmailOrUsername || u.Username == Input.EmailOrUsername)
                && u.HashedPassword == hashed);
        
            if (user == null)
            {
                ErrorMessage = "Invalid credentials.";
                return Page();
            }
        
            // Set session
            HttpContext.Session.SetInt32("UserId", user.Id);
            HttpContext.Session.SetString("Username", user.Username);
            ErrorMessage = "Redirecting...";
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