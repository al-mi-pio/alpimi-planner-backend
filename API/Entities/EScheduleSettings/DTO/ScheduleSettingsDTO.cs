namespace AlpimiAPI.Entities.EScheduleSettings.DTO
{
    public class ScheduleSettingsDTO
    {
        public required Guid Id { get; set; }
        public required int SchoolHour { get; set; }
        public required DateOnly SchoolYearStart { get; set; }
        public required DateOnly SchoolYearEnd { get; set; }
        public required string SchoolDays { get; set; }
    }
}
