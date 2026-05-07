using System.ComponentModel.DataAnnotations;

namespace BookshopTuitionSystem.Models
{
    public class Teacher
    {
        public int TeacherId { get; set; }

        public int UserId { get; set; }

        [Required]
        public string FullName { get; set; } = "";

        public string Address { get; set; } = "";

        public string Subject { get; set; } = "";

        public string Qualifications { get; set; } = "";

        public string Email { get; set; } = "";

        public string ContactNo { get; set; } = "";
    }
}