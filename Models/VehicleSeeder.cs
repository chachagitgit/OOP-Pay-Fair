// Required for LINQ operations (e.g., Any())
using System.Linq;

namespace OOP_Fair_Fare.Models
{
    // Seeds the Vehicles table with default fare data if empty.
    // Ensures the fare dashboard and admin management have initial data to work with.
    // Linked to: AppDbContext.cs (DbSet<Vehicle>), Startup.cs (called on app startup), admin.cshtml.cs (admin fare management), FareCalculator.cs (fare logic)
    public static class VehicleSeeder
    {
        // Adds default vehicles and fares to the database if none exist
        public static void SeedVehicles(AppDbContext db)
        {
            // Only seed if the Vehicles table is empty (prevents duplicate data)
            if (!db.Vehicles.Any())
            {
                db.Vehicles.AddRange(
                    new Vehicle { Name = "Airconditioned Bus", BaseFare = 15.00m, BaseKm = 5.0m, AdditionalFare = 2.65m },
                    new Vehicle { Name = "Ordinary Bus", BaseFare = 13.00m, BaseKm = 5.0m, AdditionalFare = 2.25m },
                    new Vehicle { Name = "Modern E-Jeepney", BaseFare = 15.00m, BaseKm = 4.0m, AdditionalFare = 2.00m },
                    new Vehicle { Name = "Traditional Jeepney", BaseFare = 13.00m, BaseKm = 4.0m, AdditionalFare = 1.80m }
                );
                // Save changes to the database (creates the initial vehicle records)
                db.SaveChanges();
            }
        }
    }
}
