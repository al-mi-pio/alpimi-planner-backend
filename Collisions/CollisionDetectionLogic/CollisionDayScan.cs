using System.Linq;
using AlpimiAPI.Entities.EScheduleSettings;
using alpimi_planner_backend.Collisions.CollisionDataObjects;
using alpimi_planner_backend.Collisions.CollisionUtils;
using static System.Runtime.InteropServices.JavaScript.JSType;

namespace alpimi_planner_backend.Collisions.CollisionDetectionLogic
{
    public class CollisionDayScan
    {
        public static CollisionScanResults scan(
            DateOnly scanDate,
            CollisionScanBlocks collisionScanBlocks,
            ScheduleSettings scheduleSettings,
            CollisionScanStaticData collisionScanStaticData,
            LogMaker logMaker
        )
        {
            CollisionScanResults results = new CollisionScanResults();

            logMaker.writeLog("***Starting day scan");
            //logMaker.writeLog(collisionScanBlocks.lessonBlocks.Count.ToString());
            for (int i = collisionScanBlocks.lessonBlocks.Count - 1; i >= 0; i--)
            {
                if (collisionScanBlocks.lessonBlocks[i].LessonDate == scanDate)
                {
                    CollisionScanResults blockResults = CollisionBlockScan.scan(
                        collisionScanBlocks.lessonBlocks[i].Id,
                        scanDate,
                        collisionScanBlocks,
                        scheduleSettings,
                        collisionScanStaticData,
                        logMaker
                    );

                    if (blockResults != null)
                    {
                        results.AddList.ObjectCollisions.AddRange(
                            blockResults.AddList.ObjectCollisions
                        );
                        foreach (CollisionBase collision in blockResults.AddList.TargetCollisions)
                        {
                            if (
                                !results.AddList.TargetCollisions.Any(x =>
                                    x.CollisionTypeId == collision.CollisionTypeId
                                    && x.CollidingObject1 == collision.CollidingObject1
                                    && x.CollidingObject2 == collision.CollidingObject2
                                )
                            )
                            {
                                results.AddList.TargetCollisions.AddRange(
                                    blockResults.AddList.TargetCollisions
                                );
                            }
                        }
                        results.AddList.ObjectTargetCollisions.AddRange(
                            blockResults.AddList.ObjectTargetCollisions
                        );

                        foreach (DateOnly date in blockResults.RemoveList.Days)
                        {
                            if (!results.RemoveList.Days.Contains(date))
                            {
                                results.RemoveList.Days.Add(date);
                            }
                        }
                        foreach (DateOnly date in blockResults.RemoveList.Weeks)
                        {
                            if (!results.RemoveList.Weeks.Contains(date))
                            {
                                results.RemoveList.Weeks.Add(date);
                            }
                        }
                        results.RemoveList.LessonBlockIds.AddRange(
                            blockResults.RemoveList.LessonBlockIds
                        );
                    }

                    collisionScanBlocks.lessonBlocks.RemoveAt(i);
                }
                //logMaker.writeLog(i.ToString());
            }

            return results;
        }
    }
}
