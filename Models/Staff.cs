using System.ComponentModel.DataAnnotations;

namespace BookshopTuitionSystem.Models
{
    public class Staff
    {
        public int StaffId { get; set; }

        public int UserId { get; set; }

        [Required]
        public string FullName { get; set; } = "";

        public int Age { get; set; }

        public string Address { get; set; } = "";

        public string PhotoPath { get; set; } = "";

        public string NIC { get; set; } = "";

        public string ContactNo { get; set; } = "";
    }
}