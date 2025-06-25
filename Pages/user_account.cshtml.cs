using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;
using OOP_Fair_Fare.Models;
using System.Text;
using System.Security.Cryptography;

namespace OOP_Fair_Fare.Pages
{
    public class user_accountModel : PageModel
    {
        private readonly AppDbContext _db;
        public user_accountModel(AppDbContext db)
        {
            _db = db;
        }

        public bool IsLoggedIn { get; set; }
        public AppUser? UserInfo { get; set; }
        public List<SavedRoute> SavedRoutes { get; set; } = new List<SavedRoute>();
        [BindProperty]
        public string? Username { get; set; }
        [BindProperty]
        public string? FirstName { get; set; }
        [BindProperty]
        public string? LastName { get; set; }
        public string? StatusMessage { get; set; }
        [BindProperty]
        public string? CurrentPassword { get; set; }
        [BindProperty]
        public string? NewPassword { get; set; }
        [BindProperty]
        public string? ConfirmNewPassword { get; set; }

        public async Task<IActionResult> OnGetAsync()
        {
            var userId = HttpContext.Session.GetInt32("UserId");
            if (userId == null)
            {
                IsLoggedIn = false;
                return Page();
            }
            IsLoggedIn = true;
            UserInfo = await _db.Users.FirstOrDefaultAsync(u => u.Id == userId && !u.IsDeleted);
            if (UserInfo == null)
            {
                IsLoggedIn = false;
                return Page();
            }
            Username = UserInfo.Username;
            FirstName = UserInfo.FirstName;
            LastName = UserInfo.LastName;
            SavedRoutes = await _db.SavedRoutes.Where(r => r.UserId == userId).OrderByDescending(r => r.DateSaved).ToListAsync();
            return Page();
        }

        public async Task<IActionResult> OnPostEditInfoAsync()
        {
            var userId = HttpContext.Session.GetInt32("UserId");
            if (userId == null)
            {
                return RedirectToPage("/log-in");
            }
            var user = await _db.Users.FirstOrDefaultAsync(u => u.Id == userId && !u.IsDeleted);
            if (user == null)
            {
                return RedirectToPage("/log-in");
            }            user.Username = Username ?? user.Username;
            user.FirstName = FirstName ?? user.FirstName;
            user.LastName = LastName ?? user.LastName;
            await _db.SaveChangesAsync();
            StatusMessage = "Profile updated.";
            return RedirectToPage();
        }

        public async Task<IActionResult> OnPostDeleteRouteAsync(int RouteId)
        {
            var userId = HttpContext.Session.GetInt32("UserId");
            if (userId == null)
            {
                return RedirectToPage("/log-in");
            }
            var route = await _db.SavedRoutes.FirstOrDefaultAsync(r => r.Id == RouteId && r.UserId == userId);
            if (route != null)
            {
                _db.SavedRoutes.Remove(route);
                await _db.SaveChangesAsync();
                StatusMessage = "Route deleted.";
            }
            return RedirectToPage();
        }
        
        public IActionResult OnPostLogout()
        {
            HttpContext.Session.Clear();
            return RedirectToPage("/log-in");
        }

            public async Task<IActionResult> OnPostDeleteAccountAsync(string showConfirm, string confirmDelete)
        {
            // Step 1: Show confirmation dialog
            if (!string.IsNullOrEmpty(showConfirm))
            {
                TempData["ShowDeleteConfirm"] = true;
                return Page();
            }

            // Step 2: soft delete the account
            if (!string.IsNullOrEmpty(confirmDelete))
            {
                var userId = HttpContext.Session.GetInt32("UserId");
                if (userId != null)
                {
                    var user = await _db.Users
                        .Include(u => u.SavedRoutes)
                        .FirstOrDefaultAsync(u => u.Id == userId.Value);

                    if (user != null)
                    {
                        _db.SavedRoutes.RemoveRange(user.SavedRoutes);
                        user.IsDeleted = true; // Soft delete
                        await _db.SaveChangesAsync();
                    }
                }
                HttpContext.Session.Clear();
                return RedirectToPage("/Index");
            }

            // Fallback: just reload the page
            return Page();
        }

        private string HashPassword(string password)
        {
            using (var sha256 = SHA256.Create())
            {
                var bytes = sha256.ComputeHash(Encoding.UTF8.GetBytes(password));
                return BitConverter.ToString(bytes).Replace("-", "").ToLower();
            }
        }

        private JsonResult ReturnError(string message)
        {
            return new JsonResult(new { success = false, message = message });
        }

        public async Task<IActionResult> OnPostChangePasswordAsync()
        {
            try
            {
                var userId = HttpContext.Session.GetInt32("UserId");
                if (userId == null)
                {
                    return RedirectToPage("/log-in");
                }

                // Load user data for the page
                IsLoggedIn = true;
                UserInfo = await _db.Users
                    .FirstOrDefaultAsync(u => u.Id == userId && !u.IsDeleted);
                
                if (UserInfo == null)
                {
                    return RedirectToPage("/log-in");
                }

                // Set the user information for the page
                Username = UserInfo.Username;
                FirstName = UserInfo.FirstName;
                LastName = UserInfo.LastName;
                SavedRoutes = await _db.SavedRoutes
                    .Where(r => r.UserId == userId)
                    .OrderByDescending(r => r.DateSaved)
                    .ToListAsync();

                // Validate inputs
                if (string.IsNullOrEmpty(CurrentPassword) || string.IsNullOrEmpty(NewPassword) || string.IsNullOrEmpty(ConfirmNewPassword))
                {
                    return ReturnError("All password fields are required.");
                }

                // Hash both current and new passwords for comparison
                string hashedCurrentPassword = HashPassword(CurrentPassword);
                string hashedNewPassword = HashPassword(NewPassword);

                if (!string.Equals(UserInfo.HashedPassword, hashedCurrentPassword, StringComparison.OrdinalIgnoreCase))
                {
                    return ReturnError("Current password is incorrect.");
                }

                // Check if new passwords match
                if (NewPassword != ConfirmNewPassword)
                {
                    return ReturnError("New passwords do not match.");
                }

                // Check if new password hash is same as current password hash
                if (string.Equals(hashedNewPassword, UserInfo.HashedPassword, StringComparison.OrdinalIgnoreCase))
                {
                    return ReturnError("New password must be different from your current password.");
                }

                // Update the password
                UserInfo.HashedPassword = HashPassword(NewPassword);
                _db.Users.Update(UserInfo);
                await _db.SaveChangesAsync();

                return new JsonResult(new { success = true, message = "Password successfully updated." });
            }
            catch (Exception ex)
            {
                // Log the error - remove in production
                System.Diagnostics.Debug.WriteLine($"Error changing password: {ex}");
                return ReturnError("An error occurred while changing the password.");
            }
        }
    }
}