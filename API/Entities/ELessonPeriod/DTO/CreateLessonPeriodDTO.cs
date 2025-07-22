namespace AlpimiAPI.Entities.ELessonPeriod.DTO
{
    public class CreateLessonPeriodDTO
    {
        [LocalizedRequired]
        public required TimeOnly? Start { get; set; }

        [LocalizedRequired]
        public required Guid? ScheduleId { get; set; }
    }
}
