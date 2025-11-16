namespace alpimi_planner_backend.Collisions.CollisionDataObjects
{
    public class CollisionRemoveList
    {
        public List<DateOnly> Days { get; set; }

        public List<DateOnly> Weeks { get; set; }

        public List<Guid> LessonBlockIds { get; set; }

        public CollisionRemoveList()
        {
            Days = new List<DateOnly>();
            LessonBlockIds = new List<Guid>();
            Weeks = new List<DateOnly>();
        }
    }
}
