using ExaminationSystem.Models;
using System.ComponentModel.DataAnnotations.Schema;

namespace ExaminationSystem.Models
{
    public class AssignmentSubmission : BaseModel
    {
        [ForeignKey("Assignment")]
        public int AssignmentID { get; set; }
        public Assignment Assignment { get; set; }

        [ForeignKey("Student")]
        public int StudentID { get; set; }
        public User Student { get; set; }

        public DateTime SubmissionDate { get; set; }
        public int Grade { get; set; }
        public string Feedback { get; set; }

        public List<Attachment> Attachments { get; set; } = new List<Attachment>();
    }
}