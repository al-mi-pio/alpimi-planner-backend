using AlpimiAPI.Entities.ELessonType.DTO;
using AlpimiAPI.Entities.ESubgroup.DTO;
using AlpimiAPI.Entities.ETeacher.DTO;

namespace AlpimiAPI.Entities.ELesson.DTO
{
    public class LessonDTO
    {
        public required Guid Id { get; set; }

        public required string Name { get; set; }

        public required int CurrentHours { get; set; }

        public required int AmountOfHours { get; set; }

        public required IEnumerable<SubgroupDTO> Subgroups { get; set; }

        public required LessonTypeDTO LessonType { get; set; }

        public required TeacherDTO Teacher { get; set; }
    }
}
