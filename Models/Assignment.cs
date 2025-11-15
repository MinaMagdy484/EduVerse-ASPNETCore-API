using ExaminationSystem.Models;
using System.ComponentModel.DataAnnotations.Schema;
namespace ExaminationSystem.Models
{
    public class Assignment : BaseModel
    {
        [ForeignKey("TimeLineItem")]
        public int TimeLineItemID { get; set; }
        public TimeLineItem TimeLineItem { get; set; }

        public DateTime Deadline { get; set; }

        //want to edit
        public List<AssignmentAllowedExtension> AllowedExtensions { get; set; } = new List<AssignmentAllowedExtension>();

        public List<AssignmentSubmission> Submissions { get; set; } = new List<AssignmentSubmission>();
    }
}