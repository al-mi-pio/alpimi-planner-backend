using System.ComponentModel.DataAnnotations;

namespace AlpimiAPI.Entities.EGroup.DTO
{
    public class CreateGroupDTO
    {
        [Required]
        public required string Name { get; set; }

        [Required]
        public required int StudentCount { get; set; }

        [Required]
        public required Guid ScheduleId { get; set; }
    }
}
