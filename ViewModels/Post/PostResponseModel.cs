// File: ViewModels/Post/PostResponseModel.cs
using ExaminationSystem.ViewModels.Attachment;
using System;
using System.Collections.Generic;

namespace ExaminationSystem.ViewModels.Post
{
    public class PostResponseModel
    {
        public int Id { get; set; }
        public int CourseID { get; set; }
        public int UserID { get; set; }
        public string Title { get; set; }
        public string Content { get; set; }
        public DateTime CreatedAt { get; set; }
        public List<AttachmentResponseModel> Attachments { get; set; }
    }
}
