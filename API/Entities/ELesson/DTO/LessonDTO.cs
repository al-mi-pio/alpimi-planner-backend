using AlpimiAPI.Entities.ELessonType.DTO;
using AlpimiAPI.Entities.ESubgroup.DTO;

namespace AlpimiAPI.Entities.ELesson.DTO
{
    public class LessonDTO
    {
        public required Guid Id { get; set; }
        public required string Name { get; set; }

        public required int AmountOfHours { get; set; }

        public required LessonTypeDTO LessonType { get; set; }

        public required SubgroupDTO Subgroup { get; set; }
    }
}
