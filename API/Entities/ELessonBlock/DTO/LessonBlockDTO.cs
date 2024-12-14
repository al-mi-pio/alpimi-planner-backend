using AlpimiAPI.Entities.EClassroom.DTO;
using AlpimiAPI.Entities.ELesson.DTO;
using AlpimiAPI.Entities.ETeacher.DTO;

namespace AlpimiAPI.Entities.ELessonBlock
{
    public class LessonBlockDTO
    {
        public Guid Id { get; set; }
        public required DateOnly LessonDate { get; set; }
        public required int LessonStart { get; set; }
        public required int LessonEnd { get; set; }
        public required LessonDTO Lesson { get; set; }
        public ClassroomDTO? Classroom { get; set; }
        public TeacherDTO? Teacher { get; set; }
        public required Guid ClusterId { get; set; }
    }
}
