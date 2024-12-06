using AlpimiAPI.Entities.ELessonType;

namespace AlpimiAPI.Entities.ELesson
{
    public class Lesson
    {
        public Guid Id { get; set; }
        public required string Name { get; set; }
        public required int AmountOfHours { get; set; }
        public required Guid LessonTypeId { get; set; }
        public required LessonType LessonType { get; set; }
        public required Guid SubgroupId { get; set; }
    }
}
