using AlpimiAPI.Entities.ESchedule;

namespace AlpimiAPI.Entities.EScheduleSettings
{
    public class ScheduleSettings
    {
        public Guid Id { get; set; }
        public required int SchoolHour { get; set; }
        public required DateOnly SchoolYearStart { get; set; }
        public required DateOnly SchoolYearEnd { get; set; }
        public required string SchoolDays { get; set; }
        public required Guid ScheduleId { get; set; }
        public required Schedule Schedule { get; set; }
    }
}
