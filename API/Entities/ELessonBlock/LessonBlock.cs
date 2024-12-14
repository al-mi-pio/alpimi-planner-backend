using AlpimiAPI.Entities.EClassroom;
using AlpimiAPI.Entities.ELesson;
using AlpimiAPI.Entities.ETeacher;

namespace AlpimiAPI.Entities.ELessonBlock
{
    public class LessonBlock
    {
        public Guid Id { get; set; }
        public required DateOnly LessonDate { get; set; }
        public required int LessonStart { get; set; }
        public required int LessonEnd { get; set; }
        public Guid LessonId { get; set; }
        public required Lesson Lesson { get; set; }
        public Guid? ClassroomId { get; set; }
        public Classroom? Classroom { get; set; }
        public Guid? TeacherId { get; set; }
        public Teacher? Teacher { get; set; }
        public required Guid ClusterId { get; set; }
    }
}
