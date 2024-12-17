using AlpimiAPI.Entities.EGroup.DTO;

namespace AlpimiAPI.Entities.EStudent.DTO
{
    public class StudentDTO
    {
        public required Guid Id { get; set; }

        public required string AlbumNumber { get; set; }

        public required GroupDTO Group { get; set; }
    }
}
