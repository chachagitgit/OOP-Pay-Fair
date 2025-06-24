using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace OOP_Fair_Fare.Models
{
    // Represents a route saved by a user for future reference
    // Used in user_account.cshtml.cs and ffDashboard.cshtml.cs for displaying and saving routes
    public class SavedRoute
    {
        [Key]
        public int Id { get; set; } // Primary key for the SavedRoute table

        [Required]
        public int UserId { get; set; } // Foreign key to AppUser
        [ForeignKey("UserId")]
        public virtual AppUser User { get; set; } = null!; // Navigation property to the user

        [Required]
        public required string StartLocation { get; set; } // Origin of the route

        [Required]
        public required string Destination { get; set; } // Destination of the route
        [Required]
        public required string Vehicle {get; set; } // Vehicle type used for the route
        [Required]
        public double RegularFare { get; set; } // Fare before discount
        [Required]
        public double AppliedDiscount { get; set; } // Discount applied (if any)
        [Required]
        public double Distance { get; set; } // Distance traveled
        [Required]
        public double EstimatedFare { get; set; } // Final fare after discount and rounding
        [Required]
        public DateTime DateSaved { get; set; } = DateTime.Now; // When the route was saved
    }
}