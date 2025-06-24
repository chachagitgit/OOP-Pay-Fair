using System;

namespace OOP_Fair_Fare.Models
{
    public class EmailVerification
    { //abstract the concept of email verificaiton
        //encap data with get and set
        public int Id { get; set; }
        public string? Email { get; set; }
        public required string OTP { get; set; }
        public DateTime CreatedAt { get; set; }
        public DateTime ExpiresAt { get; set; }
        public bool IsUsed { get; set; }
    }
}
