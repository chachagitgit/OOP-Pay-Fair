using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;
using OOP_Fair_Fare.Models;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

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
            }
            user.Username = Username;
            user.FirstName = FirstName;
            user.LastName = LastName;
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
    }
}