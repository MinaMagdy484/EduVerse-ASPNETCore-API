using ExaminationSystem.Models;
using System.ComponentModel.DataAnnotations.Schema;

namespace ExaminationSystem.Models
{
    public class Comment : BaseModel
    {
        [ForeignKey("TimeLineItem")]
        public int TimeLineItemID { get; set; }
        public TimeLineItem TimeLineItem { get; set; }

        public string Content { get; set; }

        [ForeignKey("CommentedBy")]
        public int CommentedByID { get; set; }
        public User CommentedBy { get; set; }

        public DateTime Time { get; set; }
    }
}