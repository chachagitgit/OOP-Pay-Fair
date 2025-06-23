using System;
using OOP_Fair_Fare.Models;
using System.Linq;

namespace OOP_Fair_Fare
{
    public class FareCalculator
    {
        private readonly AppDbContext _db;
        public FareCalculator(AppDbContext db)
        {
            _db = db;
        }

        public class FareResult
        {
            public double BaseFare { get; set; }
            public double DiscountAmount { get; set; }
            public double TotalFare { get; set; }
        }

        public FareResult Calculate(double distanceTraveled, int vehicleChoice, bool isDiscounted)
        {
            // Get vehicle from DB
            var vehicle = _db.Vehicles.FirstOrDefault(v => v.VehicleId == vehicleChoice);
            if (vehicle == null)
                throw new ArgumentException("Invalid vehicle choice");

            double initialKm = (double)vehicle.BaseKm;
            double initialFare = (double)vehicle.BaseFare;
            double additionalPerKm = (double)vehicle.AdditionalFare;

            // If you want to support discounted fares, you can add columns for discounted fares in Vehicle table
            // For now, use the same fare for both regular and discounted
            double regularFare = Math.Round(CalculateBaseFare(distanceTraveled, initialKm, initialFare, additionalPerKm), 2);
            double discountedFare = regularFare; // You can adjust this if you add discount logic
            double discountAmount = 0;
            if (isDiscounted)
            {
                // Example: 20% discount
                discountedFare = Math.Round(regularFare * 0.8, 2);
                discountAmount = Math.Round(regularFare - discountedFare, 2);
            }
            double rawTotalFare = isDiscounted ? discountedFare : regularFare;

            //apply rounding rules only to the final total fare
            double roundedTotalFare = rawTotalFare;
            if (roundedTotalFare % 1 >= 0.50)
            {
                roundedTotalFare = Math.Ceiling(roundedTotalFare);
            }
            else
            {
                roundedTotalFare = Math.Floor(roundedTotalFare);
            }

            return new FareResult
            {
                BaseFare = regularFare,
                DiscountAmount = discountAmount,
                TotalFare = roundedTotalFare
            };
        }

        private double CalculateBaseFare(double distanceTraveled, double initialKm, double initialFare, double additionalPerKm)
        {
            double additionalDistance = Math.Max(0, distanceTraveled - initialKm);
            return initialFare + (additionalDistance * additionalPerKm);
        }
    }
}