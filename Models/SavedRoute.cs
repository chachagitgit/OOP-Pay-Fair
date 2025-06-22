using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace OOP_Fair_Fare.Models
{
    public class SavedRoute
    {
        [Key]
        public int Id { get; set; }

        [Required]
        public int UserId { get; set; }

        [ForeignKey("UserId")]
        public AppUser User { get; set; }

        [Required]
        public string StartLocation { get; set; }

        [Required]
        public string Destination { get; set; }
        [Required]
        public string Vehicle {get; set; }
        [Required]
        public double RegularFare { get; set; }
        [Required]
        public double AppliedDiscount { get; set; }
        [Required]
        public double Distance { get; set; }
        [Required]
        public double EstimatedFare { get; set; }
        [Required]
        public DateTime DateSaved { get; set; } = DateTime.Now;
    }
}