// File: ViewModels/Comment/CommentCreateModel.cs
namespace ExaminationSystem.ViewModels.Comment
{
    public class CommentCreateModel
    {
        public int TimelineItemId { get; set; }
        public int UserId { get; set; }
        public string Content { get; set; }
    }
}
