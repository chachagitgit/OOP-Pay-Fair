using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using OOP_Fair_Fare.Models;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using System.Collections.Generic;
using Microsoft.AspNetCore.Authorization;

namespace OOP_Fair_Fare.Pages
{   [Authorize]
    [IgnoreAntiforgeryToken]
    public class adminModel : PageModel
    {
        private readonly AppDbContext _db;
        public adminModel(AppDbContext db) { _db = db; }

        public List<Vehicle> Vehicles { get; set; } = new();

        public async Task OnGetAsync()
        {
            Vehicles = await _db.Vehicles.OrderBy(v => v.VehicleId).ToListAsync();
        }

        public async Task<IActionResult> OnPostUpdateFareAsync([FromBody] UpdateFareDto dto)
        {
            var vehicle = await _db.Vehicles.FindAsync(dto.Id);
            if (vehicle == null) return NotFound();
            vehicle.BaseFare = dto.BaseFare;
            vehicle.BaseKm = dto.BaseKm;
            vehicle.AdditionalFare = dto.AdditionalFare;
            await _db.SaveChangesAsync();
            return new JsonResult(new { success = true });
        }

        public IActionResult OnPostLogout()
        {
            HttpContext.Session.Clear();
            return RedirectToPage("/log-in");
        }

        public class UpdateFareDto
        {
            public int Id { get; set; }
            public decimal BaseFare { get; set; }
            public decimal BaseKm { get; set; }
            public decimal AdditionalFare { get; set; }
        }
    }
}
