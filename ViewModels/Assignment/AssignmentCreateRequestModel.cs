// File: ViewModels/Assignment/AssignmentCreateRequestModel.cs
using ExaminationSystem.ViewModels.Attachment;
using System;
using System.Collections.Generic;

namespace ExaminationSystem.ViewModels.Assignment
{
    public class AssignmentCreateRequestModel
    {
        public int CourseID { get; set; }
        public int InstructorID { get; set; }
        public string Title { get; set; }
        public string Content { get; set; }
        public DateTime Deadline { get; set; }
        public List<string> AllowedExtensions { get; set; }
        public List<AttachmentInfoModel> Attachments { get; set; }
    }
}
