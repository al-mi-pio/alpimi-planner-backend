using System.ComponentModel.DataAnnotations;

namespace AlpimiAPI.Entities.ECollisionType.DTO
{
    public class CreateCollisionTypeDTO
    {
        [Required]
        public required string Name { get; set; }

        [Required]
        public required string Description { get; set; }

        [Required]
        public required double Weight { get; set; }

        public string? Filter { get; set; }

        [Required]
        public required string Category { get; set; }

        [Required]
        public required Guid ScheduleId { get; set; }
    }
}
