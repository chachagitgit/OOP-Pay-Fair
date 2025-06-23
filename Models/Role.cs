using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace OOP_Fair_Fare.Models
{
    public class Role
    {
        [Key]
        public int Id { get; set; }

        [Required]
        public int UserId { get; set; }

        [ForeignKey("UserId")]
        public AppUser User { get; set; }

        [Required]
        [MaxLength(50)]
        public string RoleName { get; set; } // "Admin" or "Regular"
    }
}
