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
            
            // If decimal is <= 0.49, round down
            if (decimalPart <= 0.49M)
            {
                return Math.Floor(fare);
            }
            // If decimal is >= 0.50, round up
            else
            {
                return Math.Ceiling(fare);
            }
        }
    }
}
