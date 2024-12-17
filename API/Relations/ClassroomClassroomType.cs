using AlpimiAPI.Entities.EClassroom;

namespace AlpimiAPI.Relations
{
    public class ClassroomClassroomType
    {
        public Guid Id { get; set; }

        public required Guid ClassroomId { get; set; }

        public required Classroom Classroom { get; set; }

        public required Guid ClassroomTypeId { get; set; }
    }
}
