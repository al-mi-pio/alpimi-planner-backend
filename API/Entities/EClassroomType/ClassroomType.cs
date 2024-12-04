using AlpimiAPI.Entities.ESchedule;

namespace AlpimiAPI.Entities.EClassroomType
{
    public class ClassroomType
    {
        public Guid Id { get; set; }
        public required string Name { get; set; }
        public required Guid ScheduleId { get; set; }
        public required Schedule Schedule { get; set; }
    }
}
