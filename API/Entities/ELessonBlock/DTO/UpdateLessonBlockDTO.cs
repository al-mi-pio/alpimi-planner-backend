using System.ComponentModel.DataAnnotations;

namespace AlpimiAPI.Entities.ELessonBlock.DTO
{
    public class UpdateLessonBlockDTO
    {
        public int? WeekDay { get; set; } //ex. 0 means Sunday

        public int? LessonStart { get; set; }

        public int? LessonEnd { get; set; }

        public Guid? ClassroomId { get; set; }

        public Guid? TeacherId { get; set; }

        [Required]
        public required bool UpdateCluster { get; set; }
    }
}
