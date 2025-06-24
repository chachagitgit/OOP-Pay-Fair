namespace OOP_Fair_Fare.Models
{
    // Represents a user account in the system
    // Used for authentication, user management, and linking to roles and saved routes
    public class AppUser
    {
        public int Id { get; set; } // Primary key for the AppUser table
        public required string FirstName { get; set; } // User's first name
        public required string LastName { get; set; } // User's last name
        public required string Username { get; set; } // Unique username for login
        public required string Email { get; set; } // Unique email for login and notifications
        public required string HashedPassword { get; set; } // Hashed password for authentication
        public bool IsDeleted { get; set; } = false; // Soft delete flag (used for account deletion)
        public ICollection<SavedRoute> SavedRoutes { get; set; } = new List<SavedRoute>(); // Navigation property for user's saved routes (see SavedRoute.cs)
    }
}