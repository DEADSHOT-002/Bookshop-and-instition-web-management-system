using Microsoft.EntityFrameworkCore;
using BookshopTuitionSystem.Models;

namespace BookshopTuitionSystem.Data
{
    public class AppDbContext : DbContext
    {
        public AppDbContext(DbContextOptions<AppDbContext> options) : base(options)
        {
        }

        public DbSet<User> Users { get; set; }
        public DbSet<Student> Students { get; set; }
        public DbSet<Item> Items { get; set; }
        public DbSet<Order> Orders { get; set; }
        public DbSet<OrderItem> OrderItems { get; set; }
        public DbSet<Teacher> Teachers { get; set; }
        public DbSet<Staff> Staffs { get; set; }
        public DbSet<TeacherAvailability> TeacherAvailabilities { get; set; }
        public DbSet<Attendance> Attendances { get; set; }
    }
}