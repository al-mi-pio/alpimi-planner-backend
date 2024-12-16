using AlpimiAPI.Entities.ESchedule;

namespace AlpimiAPI.Entities.ETeacher
{
    public class Teacher
    {
        public Guid Id { get; set; }

        public required string Name { get; set; }

        public required string Surname { get; set; }

        public required Guid ScheduleId { get; set; }

        public required Schedule Schedule { get; set; }
    }
}
