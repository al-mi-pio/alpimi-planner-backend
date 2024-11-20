using AlpimiAPI.Entities.EUser;

namespace AlpimiAPI.Entities.ESchedule
{
    public class Schedule
    {
        public Guid Id { get; set; }
        public required string Name { get; set; }
        public required Guid UserId { get; set; }
        public required User User { get; set; }
    }
}
