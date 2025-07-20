namespace AlpimiAPI.Entities.EDayOff.DTO
{
    public class CreateDayOffDTO
    {
        [LocalizedRequired]
        public required string? Name { get; set; }

        [LocalizedRequired]
        public required DateOnly? From { get; set; }

        public DateOnly? To { get; set; }

        [LocalizedRequired]
        public required Guid? ScheduleId { get; set; }
    }
}
