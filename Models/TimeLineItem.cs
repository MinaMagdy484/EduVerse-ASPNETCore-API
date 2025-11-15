using ExaminationSystem.Enums;
using Microsoft.Extensions.Hosting;
using System.Net.Mail;

namespace ExaminationSystem.Models
{
    public class TimeLineItem : BaseModel
    {
        public int CourseID { get; set; }
        public Course Course { get; set; }

        public int UserID { get; set; }
        public User User { get; set; }

        public TimeLineItemType Type { get; set; }

        public string Title { get; set; }
        public string Content { get; set; }
        public DateTime CreatedAt { get; set; }

        public List<Attachment> Attachments { get; set; } = new List<Attachment>();
        public List<Comment> Comments { get; set; } = new List<Comment>();

        // One-to-one relationships
        public Post? Post { get; set; }
        public Assignment? Assignment { get; set; }
    }
}