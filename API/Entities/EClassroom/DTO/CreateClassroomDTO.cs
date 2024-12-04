using System.ComponentModel.DataAnnotations;

namespace AlpimiAPI.Entities.EClassroom.DTO
{
    public class CreateClassroomDTO
    {
        [Required]
        public required string Name { get; set; }

        [Required]
        public required int Capacity { get; set; }

        [Required]
        public required Guid ScheduleId { get; set; }
    }
}
