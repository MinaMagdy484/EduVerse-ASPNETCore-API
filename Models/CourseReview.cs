using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace ExaminationSystem.Models
{
    public class CourseReview :BaseModel
    {
            [ForeignKey("Course")]
        public int CourseID { get; set; }
        public Course? Course { get; set; }

        [ForeignKey("Student")]
        public int StudentID { get; set; }
        public Student? Student { get; set; }

        [Range(1, 5)]
        public int Rating { get; set; }

        [MaxLength(1000)]
        public string? Comment { get; set; }

        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
    }
}
