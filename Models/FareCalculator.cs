using System;

namespace OOP_Fair_Fare
{
    public class FareCalculator
    {
        public class FareResult
        {
            public double BaseFare { get; set; }
            public double DiscountAmount { get; set; }
            public double TotalFare { get; set; }
        }

        public FareResult Calculate(double distanceTraveled, int vehicleChoice, bool isDiscounted)
        {
            //calculate with decimals for breakdown
            double baseFare = Math.Round(CalculateBaseFare(distanceTraveled, vehicleChoice), 2);
            double discountAmount = Math.Round(isDiscounted ? baseFare * 0.20 : 0, 2); // 20% discount
            double rawTotalFare = baseFare - discountAmount;

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
                BaseFare = baseFare,
                DiscountAmount = discountAmount,
                TotalFare = roundedTotalFare
            };
        }

        private double CalculateBaseFare(double distanceTraveled, int vehicleChoice)
        {
            double baseFare = 0;
            
            switch (vehicleChoice)
            {
                case 1: // Airconditioned Bus
                    baseFare = 13 + (Math.Max(0, distanceTraveled - 5) * 2.20);
                    break;
                case 2: // Ordinary Bus
                    baseFare = 11 + (Math.Max(0, distanceTraveled - 5) * 1.85);
                    break;
                case 3: // Modern E-Jeepney
                    baseFare = 12 + (Math.Max(0, distanceTraveled - 4) * 1.80);
                    break;
                case 4: // Traditional Jeepney
                    baseFare = 12 + (Math.Max(0, distanceTraveled - 4) * 1.80);
                    break;
                default:
                    throw new ArgumentException("Invalid vehicle choice");
            }

            return baseFare;
        }
    }
} 