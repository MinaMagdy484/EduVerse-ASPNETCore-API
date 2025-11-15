// File: ViewModels/Submission/SubmissionResponseModel.cs
using ExaminationSystem.ViewModels.Attachment;
using System;
using System.Collections.Generic;

namespace ExaminationSystem.ViewModels.Submission
{
    public class SubmissionResponseModel
    {
        public int Id { get; set; }
        public int StudentId { get; set; }
        public string StudentName { get; set; }
        public DateTime SubmissionDate { get; set; }
        public int Grade { get; set; }
        public string? Feedback { get; set; }
        public List<AttachmentResponseModel> Attachments { get; set; }
    }
}
