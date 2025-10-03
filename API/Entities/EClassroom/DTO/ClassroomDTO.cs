using AlpimiAPI.Entities.EClassroomType.DTO;

namespace AlpimiAPI.Entities.EClassroom.DTO
{
    public class ClassroomDTO
    {
        public required Guid Id { get; set; }

        public required string Name { get; set; }

        public required int Capacity { get; set; }

        public required IEnumerable<ClassroomTypeDTO> ClassroomTypes { get; set; }
    }
}
