using System;
using OOP_Fair_Fare.Models;
using System.Linq;

namespace OOP_Fair_Fare.Services
{
    public class FareCalculatorDB : IFareCalculator //inheritance - implements IFareCalculator interface
    {//encap
        private readonly AppDbContext _db; //implementation of polymorphism in IFareCalculator.cs
        private const decimal DISCOUNT_RATE = 0.20m; // 20% discount

        public FareCalculatorDB(AppDbContext db) //constructor encap
        {
            _db = db ?? throw new ArgumentNullException(nameof(db));
        }

//abstraction - hides complex fare calculation logic
        public FareResult Calculate(double distanceTraveled, int vehicleChoice, bool isDiscounted)
        {
            var vehicle = _db.Vehicles.FirstOrDefault(v => v.VehicleId == vehicleChoice)
                ?? throw new ArgumentException("Invalid vehicle choice");

            decimal distance = Convert.ToDecimal(distanceTraveled);
            decimal regularFare = vehicle.CalculateFare(distance);
            decimal discountAmount = isDiscounted ? regularFare * DISCOUNT_RATE : 0;
            decimal totalFare = regularFare - discountAmount;

            return new FareResult
            {
                BaseFare = Convert.ToDouble(regularFare),
                DiscountAmount = Convert.ToDouble(discountAmount),
                TotalFare = Convert.ToDouble(totalFare)
            };
        }
    }
}
