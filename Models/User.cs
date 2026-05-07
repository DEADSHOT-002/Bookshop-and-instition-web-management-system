using System.ComponentModel.DataAnnotations;

namespace BookshopTuitionSystem.Models
{
    public class User
    {
        public int UserId { get; set; }
        [Required]
        public string FullName { get; set; } = "";
        public string Username { get; set; } = "";
        public string PasswordHash { get; set; } = "";
        public string Role { get; set; } = "Staff";
        public string Status { get; set; } = "Active";
    }
}