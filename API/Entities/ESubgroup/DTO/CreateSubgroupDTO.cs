using System.ComponentModel.DataAnnotations;

namespace AlpimiAPI.Entities.ESubgroup.DTO
{
    public class CreateSubgroupDTO
    {
        [Required]
        public required string Name { get; set; }

        [Required]
        public required int StudentCount { get; set; }

        [Required]
        public required Guid GroupId { get; set; }
    }
}
