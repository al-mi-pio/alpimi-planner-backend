namespace AlpimiAPI.Entities.EData.DTO
{
    public class ImportDataDTO
    {
        [LocalizedRequired]
        public required string Payload { get; set; }

        [LocalizedRequired]
        public required Guid ScheduleId { get; set; }
    }
}
