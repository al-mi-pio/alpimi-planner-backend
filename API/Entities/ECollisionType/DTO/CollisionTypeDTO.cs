namespace AlpimiAPI.Entities.ECollisionType.DTO
{
    public class CollisionTypeDTO
    {
        public required Guid Id { get; set; }

        public required string Name { get; set; }

        public required string Description { get; set; }

        public required double Weight { get; set; }

        public string? Filter { get; set; }

        public required string Category { get; set; }
    }
}
