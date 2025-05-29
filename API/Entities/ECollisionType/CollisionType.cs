using AlpimiAPI.Entities.ESchedule;

namespace AlpimiAPI.Entities.ECollisionType
{
    public class CollisionType
    {
        public Guid Id { get; set; }

        public required string Name { get; set; }

        public required string Description { get; set; }

        public required Double Weight { get; set; }

        public string? Filter { get; set; }

        public required string Category { get; set; }

        public required Guid ScheduleId { get; set; }

        public required Schedule Schedule { get; set; }
    }
}
