using System.ComponentModel.DataAnnotations;

namespace AlpimiAPI.Entities.EDayOff.DTO
{
    public class CreateDayOffDTO
    {
        [Required]
        public required string Name { get; set; }

        [Required]
        public required DateOnly From { get; set; }

        public DateOnly? To { get; set; }

        [Required]
        public required Guid ScheduleId { get; set; }
    }
}
