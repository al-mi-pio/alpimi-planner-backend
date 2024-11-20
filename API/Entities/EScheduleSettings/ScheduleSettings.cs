using AlpimiAPI.Entities.ESchedule;

namespace AlpimiAPI.Entities.EScheduleSettings
{
    public class ScheduleSettings
    {
        public Guid Id { get; set; }
        public required int SchoolHour { get; set; }
        public required DateTime SchoolYearStart { get; set; }
        public required DateTime SchoolYearEnd { get; set; }
        public required Guid ScheduleId { get; set; }
        public required Schedule Schedule { get; set; }
    }
}
