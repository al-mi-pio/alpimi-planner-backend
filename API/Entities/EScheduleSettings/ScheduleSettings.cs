using AlpimiAPI.Entities.ESchedule;

namespace AlpimiAPI.Entities.EScheduleSettings
{
    public class ScheduleSettings
    {
        public Guid Id { get; set; }
        public required int SchoolHour { get; set; }
        public required DateOnly SchoolYearStart { get; set; }
        public required DateOnly SchoolYearEnd { get; set; }
        public required string SchoolDays { get; set; } // ex. "0110110" this means monday, tuesday, thursday and friday are school days and wendsday, saturday and sunday arent (you cant place lesson blocks inside them)
        public required Guid ScheduleId { get; set; }
        public required Schedule Schedule { get; set; }
    }
}
