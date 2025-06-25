using System;
using System.Linq;
using System.Text;
using System.Security.Cryptography;
using OOP_Fair_Fare.Models;

namespace OOP_Fair_Fare.Models
{
    public static class AdminSeeder
    {
        public static void SeedAdmin(AppDbContext db)
        {
            var admin = db.Users.FirstOrDefault(u => u.Id == 1);
            string hashedPassword = HashPassword("admin123");

            if (admin != null)
            {
                // Overwrite existing admin with Id = 1
                admin.FirstName = "payfair";
                admin.LastName = "admin";
                admin.Username = "admin";
                admin.Email = "payfairadmin@gmail.com";
                admin.IsDeleted = false;
                admin.HashedPassword = hashedPassword;
                db.Users.Update(admin);
            }
            else
            {
                // Create new admin with Id = 1
                admin = new AppUser
                {
                    Id = 1,
                    FirstName = "payfair",
                    LastName = "admin",
                    Username = "admin",
                    Email = "payfairadmin@gmail.com",
                    IsDeleted = false,
                    HashedPassword = hashedPassword
                };
                db.Users.Add(admin);
            }
            db.SaveChanges();
        }

        private static string HashPassword(string password)
        {
            using (var sha256 = SHA256.Create())
            {
                var bytes = sha256.ComputeHash(Encoding.UTF8.GetBytes(password));
                return BitConverter.ToString(bytes).Replace("-", "").ToLower();
            }
        }
    }
}
