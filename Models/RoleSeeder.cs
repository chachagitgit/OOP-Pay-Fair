using System.Linq;

namespace OOP_Fair_Fare.Models
{
    public static class RoleSeeder
    {
        public static void SeedRoles(AppDbContext db)
        {
            // Only this user can be admin
            var adminEmail = "payfairadmin@gmail.com";
            var adminUser = db.Users.FirstOrDefault(u => u.Email == adminEmail);
            if (adminUser != null)
            {
                // Remove any existing Admin roles from other users
                var nonAdminAdmins = db.Roles.Where(r => r.RoleName == "Admin" && r.UserId != adminUser.Id).ToList();
                if (nonAdminAdmins.Any())
                {
                    db.Roles.RemoveRange(nonAdminAdmins);
                }
                // Ensure only this user has Admin role
                if (!db.Roles.Any(r => r.UserId == adminUser.Id && r.RoleName == "Admin"))
                {
                    db.Roles.Add(new Role { UserId = adminUser.Id, RoleName = "Admin" });
                }
            }
            // Assign Regular role to all other users who have no role
            var usersWithoutRole = db.Users.Where(u => !db.Roles.Any(r => r.UserId == u.Id));
            foreach (var user in usersWithoutRole)
            {
                db.Roles.Add(new Role { UserId = user.Id, RoleName = "Regular" });
            }
            db.SaveChanges();
        }
    }
}
