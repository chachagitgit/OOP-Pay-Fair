namespace OOP_Fair_Fare.Models
{
    //encapsulation
    public class PasswordResetToken
    {
        public int Id { get; set; }
        public int UserId { get; set; }
        public required string Token { get; set; }
        public DateTime ExpiryDate { get; set; }
        public bool IsUsed { get; set; }
        public virtual AppUser? User { get; set; }
    }
}
