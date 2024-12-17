using System.ComponentModel.DataAnnotations;

namespace AlpimiAPI.Entities.ETeacher.DTO
{
    public class CreateTeacherDTO
    {
        [Required]
        public required string Name { get; set; }

        [Required]
        public required string Surname { get; set; }

        [Required]
        public required Guid ScheduleId { get; set; }
    }
}
