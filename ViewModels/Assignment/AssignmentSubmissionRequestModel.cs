// File: ViewModels/Submission/AssignmentSubmissionRequestModel.cs
using ExaminationSystem.ViewModels.Attachment;
using System.Collections.Generic;

namespace ExaminationSystem.ViewModels.Submission
{
    public class AssignmentSubmissionRequestModel
    {
        public int TimelineItemId { get; set; }
        public int StudentId { get; set; }
        public List<AttachmentInfoModel> Attachments { get; set; }
    }
}
