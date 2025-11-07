using AlpimiAPI.Entities.EDayOff;
using AlpimiAPI.Entities.ELessonBlock;

namespace alpimi_planner_backend.Collisions.CollisionDataObjects
{
    public class CollisionScanBlocks
    {
        public record DayOffInstance(Guid Id, string Name, DateOnly Date);

        public List<CollisionLessonBlock> lessonBlocks { get; set; }
        public List<DayOff> daysOffFull { get; set; }
        public List<DayOffInstance> daysOff { get; set; }

        public CollisionScanBlocks()
        {
            lessonBlocks = new List<CollisionLessonBlock>();
            daysOffFull = new List<DayOff>();
            daysOff = new List<DayOffInstance>();
        }
    }
}
