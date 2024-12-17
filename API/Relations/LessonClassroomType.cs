using AlpimiAPI.Entities.ELesson;

namespace AlpimiAPI.Relations
{
    public class LessonClassroomType
    {
        public Guid Id { get; set; }

        public required Guid LessonId { get; set; }

        public required Lesson Lesson { get; set; }

        public required Guid ClassroomTypeId { get; set; }
    }
}
