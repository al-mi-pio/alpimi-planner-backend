namespace AlpimiAPI.Entities.ELessonPeriod.DTO
{
    public class LessonPeriodDTO
    {
        public required Guid Id { get; set; }

        public required TimeOnly Start { get; set; }

        public required TimeOnly Finish { get; set; }
    }
}
