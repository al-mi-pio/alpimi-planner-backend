using AlpimiAPI.Entities.ESchedule;

namespace AlpimiAPI.Entities.EClassroom
{
    public class Classroom
    {
        public Guid Id { get; set; }
        public required string Name { get; set; }
        public required int Capacity { get; set; }
        public required Guid ScheduleId { get; set; }
        public required Schedule Schedule { get; set; }
    }
}
