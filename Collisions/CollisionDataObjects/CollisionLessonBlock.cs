using AlpimiAPI.Entities.EClassroom;
using AlpimiAPI.Entities.EClassroomType;
using AlpimiAPI.Entities.ELesson;
using AlpimiAPI.Entities.ESubgroup;

namespace alpimi_planner_backend.Collisions.CollisionDataObjects
{
    public class CollisionLessonBlock
    {
        public Guid Id { get; set; }

        public DateOnly LessonDate { get; set; }

        public int LessonStart { get; set; }

        public int LessonEnd { get; set; }

        public Guid LessonId { get; set; }

        public Lesson? Lesson { get; set; }

        public Guid? ClassroomId { get; set; }

        public Classroom? Classroom { get; set; }

        public Guid ClusterId { get; set; }

        public IEnumerable<Subgroup>? Subgroups { get; set; }

        public IEnumerable<ClassroomType>? ClassroomTypeslesson { get; set; }

        public IEnumerable<ClassroomType>? ClassroomTypesClassroom { get; set; }
    }
}
