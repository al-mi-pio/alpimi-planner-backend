using AlpimiAPI.Entities.ECollision;

namespace alpimi_planner_backend.Collisions.CollisionDataObjects
{
    public class CollisionAddList
    {
        public List<CollisionBase> ObjectCollisions { get; set; }

        public List<CollisionBase> TargetCollisions { get; set; }

        public List<CollisionBase> ObjectTargetCollisions { get; set; }

        public CollisionAddList()
        {
            ObjectCollisions = new List<CollisionBase>();
            TargetCollisions = new List<CollisionBase>();
            ObjectTargetCollisions = new List<CollisionBase>();
        }
    }
}
