using System.Linq;

namespace OOP_Fair_Fare.Models
{
    public static class VehicleSeeder
    {
        public static void SeedVehicles(AppDbContext db)
        {
            if (!db.Vehicles.Any())
            {
                db.Vehicles.AddRange(
                    new Vehicle { Name = "Airconditioned Bus", BaseFare = 15.00m, BaseKm = 5.0m, AdditionalFare = 2.65m },
                    new Vehicle { Name = "Ordinary Bus", BaseFare = 13.00m, BaseKm = 5.0m, AdditionalFare = 2.25m },
                    new Vehicle { Name = "Modern E-Jeepney", BaseFare = 14.00m, BaseKm = 4.0m, AdditionalFare = 2.00m },
                    new Vehicle { Name = "Traditional Jeepney", BaseFare = 12.00m, BaseKm = 4.0m, AdditionalFare = 1.80m }
                );
                db.SaveChanges();
            }
        }
    }
}
