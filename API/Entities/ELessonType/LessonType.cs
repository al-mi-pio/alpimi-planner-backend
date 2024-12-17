using AlpimiAPI.Entities.ESchedule;

namespace AlpimiAPI.Entities.ELessonType
{
    public class LessonType
    {
        public Guid Id { get; set; }

        public required string Name { get; set; }

        public required int Color { get; set; }

        public required Guid ScheduleId { get; set; }

        public required Schedule Schedule { get; set; }
    }
}
