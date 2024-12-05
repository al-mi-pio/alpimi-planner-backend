using System.ComponentModel.DataAnnotations;

namespace AlpimiAPI.Entities.ELessonType.DTO
{
    public class CreateLessonTypeDTO
    {
        [Required]
        public required string Name { get; set; }

        [Required]
        public required int Color { get; set; }

        [Required]
        public required Guid ScheduleId { get; set; }
    }
}
