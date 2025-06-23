using System.ComponentModel.DataAnnotations;

namespace OOP_Fair_Fare.Models
{
    public class Vehicle
    {
        [Key]
        public int VehicleId { get; set; }

        [Required]
        [MaxLength(100)]
        public string Name { get; set; }

        [Required]
        public decimal BaseFare { get; set; }

        [Required]
        public decimal BaseKm { get; set; }

        [Required]
        public decimal AdditionalFare { get; set; }
    }
}
