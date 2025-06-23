using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.AspNetCore.Http;
using OOP_Fair_Fare.Models;
using System.Threading.Tasks;
using System.IO;
using Microsoft.Extensions.Logging;
using Microsoft.AspNetCore.Mvc.ViewFeatures;

namespace OOP_Fair_Fare.Pages
{
    [IgnoreAntiforgeryToken]
    public class ffDashboardModel : PageModel
    {
        private readonly AppDbContext _dbContext;
        private readonly ILogger<ffDashboardModel> _logger;
        public ffDashboardModel(AppDbContext dbContext, ILogger<ffDashboardModel> logger)
        {
            _dbContext = dbContext;
            _logger = logger;
            _logger.LogInformation("ffDashboardModel constructed");
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
            _logger.LogInformation("OnGet called");
        }

        [BindProperty] public string? RouteOrigin { get; set; }
        [BindProperty] public string? RouteDestination { get; set; }
        [BindProperty] public string? RouteVehicle { get; set; }
        [BindProperty] public double RouteRegularFare { get; set; }
        [BindProperty] public double RouteAppliedDiscount { get; set; }
        [BindProperty] public double RouteDistance { get; set; }
        [BindProperty] public decimal RouteFare { get; set; }

        [TempData]
        public string? SaveRouteResult { get; set; }

        public async Task<IActionResult> OnPostAsync()
        {
            UserId = HttpContext.Session.GetInt32("UserId");
            _logger.LogInformation("OnPostAsync: UserId={UserId}, Origin={RouteOrigin}, Destination={RouteDestination}, Vehicle={RouteVehicle}, RegularFare={RouteRegularFare}, AppliedDiscount={RouteAppliedDiscount}, Distance={RouteDistance}, Fare={RouteFare}", UserId, RouteOrigin, RouteDestination, RouteVehicle, RouteRegularFare, RouteAppliedDiscount, RouteDistance, RouteFare);
            _logger.LogInformation("ModelState.IsValid={IsValid}", ModelState.IsValid);
            if (!ModelState.IsValid)
            {
                foreach (var kvp in ModelState)
                {
                    foreach (var error in kvp.Value.Errors)
                    {
                        _logger.LogWarning($"ModelState error for {kvp.Key}: {error.ErrorMessage}");
                    }
                }
                SaveRouteResult = "Invalid form submission. Please try again.";
                return Page();
            }
            if (UserId == null)
            {
                _logger.LogWarning("SaveRoute: Not logged in");
                SaveRouteResult = "You must be logged in to save routes.";
                return Page();
            }
            // Validate required fields
            if (string.IsNullOrWhiteSpace(RouteOrigin) || string.IsNullOrWhiteSpace(RouteDestination) || RouteFare == 0)
            {
                _logger.LogWarning("SaveRoute: Missing or invalid required fields. Origin={RouteOrigin}, Destination={RouteDestination}, Fare={RouteFare}", RouteOrigin, RouteDestination, RouteFare);
                SaveRouteResult = "Missing or invalid required fields. Please calculate a fare and try again.";
                return Page();
            }
            var route = new SavedRoute
            {
                UserId = UserId.Value,
                StartLocation = RouteOrigin,
                Destination = RouteDestination,
                Vehicle = RouteVehicle ?? string.Empty,
                RegularFare = RouteRegularFare,
                AppliedDiscount = RouteAppliedDiscount,
                Distance = RouteDistance,
                EstimatedFare = (double)RouteFare,
                DateSaved = DateTime.Now
            };
            try
            {
                _dbContext.SavedRoutes.Add(route);
                await _dbContext.SaveChangesAsync();
                _logger.LogInformation("SaveRoute: Route saved for user {UserId}", UserId);
                SaveRouteResult = "Route saved successfully!";
                return RedirectToPage("/ffDashboard");
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error saving route. UserId={UserId}, Origin={RouteOrigin}, Destination={RouteDestination}", UserId, RouteOrigin, RouteDestination);
                SaveRouteResult = $"An error occurred while saving the route: {ex.Message}";
                return Page();
            }
        }
    }
}