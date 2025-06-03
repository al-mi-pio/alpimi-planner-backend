using System.ComponentModel.DataAnnotations;

namespace AlpimiAPI.Entities.EAvailability.DTO
{
    public class CreateavAvailabilityDTO
    {
        [Required]
        public required int WeekDay { get; set; } //ex. 0 means Sunday

        [Required]
        public required int Start { get; set; }

        [Required]
        public required int End { get; set; }

        [Required]
        public required Guid TeacherId { get; set; }
    }
}
