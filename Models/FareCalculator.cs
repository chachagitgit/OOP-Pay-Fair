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
            double regularFare = Math.Round(CalculateBaseFare(distanceTraveled, vehicleChoice, false), 2);
            double discountedFare = Math.Round(CalculateBaseFare(distanceTraveled, vehicleChoice, isDiscounted), 2);
            double discountAmount = isDiscounted ? Math.Round(regularFare - discountedFare, 2) : 0;
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

        private double CalculateBaseFare(double distanceTraveled, int vehicleChoice, bool isDiscounted)
        {
            double baseFare = 0;
            double initialKm;
            double initialFare;
            double additionalPerKm;
            
            switch (vehicleChoice)
            {
                case 1: // Airconditioned Bus
                    initialKm = 5;
                    if (isDiscounted)
                    {
                        initialFare = 12.00;
                        additionalPerKm = 2.12;
                    }
                    else
                    {
                        initialFare = 15.00;
                        additionalPerKm = 2.65;
                    }
                    break;

                case 2: // Ordinary Bus
                    if (isDiscounted)
                    {
                        initialKm = 4;
                        initialFare = 10.40;
                        additionalPerKm = 1.80;
                    }
                    else
                    {
                        initialKm = 5;
                        initialFare = 13.00;
                        additionalPerKm = 2.25;
                    }
                    break;

                case 3: // Modern E-Jeepney
                    initialKm = 4;
                    if (isDiscounted)
                    {
                        initialFare = 12.00;
                        additionalPerKm = 1.44;
                    }
                    else
                    {
                        initialFare = 15.00;
                        additionalPerKm = 1.80;
                    }
                    break;

                case 4: // Traditional Jeepney
                    initialKm = 4;
                    if (isDiscounted)
                    {
                        initialFare = 10.40;
                        additionalPerKm = 1.44;
                    }
                    else
                    {
                        initialFare = 13.00;
                        additionalPerKm = 1.80;
                    }
                    break;

                default:
                    throw new ArgumentException("Invalid vehicle choice");
            }

            // Calculate total fare
            double additionalDistance = Math.Max(0, distanceTraveled - initialKm);
            baseFare = initialFare + (additionalDistance * additionalPerKm);

            return baseFare;
        }
    }
} 