using System;

namespace OOP_Fair_Fare.Models
{
    // Represents an email verification record for user registration or password reset
    // Used to store OTPs and their status for email-based verification flows
    public class EmailVerification
    {
        public int Id { get; set; } // Primary key for the EmailVerification table
        public string? Email { get; set; } // Email address to verify
        public required string OTP { get; set; } // One-time password (verification code)
        public DateTime CreatedAt { get; set; } // When the OTP was generated
        public DateTime ExpiresAt { get; set; } // When the OTP expires
        public bool IsUsed { get; set; } // True if the OTP has already been used
        // Linked to user registration, password reset, and email confirmation logic
    }
}
