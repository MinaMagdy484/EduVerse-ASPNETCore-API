// File: ViewModels/Submission/StudentSubmissionResponseModel.cs
using ExaminationSystem.ViewModels.Attachment;
using System;
using System.Collections.Generic;

namespace ExaminationSystem.ViewModels.Submission
{
    public class StudentSubmissionResponseModel
    {
        public int AssignmentId { get; set; }
        public string AssignmentTitle { get; set; }
        public DateTime Deadline { get; set; }
        public bool IsSubmitted { get; set; }
        public int SubmissionId { get; set; }
        public DateTime? SubmissionDate { get; set; }
        public int Grade { get; set; }
        public string Feedback { get; set; }
        public List<AttachmentResponseModel> Attachments { get; set; }
    }
}
