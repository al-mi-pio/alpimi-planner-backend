using AlpimiAPI.Entities.ELessonType.DTO;

namespace AlpimiAPI.Entities.ELesson.DTO
{
    public class LessonDTO
    {
        public required Guid Id { get; set; }
        public required string Name { get; set; }

        public required int AmountOfHours { get; set; }

        public required LessonTypeDTO LessonType { get; set; }

        public required Guid SubgroupId { get; set; } // podczas dodawania LessonBlocka zamień to na SubgroupDTO
    }
}
