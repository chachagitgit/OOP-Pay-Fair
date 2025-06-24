using System;
using System.ComponentModel.DataAnnotations;

namespace OOP_Fair_Fare.Models
{
//abstraction
    public abstract class BaseVehicle //defines base template for all vehicle types
    {
        [Key] //encapsulates data validation ng attribute Key
        public int VehicleId { get; set; } //also encapsulates this property

        [Required] //encapsulates data validation of Required attribute
        [MaxLength(100)] //encapsulates data validation of MaxLength atribute
        public required string Name { get; set; } //encapsulates this property

        [Required]
        public decimal BaseFare { get; set; } //encapsulates this property

        public abstract decimal CalculateFare(decimal distance);
        //implement fare calculations
        protected decimal RoundFare(decimal fare) //hides the rounding process
        {
            // Get the decimal part
            decimal decimalPart = fare - Math.Floor(fare);
            
            // If there's any decimal part, round up
            if (decimalPart > 0)
            {
                return Math.Ceiling(fare);
            }
            // If no decimal part, return as is
            return fare;
        }
    }
}
