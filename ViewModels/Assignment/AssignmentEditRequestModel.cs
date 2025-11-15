// File: ViewModels/Assignment/AssignmentEditRequestModel.cs
using ExaminationSystem.ViewModels.Attachment;
using System;
using System.Collections.Generic;

namespace ExaminationSystem.ViewModels.Assignment
{
    public class AssignmentEditRequestModel
    {
        public int Id { get; set; }
        public int InstructorID { get; set; }
        public string Title { get; set; }
        public string Content { get; set; }
        public DateTime Deadline { get; set; }
        public List<string> AllowedExtensions { get; set; }
        public List<AttachmentInfoModel> Attachments { get; set; }
    }
}
