using ExaminationSystem.Models;
using System.ComponentModel.DataAnnotations.Schema;

namespace ExaminationSystem.Models
{
    public class Attachment : BaseModel
    {
        public string FileName { get; set; }
        public string FilePath { get; set; }
        public string FileType { get; set; }
        public long FileSize { get; set; }
        public DateTime UploadDate { get; set; }

        // TimelineItem attachments

        [ForeignKey("TimeLineItem")]
        public int? TimeLineItemID { get; set; }
        public TimeLineItem TimeLineItem { get; set; }

        // AssignmentSubmission attachments
        [ForeignKey("AssignmentSubmission")]
        public int? AssignmentSubmissionID { get; set; }
        public AssignmentSubmission AssignmentSubmission { get; set; }
    }
}