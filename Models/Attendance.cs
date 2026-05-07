using System.ComponentModel.DataAnnotations;

namespace BookshopTuitionSystem.Models
{
    public class Attendance
    {
        public int AttendanceId { get; set; }

        public int StudentId { get; set; }

        [Required]
        public DateTime Date { get; set; }

        public string Grade { get; set; } = "";

        public string Subject { get; set; } = "";

        public string Status { get; set; } = "";

        public Student? Student { get; set; }
    }
}