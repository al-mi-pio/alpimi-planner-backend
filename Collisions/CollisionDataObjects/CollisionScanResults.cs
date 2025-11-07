namespace alpimi_planner_backend.Collisions.CollisionDataObjects
{
    public class CollisionScanResults
    {
        public CollisionRemoveList RemoveList { get; set; }

        public CollisionAddList AddList { get; set; }

        public CollisionScanResults()
        {
            AddList = new CollisionAddList();
            RemoveList = new CollisionRemoveList();
        }
    }
}
