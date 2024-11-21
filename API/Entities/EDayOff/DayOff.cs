using AlpimiAPI.Entities.EScheduleSettings;

namespace AlpimiAPI.Entities.EDayOff
{
    public class DayOff
    {
        public Guid Id { get; set; }
        public required string Name { get; set; }
        public required DateTime From { get; set; }
        public required DateTime To { get; set; }
        public Guid ScheduleSettingsId { get; set; }
        public required ScheduleSettings ScheduleSettings { get; set; }
    }
}
