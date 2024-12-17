using System.ComponentModel.DataAnnotations;

namespace AlpimiAPI.Entities.EClassroomType.DTO
{
    public class CreateClassroomTypeDTO
    {
        [Required]
        public required string Name { get; set; }

        [Required]
        public required Guid ScheduleId { get; set; }
    }
}
