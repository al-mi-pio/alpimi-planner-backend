namespace AlpimiAPI.Entities.ELessonBlock.DTO
{
    public class CreateLessonBlockDTO
    {
        [LocalizedRequired]
        public required DateOnly? LessonDate { get; set; }

        [LocalizedRequired]
        public required int? LessonStart { get; set; }

        [LocalizedRequired]
        public required int? LessonEnd { get; set; }

        [LocalizedRequired]
        public required Guid LessonId { get; set; }

        public Guid? ClassroomId { get; set; }

        public int? WeekInterval { get; set; }
    }
}
