using System.ComponentModel.DataAnnotations;

namespace AlpimiAPI.Entities.ELessonPeriod.DTO
{
    public class CreateLessonPeriodDTO
    {
        [Required]
        public required TimeOnly Start { get; set; }

        [Required]
        public required Guid ScheduleId { get; set; }
    }
}
