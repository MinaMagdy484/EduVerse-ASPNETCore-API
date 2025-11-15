// File: ViewModels/Assignment/AssignmentResponseModel.cs
using ExaminationSystem.ViewModels.Attachment;
using System;
using System.Collections.Generic;

namespace ExaminationSystem.ViewModels.Assignment
{
    public class AssignmentResponseModel
    {
        public int Id { get; set; }
        public int CourseID { get; set; }
        public int UserID { get; set; }
        public string Title { get; set; }
        public string Content { get; set; }
        public DateTime CreatedAt { get; set; }
        public DateTime Deadline { get; set; }
        public List<AllowedExtensionResponseModel> AllowedExtensions { get; set; }
        public List<AttachmentResponseModel> Attachments { get; set; }
    }
}
