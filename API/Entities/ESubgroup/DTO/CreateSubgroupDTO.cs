namespace AlpimiAPI.Entities.ESubgroup.DTO
{
    public class CreateSubgroupDTO
    {
        [LocalizedRequired]
        public required string? Name { get; set; }

        [LocalizedRequired]
        public required int? StudentCount { get; set; }

        [LocalizedRequired]
        public required Guid? GroupId { get; set; }
    }
}
