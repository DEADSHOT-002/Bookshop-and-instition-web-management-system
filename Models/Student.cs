using System.ComponentModel.DataAnnotations;

namespace BookshopTuitionSystem.Models
{
    public class Student
    {
        public int Id { get; set; }

        [Required]
        public string FullName { get; set; } = "";

        public string Phone { get; set; } = "";

        public string Course { get; set; } = "";

        public string Status { get; set; } = "Active";

        public string Grade { get; set; } = "";

        public string Subject { get; set; } = "";

        public string GuardianName { get; set; } = "";

        public string School { get; set; } = "";

        public string GuardianContact { get; set; } = "";
    }
}