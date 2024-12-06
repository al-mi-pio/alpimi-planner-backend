using System.ComponentModel.DataAnnotations;

namespace AlpimiAPI.Entities.ELesson.DTO
{
    public class CreateLessonDTO
    {
        [Required]
        public required string Name { get; set; }

        [Required]
        public required int AmountOfHours { get; set; }

        [Required]
        public required Guid LessonTypeId { get; set; }

        [Required]
        public required Guid SubgroupId { get; set; }
    }
}
