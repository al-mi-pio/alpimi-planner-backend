namespace AlpimiAPI.Entities.ESchedule.DTO
{
    public class ScheduleDTO
    {
        public required Guid Id { get; set; }

        public required string Name { get; set; }

        public required DateTime ModifyDate { get; set; }
    }
}
