using AlpimiAPI.Entities.EScheduleSettings;
using AlpimiAPI.Entities.EUser;

namespace AlpimiAPI.Entities.EDayOff
{
    public class DayOff
    {
        public Guid Id { get; set; }
        public required string Name { get; set; }
        public required DateTime Date { get; set; }
        public Guid ScheduleSettingsId { get; set; }
        public required ScheduleSettings ScheduleSettings { get; set; }
    }
}
