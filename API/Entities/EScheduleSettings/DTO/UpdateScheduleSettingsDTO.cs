namespace AlpimiAPI.Entities.EScheduleSettings.DTO
{
    public class UpdateScheduleSettingsDTO
    {
        public int? SchoolHour { get; set; }

        public DateOnly? SchoolYearStart { get; set; }

        public DateOnly? SchoolYearEnd { get; set; }

        public string? SchoolDays { get; set; }
    }
}
