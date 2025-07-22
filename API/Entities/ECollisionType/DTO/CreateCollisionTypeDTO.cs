namespace AlpimiAPI.Entities.ECollisionType.DTO
{
    public class CreateCollisionTypeDTO
    {
        [LocalizedRequired]
        public required string? Name { get; set; }

        [LocalizedRequired]
        public required string? Description { get; set; }

        [LocalizedRequired]
        public required double? Weight { get; set; }

        public string? Filter { get; set; }

        [LocalizedRequired]
        public required string Category { get; set; }

        [LocalizedRequired]
        public required Guid? ScheduleId { get; set; }
    }
}
