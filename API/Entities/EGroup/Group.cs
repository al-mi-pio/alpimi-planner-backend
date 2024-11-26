using AlpimiAPI.Entities.ESchedule;

namespace AlpimiAPI.Entities.EGroup
{
    public class Group
    {
        public Guid Id { get; set; }
        public required string Name { get; set; }
        public required int StudentCount { get; set; }
        public required Guid ScheduleId { get; set; }
        public required Schedule Schedule { get; set; }
    }
}
