using AlpimiAPI.Entities.EScheduleSettings;

namespace AlpimiAPI.Entities.ELessonPeriod
{
    public class LessonPeriod
    {
        public Guid Id { get; set; }
        public required TimeOnly Start { get; set; }
        public required TimeOnly Finish { get; set; }
        public Guid ScheduleSettingsId { get; set; }
        public required ScheduleSettings ScheduleSettings { get; set; }
    }
}
