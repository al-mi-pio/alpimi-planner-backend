using System.ComponentModel.DataAnnotations;

namespace AlpimiAPI.Entities.EStudent.DTO
{
    public class CreateStudentDTO
    {
        [Required]
        public required string AlbumNumber { get; set; }

        [Required]
        public required Guid GroupId { get; set; }

        public IEnumerable<Guid>? SubgroupIds { get; set; }
    }
}
