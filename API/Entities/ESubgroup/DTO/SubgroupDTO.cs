using AlpimiAPI.Entities.EGroup.DTO;
using AlpimiAPI.Entities.ELesson.DTO;

namespace AlpimiAPI.Entities.ESubgroup.DTO
{
    public class SubgroupDTO
    {
        public required Guid Id { get; set; }

        public required string Name { get; set; }

        public required int StudentCount { get; set; }

        public required IEnumerable<LessonDTO> Lessons { get; set; }

        public required GroupDTO Group { get; set; }
    }
}
