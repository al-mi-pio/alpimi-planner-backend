namespace AlpimiAPI.Entities.ELesson.DTO
{
    public class CreateLessonDTO
    {
        [LocalizedRequired]
        public required string? Name { get; set; }

        [LocalizedRequired]
        public required int? AmountOfHours { get; set; }

        [LocalizedRequired]
        public required Guid? LessonTypeId { get; set; }

        [LocalizedRequired]
        public required Guid? TeacherId { get; set; }

        public IEnumerable<Guid>? ClassroomTypeIds { get; set; }

        [LocalizedRequired]
        public required IEnumerable<Guid>? SubgroupIds { get; set; }
    }
}
