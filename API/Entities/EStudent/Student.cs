using AlpimiAPI.Entities.EGroup;

namespace AlpimiAPI.Entities.EStudent
{
    public class Student
    {
        public Guid Id { get; set; }
        public required string AlbumNumber { get; set; }
        public required Guid GroupId { get; set; }
        public required Group Group { get; set; }
    }
}
