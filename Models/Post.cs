using ExaminationSystem.Models;
using System.ComponentModel.DataAnnotations.Schema;

namespace ExaminationSystem.Models
{
    public class Post : BaseModel
    {
        [ForeignKey("TimeLineItem")]
        public int TimeLineItemID { get; set; }
        public TimeLineItem TimeLineItem { get; set; }

        public string Content { get; set; }
        public DateTime CreatedAt { get; set; }
    }

}