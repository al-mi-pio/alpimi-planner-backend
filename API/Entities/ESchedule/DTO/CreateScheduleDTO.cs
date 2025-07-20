namespace AlpimiAPI.Entities.ESchedule.DTO
{
    public class CreateScheduleDTO
    {
        [LocalizedRequired]
        public required string? Name { get; set; }

        [LocalizedRequired]
        public required int? SchoolHour { get; set; }

        [LocalizedRequired]
        public required DateOnly? SchoolYearStart { get; set; }

        [LocalizedRequired]
        public required DateOnly? SchoolYearEnd { get; set; }

        [LocalizedRequired]
        public required string? SchoolDays { get; set; }
    }
}
