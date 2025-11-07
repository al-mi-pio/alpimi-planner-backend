using AlpimiAPI.Entities.EDayOff;
using AlpimiAPI.Entities.ELessonBlock;

namespace alpimi_planner_backend.Collisions.CollisionDataObjects
{
    public class CollisionScanBlocks
    {
        public List<CollisionLessonBlock> lessonBlocks { get; set; }
        public List<DayOff> daysOffFull { get; set; }

        public CollisionScanBlocks()
        {
            lessonBlocks = new List<CollisionLessonBlock>();
            daysOffFull = new List<DayOff>();
        }
    }
}
