namespace AlpimiAPI.Entities.EGroup.DTO
{
    public class GroupDTO
    {
        public required Guid Id { get; set; }

        public required string Name { get; set; }

        public required int StudentCount { get; set; }
    }
}
