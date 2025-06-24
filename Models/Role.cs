using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace OOP_Fair_Fare.Models
{
    // Represents a user role (e.g., Admin or Regular) in the system
    // Used for role-based access control throughout the project
    public class Role
    {
        [Key]
        public int Id { get; set; } // Primary key for the Role table

        [Required]
        public int UserId { get; set; } // Foreign key to AppUser
        [ForeignKey("UserId")]
        public virtual AppUser User { get; set; } = null!; // Navigation property to the user

        [Required]
        [MaxLength(50)]
        public required string RoleName { get; set; } // "Admin" or "Regular"; checked in login, dashboard, and admin logic
    }
}
