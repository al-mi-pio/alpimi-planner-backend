namespace AlpimiAPI.Entities.EStudent.DTO
{
    public class CreateStudentDTO
    {
        [LocalizedRequired]
        public required string? AlbumNumber { get; set; }

        [LocalizedRequired]
        public required Guid? GroupId { get; set; }

        public IEnumerable<Guid>? SubgroupIds { get; set; }
    }
}
