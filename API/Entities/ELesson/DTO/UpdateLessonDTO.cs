namespace AlpimiAPI.Entities.ELesson.DTO
{
    public class UpdateLessonDTO
    {
        public string? Name { get; set; }

        public int? AmountOfHours { get; set; }

        public Guid? LessonTypeId { get; set; }

        public Guid? TeacherId { get; set; }

        public IEnumerable<Guid>? ClassroomTypeIds { get; set; }

        public IEnumerable<Guid>? SubgroupIds { get; set; }
    }
}
