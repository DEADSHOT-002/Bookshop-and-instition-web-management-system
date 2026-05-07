using System.Linq;
using BookshopTuitionSystem.Models;
using Microsoft.AspNetCore.Identity;

namespace BookshopTuitionSystem.Data
{
    public static class DbInitializer
    {
        public static void Seed(AppDbContext db)
        {
            if (db.Users.Any()) return;

            var hasher = new PasswordHasher<User>();

            var admin = new User
            {
                FullName = "System Admin",
                Username = "admin",
                Role = "Owner/Admin",
                Status = "Active"
            };

            admin.PasswordHash = hasher.HashPassword(admin, "Admin@123");

            db.Users.Add(admin);
            db.SaveChanges();
        }
    }
}