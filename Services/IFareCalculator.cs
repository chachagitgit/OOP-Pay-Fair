namespace OOP_Fair_Fare.Services
{
    public interface IFareCalculator
    // defines a contract that can be implemented differently in different classes
    //while maintaining the same contract - polymorphism
    {
        FareResult Calculate(double distanceTraveled, int vehicleChoice, bool isDiscounted);
    }

    public class FareResult //encapsulates calculation results
    {
        public double BaseFare { get; set; }
        public double DiscountAmount { get; set; }
        public double TotalFare { get; set; }
    }
}
