using AlpimiAPI.Entities.ECollision;
using AlpimiAPI.Entities.EDayOff;
using AlpimiAPI.Entities.EScheduleSettings;
using alpimi_planner_backend.Collisions.CollisionDataObjects;
using alpimi_planner_backend.Collisions.CollisionDetectionLogic;
using alpimi_planner_backend.Collisions.CollisionUtils;
using static alpimi_planner_backend.Collisions.CollisionDataObjects.CollisionScanBlocks;

namespace alpimi_planner_backend.Collisions.CollisionDetectionLogic
{
    public class CollisionFullScan
    {
        public static CollisionScanResults scan(
            CollisionScanBlocks collisionScanBlocks,
            ScheduleSettings scheduleSettings,
            CollisionScanStaticData collisionScanStaticData,
            LogMaker logMaker
        )
        {
            CollisionScanResults results = new CollisionScanResults();

            DateOnly scheduleStart = scheduleSettings.SchoolYearStart;
            DateOnly scheduleEnd = scheduleSettings.SchoolYearEnd;

            logMaker.writeLog("***Starting full scan");

            for (DateOnly date = scheduleStart; date <= scheduleEnd; date = date.AddDays(1))
            {
                CollisionScanBlocks dayScanBlocks = new CollisionScanBlocks();

                DateOnly weekStart = DateOnlyUtils.getWeekStart(date);
                DateOnly weekEnd = DateOnlyUtils.getWeekEnd(date);

                if (collisionScanBlocks.lessonBlocks != null)
                {
                    for (int i = collisionScanBlocks.lessonBlocks.Count - 1; i >= 0; i--)
                    {
                        if (
                            collisionScanBlocks.lessonBlocks[i].LessonDate >= weekStart
                            && collisionScanBlocks.lessonBlocks[i].LessonDate <= weekEnd
                        )
                        {
                            dayScanBlocks.lessonBlocks.Add(collisionScanBlocks.lessonBlocks[i]);
                        }
                        if (collisionScanBlocks.lessonBlocks[i].LessonDate == date)
                        {
                            collisionScanBlocks.lessonBlocks.RemoveAt(i);
                        }
                    }
                }

                if (collisionScanBlocks.daysOffFull != null)
                {
                    for (int i = collisionScanBlocks.daysOffFull.Count - 1; i >= 0; i--)
                    {
                        if (
                            collisionScanBlocks.daysOffFull[i].To >= weekStart
                            && collisionScanBlocks.daysOffFull[i].From <= weekEnd
                        )
                        {
                            dayScanBlocks.daysOffFull.Add(collisionScanBlocks.daysOffFull[i]);
                        }
                    }
                }

                CollisionScanResults dayResults = CollisionDayScan.scan(
                    date,
                    dayScanBlocks,
                    scheduleSettings,
                    collisionScanStaticData,
                    logMaker
                );

                if (dayResults != null)
                {
                    results.AddList.ObjectCollisions.AddRange(dayResults.AddList.ObjectCollisions);
                    results.AddList.TargetCollisions.AddRange(dayResults.AddList.TargetCollisions);
                    results.AddList.ObjectTargetCollisions.AddRange(
                        dayResults.AddList.ObjectTargetCollisions
                    );

                    results.RemoveList.Days.AddRange(dayResults.RemoveList.Days);
                    results.RemoveList.LessonBlockIds.AddRange(
                        dayResults.RemoveList.LessonBlockIds
                    );
                }
            }

            return results;
        }
    }
}
