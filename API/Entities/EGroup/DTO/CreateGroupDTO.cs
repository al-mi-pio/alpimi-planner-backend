namespace AlpimiAPI.Entities.EGroup.DTO
{
    public class CreateGroupDTO
    {
        [LocalizedRequired]
        public required string? Name { get; set; }

        [LocalizedRequired]
        public required int? StudentCount { get; set; }

        [LocalizedRequired]
        public required Guid? ScheduleId { get; set; }
    }
}
