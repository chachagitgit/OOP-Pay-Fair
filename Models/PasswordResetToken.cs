namespace OOP_Fair_Fare.Models
{
    // Represents a password reset token for a user
    // Used in password reset flows to securely allow users to change their password
    public class PasswordResetToken
    {
        public int Id { get; set; } // Primary key for the PasswordResetToken table
        public int UserId { get; set; } // Foreign key to AppUser
        public required string Token { get; set; } // Unique token for password reset
        public DateTime ExpiryDate { get; set; } // When the token expires
        public bool IsUsed { get; set; } // True if the token has already been used
        public virtual AppUser? User { get; set; } // Navigation property to the user
        // Linked to user authentication and password reset logic
    }
}
