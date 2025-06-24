using System.ComponentModel.DataAnnotations;

namespace OOP_Fair_Fare.Models 
{
    public class Vehicle : BaseVehicle //inherits the base properties from BaseVehicle.cs
    {
        [Required]
        public decimal BaseKm { get; set; }

        [Required]
        public decimal AdditionalFare { get; set; }

        public override decimal CalculateFare(decimal distance) // runtime polymorphism
        //overrides the abstract method from BaseVehicle.cs with its own implementation
        {
            var additionalDistance = Math.Max(0, distance - BaseKm);
            var fare = BaseFare + (additionalDistance * AdditionalFare);
            return RoundFare(fare); //abstraction - uses the protected RoundFare method from superclass
        }
    }
}
