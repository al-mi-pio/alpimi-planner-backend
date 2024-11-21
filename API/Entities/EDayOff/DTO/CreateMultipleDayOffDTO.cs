using System.ComponentModel.DataAnnotations;

namespace AlpimiAPI.Entities.EDayOff.DTO
{
    public class CreateMultipleDayOffDTO
    {
        [Required]
        public required string Name { get; set; }

        [Required]
        public required DateTime From { get; set; }

        [Required]
        public required DateTime To { get; set; }

        [Required]
        public required Guid ScheduleId { get; set; }
    }
}
