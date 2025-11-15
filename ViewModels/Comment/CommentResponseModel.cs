// File: ViewModels/Comment/CommentResponseModel.cs
using System;

namespace ExaminationSystem.ViewModels.Comment
{
    public class CommentResponseModel
    {
        public int Id { get; set; }
        public string Content { get; set; }
        public int CommentedById { get; set; }
        public string CommentedByName { get; set; }
        public string CommentedByImage { get; set; }
        public DateTime Time { get; set; }
        public int TimelineItemId { get; set; }
        public string TimelineItemTitle { get; set; }
    }
}
