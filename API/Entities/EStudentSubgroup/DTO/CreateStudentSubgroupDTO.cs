using System.ComponentModel.DataAnnotations;

namespace AlpimiAPI.Entities.EStudentSubgroup.DTO
{
    public class CreateStudentSubgroupDTO
    {
        [Required]
        public required Guid StudentId { get; set; }

        [Required]
        public required IEnumerable<Guid> SubgroupIds { get; set; }
    }
}
