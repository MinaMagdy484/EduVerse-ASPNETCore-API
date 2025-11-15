// File: ViewModels/Post/PostCreateRequestModel.cs
using ExaminationSystem.ViewModels.Attachment;
using System.Collections.Generic;

namespace ExaminationSystem.ViewModels.Post
{
    public class PostCreateRequestModel
    {
        public int CourseID { get; set; }
        public int InstructorID { get; set; }
        public string Title { get; set; }
        public string Content { get; set; }
        public List<AttachmentInfoModel> Attachments { get; set; }
    }
}
