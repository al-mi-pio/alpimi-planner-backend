namespace AlpimiAPI.Entities.EStudent.DTO
{
    public class UpdateStudentDTO
    {
        public string? AlbumNumber { get; set; }
        public IEnumerable<Guid>? SubgroupIds { get; set; }
    }
}
