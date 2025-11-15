using System.ComponentModel.DataAnnotations.Schema;

namespace ExaminationSystem.Models
{
    public class AssignmentAllowedExtension : BaseModel
    {
        [ForeignKey("Assignment")]
        public int AssignmentID { get; set; }
        public Assignment Assignment { get; set; }

        public string Extension { get; set; }
    }
}