using AlpimiAPI.Entities.EScheduleSettings;
using alpimi_planner_backend.Collisions.CollisionDataObjects;
using alpimi_planner_backend.Collisions.CollisionUtils;

namespace alpimi_planner_backend.Collisions.RuleLibrary
{
    public class SameTeacherRule : Rule
    {
        public SameTeacherRule(string data)
            : base(data) { }

        public override bool CheckPair(
            CollisionLessonBlock objectBlock,
            CollisionLessonBlock targetBlock,
            DateOnly scanDate,
            CollisionScanBlocks collisionScanBlocks,
            ScheduleSettings scheduleSettings,
            CollisionScanStaticData collisionScanStaticData,
            LogMaker logMaker
        )
        {
            if (objectBlock != null && targetBlock != null)
            {
                if (objectBlock.Lesson != null && targetBlock.Lesson != null)
                {
                    if (objectBlock.Lesson.Teacher == targetBlock.Lesson.Teacher)
                    {
                        logMaker.writeLog("Blocks have the same teacher");
                        return true;
                    }
                }
            }
            return false;
        }
    }
}
