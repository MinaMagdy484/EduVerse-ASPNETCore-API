using ExaminationSystem.DTO.Exam;

namespace ExaminationSystem.ViewModels.Exam
{
    public class ExamStudentViewModel
    {
        // Remove or comment out the Id property since it's not needed for submission
        public int StudentId { get; set; }
        public int ExamId { get; set; }
        public List<ExamAnswerDTO> Answers { get; set; }
    }
}
