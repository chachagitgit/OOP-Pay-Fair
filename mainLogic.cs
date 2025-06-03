using System;

public class FareCalculator
{
    private const decimal DiscountRate = 0.20m;

    public static void CalculateFare(double distanceTraveled, int vehicleChoice, bool isDiscounted)
    {
        decimal baseFare = 0;
        int baseDistance = 0;
        decimal perKmRate = 0;
        string vehicleName = "";

// vehicle selection
        switch (vehicleChoice)
        {
            case 1:
                vehicleName = "Airconditioned Bus";
                baseFare = 15;
                baseDistance = 5;
                perKmRate = 3;
                break;
            case 2:
                vehicleName = "Ordinary Bus";
                baseFare = 13;
                baseDistance = 5;
                perKmRate = 3;
                break;
            case 3:
                vehicleName = "Modern E-Jeepney";
                baseFare = 15;
                baseDistance = 4;
                perKmRate = 3;
                break;
            case 4:
                vehicleName = "Traditional Jeepney";
                baseFare = 13;
                baseDistance = 4;
                perKmRate = 2;
                break;
            default:
                Console.WriteLine("Invalid vehicle selection.");
                return;
        }

        decimal regularFare = baseFare;

        if (distanceTraveled > baseDistance)
        {
            int extraKm = (int)Math.Floor(distanceTraveled - baseDistance);
            regularFare += extraKm * perKmRate;
        }

        int discountAmount = 0;
        if (isDiscounted)
        {
            discountAmount = (int)Math.Round(regularFare * DiscountRate);
        }

        int totalFare = (int)Math.Round(regularFare) - discountAmount;

// fare summary
        Console.WriteLine("\n--- Fare Summary ---");
        Console.WriteLine($"Vehicle Type     : {vehicleName}");
        Console.WriteLine($"Distance Traveled: {distanceTraveled} km");
        Console.WriteLine($"Regular Fare     : ₱{Math.Round(regularFare)}");
        Console.WriteLine($"Discount Applied : ₱{discountAmount}");
        Console.WriteLine($"Total Fare       : ₱{totalFare}");
    }
}
