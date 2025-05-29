using AlpimiAPI.Entities.ECollisionType;

namespace AlpimiAPI.Entities.ECollision
{
    public class Collision
    {
        public Guid Id { get; set; }

        public required string CollidingObject1 { get; set; }

        public string? CollidingObject2 { get; set; }

        public required bool Ignored { get; set; }

        public required Guid CollisionTypeId { get; set; }

        public required CollisionType CollisionType { get; set; }
    }
}
