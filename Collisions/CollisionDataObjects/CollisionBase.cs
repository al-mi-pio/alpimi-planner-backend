namespace alpimi_planner_backend.Collisions.CollisionDataObjects
{
    public class CollisionBase
    {
        public Guid Id { get; set; }

        public required string CollidingObject1 { get; set; }

        public string? CollidingObject2 { get; set; }

        public required bool Ignored { get; set; }

        public required Guid CollisionTypeId { get; set; }
    }
}
