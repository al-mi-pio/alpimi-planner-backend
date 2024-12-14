using System.ComponentModel.DataAnnotations;

namespace AlpimiAPI.Entities.ELessonBlock.DTO
{
    public class CreateLessonBlockDTO
    {
        [Required]
        public required DateOnly LessonDate { get; set; }

        [Required]
        public required int LessonStart { get; set; }

        [Required]
        public required int LessonEnd { get; set; }

        [Required]
        public required Guid LessonId { get; set; }
        public Guid? ClassroomId { get; set; }
        public Guid? TeacherId { get; set; }
        public int? WeekInterval { get; set; }
    }
}
