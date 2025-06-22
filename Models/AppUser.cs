namespace OOP_Fair_Fare.Models
{
    public class AppUser
    {
        public int Id { get; set; }
        public required string FirstName { get; set; }
        public required string LastName { get; set; }
        public required string Username { get; set; }
        public required string Email { get; set; }
        public required string HashedPassword { get; set; }
        public bool IsDeleted { get; set; } = false;
        public ICollection<SavedRoute> SavedRoutes { get; set; }
    }
}