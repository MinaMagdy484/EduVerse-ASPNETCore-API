using System;

namespace ExaminationSystem.ViewModels.CourseReview
{
    public class CourseReviewResponseModel
    {
        public int Id { get; set; }        public int CourseID { get; set; }
        public string CourseName { get; set; } = string.Empty;
        public int StudentID { get; set; }
        public string StudentName { get; set; } = string.Empty;
        public int Rating { get; set; }
        public string Comment { get; set; } = string.Empty;
        public DateTime CreatedAt { get; set; }
    }
}
