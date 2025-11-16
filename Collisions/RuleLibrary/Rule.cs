using AlpimiAPI.Entities.EAvailability;
using AlpimiAPI.Entities.EDayOff;
using AlpimiAPI.Entities.EScheduleSettings;
using alpimi_planner_backend.Collisions.CollisionDataObjects;
using alpimi_planner_backend.Collisions.CollisionUtils;

namespace alpimi_planner_backend.Collisions.RuleLibrary
{
    public class Rule
    {
        public virtual bool CheckBlock(
            CollisionLessonBlock objectBlock,
            DateOnly scanDate,
            CollisionScanBlocks collisionScanBlocks,
            ScheduleSettings scheduleSettings,
            CollisionScanStaticData collisionScanStaticData,
            LogMaker logMaker
        )
        {
            return false;
        }

        public virtual bool CheckPair(
            CollisionLessonBlock objectBlock,
            CollisionLessonBlock targetBlock,
            DateOnly scanDate,
            CollisionScanBlocks collisionScanBlocks,
            ScheduleSettings scheduleSettings,
            CollisionScanStaticData collisionScanStaticData,
            LogMaker logMaker
        )
        {
            return false;
        }

        public virtual List<Guid> CheckGroup(
            List<CollisionLessonBlock> targetBlocks,
            DateOnly scanDate,
            CollisionScanBlocks collisionScanBlocks,
            ScheduleSettings scheduleSettings,
            CollisionScanStaticData collisionScanStaticData,
            LogMaker logMaker
        )
        {
            return new List<Guid>();
        }

        public virtual bool CheckDayOff(
            CollisionLessonBlock objectBlock,
            DayOff dayOff,
            DateOnly scanDate,
            CollisionScanBlocks collisionScanBlocks,
            ScheduleSettings scheduleSettings,
            CollisionScanStaticData collisionScanStaticData,
            LogMaker logMaker
        )
        {
            return false;
        }

        public virtual bool CheckAvailability(
            CollisionLessonBlock objectBlock,
            Availability availability,
            DateOnly scanDate,
            CollisionScanBlocks collisionScanBlocks,
            ScheduleSettings scheduleSettings,
            CollisionScanStaticData collisionScanStaticData,
            LogMaker logMaker
        )
        {
            return false;
        }

        public Rule(string data) { }
    }
}
