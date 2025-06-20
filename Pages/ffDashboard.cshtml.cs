using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.AspNetCore.Http;
using OOP_Fair_Fare.Models;
using System.Threading.Tasks;
using System.IO;

namespace OOP_Fair_Fare.Pages
{
    [IgnoreAntiforgeryToken]
    public class ffDashboardModel : PageModel
    {
        private readonly AppDbContext _dbContext;
        public ffDashboardModel(AppDbContext dbContext)
        {
            _dbContext = dbContext;
        }

        [BindProperty]
        public string? StartLocation { get; set; }
        [BindProperty]
        public string? Destination { get; set; }
        [BindProperty]
        public decimal EstimatedFare { get; set; }

        public int? UserId { get; private set; }
        public bool IsLoggedIn => UserId != null;

        public void OnGet()
        {
            UserId = HttpContext.Session.GetInt32("UserId");
        }

        public async Task<IActionResult> OnPostSaveRouteAsync()
        {
            UserId = HttpContext.Session.GetInt32("UserId");
            if (UserId == null)
                return new JsonResult(new { success = false, error = "Not logged in" });

            using (var reader = new StreamReader(Request.Body))
            {
                var body = await reader.ReadToEndAsync();
                var data = System.Text.Json.JsonDocument.Parse(body).RootElement;

                var origin = data.GetProperty("Origin").GetString();
                var destination = data.GetProperty("Destination").GetString();
                var fare = data.GetProperty("Fare").GetDecimal();

                var route = new SavedRoute
                {
                    UserId = UserId.Value,
                    StartLocation = origin!,
                    Destination = destination!,
                    EstimatedFare = (double)fare
                };
                _dbContext.SavedRoutes.Add(route);
                await _dbContext.SaveChangesAsync();
                return new JsonResult(new { success = true });
            }
        }
    }
}