// File: ViewModels/Post/PostEditRequestModel.cs
using ExaminationSystem.ViewModels.Attachment;
using System.Collections.Generic;

namespace ExaminationSystem.ViewModels.Post
{
    public class PostEditRequestModel
    {
        public int Id { get; set; }
        public int InstructorID { get; set; }
        public string Title { get; set; }
        public string Content { get; set; }
        public List<AttachmentInfoModel> Attachments { get; set; }
    }
}

