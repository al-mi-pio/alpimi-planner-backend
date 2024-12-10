namespace AlpimiAPI.Entities.ELesson.DTO
{
    public class UpdateLessonDTO
    {
        public string? Name { get; set; }

        public int? AmountOfHours { get; set; }

        public IEnumerable<Guid>? ClassroomTypeIds { get; set; }
    }
}
