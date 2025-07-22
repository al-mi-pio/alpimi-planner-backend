namespace AlpimiAPI.Entities.ELessonType.DTO
{
    public class CreateLessonTypeDTO
    {
        [LocalizedRequired]
        public required string? Name { get; set; }

        [LocalizedRequired]
        public required int? Color { get; set; }

        [LocalizedRequired]
        public required Guid? ScheduleId { get; set; }
    }
}
