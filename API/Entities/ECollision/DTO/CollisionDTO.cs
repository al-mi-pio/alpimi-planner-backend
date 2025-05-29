using AlpimiAPI.Entities.ECollisionType.DTO;

namespace AlpimiAPI.Entities.ECollision.DTO
{
    public class CollisionDTO
    {
        public Guid Id { get; set; }

        public required string CollidingObject1 { get; set; }

        public string? CollidingObject2 { get; set; }

        public required bool Ignored { get; set; }

        public required CollisionTypeDTO CollisionType { get; set; }
    }
}
