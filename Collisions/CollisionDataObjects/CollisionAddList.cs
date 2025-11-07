using AlpimiAPI.Entities.ECollision;

namespace alpimi_planner_backend.Collisions.CollisionDataObjects
{
    public class CollisionAddList
    {
        public List<Collision> ObjectCollisions { get; set; }

        public List<Collision> TargetCollisions { get; set; }

        public List<Collision> ObjectTargetCollisions { get; set; }

        public CollisionAddList()
        {
            ObjectCollisions = new List<Collision>();
            TargetCollisions = new List<Collision>();
            ObjectTargetCollisions = new List<Collision>();
        }
    }
}
