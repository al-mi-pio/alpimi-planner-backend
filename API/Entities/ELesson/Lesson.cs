using AlpimiAPI.Entities.ELessonType;
using AlpimiAPI.Entities.ESubgroup;

namespace AlpimiAPI.Entities.ELesson
{
    public class Lesson
    {
        public Guid Id { get; set; }
        public required string Name { get; set; }
        public required int CurrentHours { get; set; }
        public required int AmountOfHours { get; set; }
        public required Guid LessonTypeId { get; set; }
        public required LessonType LessonType { get; set; }
        public required Guid SubgroupId { get; set; }
        public required Subgroup Subgroup { get; set; }
    }
}
