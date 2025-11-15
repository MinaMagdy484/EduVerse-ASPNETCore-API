using System.ComponentModel.DataAnnotations;

namespace ExaminationSystem.ViewModels.CourseReview
{
    public class CourseReviewCreateRequestModel
    {
        [Required]
        public int CourseID { get; set; }
        
        [Required]
        public int StudentID { get; set; }
        
        [Required]
        [Range(1, 5, ErrorMessage = "Rating must be between 1 and 5")]
        public int Rating { get; set; }
          [MaxLength(1000, ErrorMessage = "Comment cannot exceed 1000 characters")]
        public string? Comment { get; set; }
    }
}
