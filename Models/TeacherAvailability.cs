using BookshopTuitionSystem.Models;
using System;
using System.ComponentModel.DataAnnotations;

namespace BookshopTuitionSystem.Models
{
    public class TeacherAvailability
    {
        public int Id { get; set; }

        public int TeacherId { get; set; }

        [Required]
        public DateTime AvailableDate { get; set; }
    }
}