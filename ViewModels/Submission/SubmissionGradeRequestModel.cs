// File: ViewModels/Submission/SubmissionGradeRequestModel.cs
namespace ExaminationSystem.ViewModels.Submission
{
    public class SubmissionGradeRequestModel
    {
        public int SubmissionId { get; set; }
        public int InstructorId { get; set; }
        public int Grade { get; set; }
        public string Feedback { get; set; }
    }
}
