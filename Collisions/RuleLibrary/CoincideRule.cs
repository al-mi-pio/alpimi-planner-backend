using AlpimiAPI.Entities.ELessonPeriod;
using AlpimiAPI.Entities.EScheduleSettings;
using alpimi_planner_backend.Collisions.CollisionDataObjects;
using alpimi_planner_backend.Collisions.CollisionUtils;

namespace alpimi_planner_backend.Collisions.RuleLibrary
{
    public class CoincideRule : Rule
    {
        public CoincideRule(string data)
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
            List<LessonPeriod> periods =
                collisionScanStaticData.LessonPeriods.ToList<LessonPeriod>();

            var objectStart =
                (periods[objectBlock.LessonStart].Start.Hour * 60)
                + (periods[objectBlock.LessonStart].Start.Minute);
            var objectEnd =
                (periods[objectBlock.LessonEnd].Start.Hour * 60)
                + (periods[objectBlock.LessonEnd].Start.Minute)
                + scheduleSettings.SchoolHour;
            var targetStart =
                (periods[targetBlock.LessonStart].Start.Hour * 60)
                + (periods[targetBlock.LessonStart].Start.Minute);
            var targetEnd =
                (periods[targetBlock.LessonEnd].Start.Hour * 60)
                + (periods[targetBlock.LessonEnd].Start.Minute)
                + scheduleSettings.SchoolHour;

            if (objectStart < targetEnd && objectEnd > targetStart)
            {
                logMaker.writeLog("Blocks Coincide");
                return true;
            }
            else
            {
                return false;
            }
        }
    }
}
