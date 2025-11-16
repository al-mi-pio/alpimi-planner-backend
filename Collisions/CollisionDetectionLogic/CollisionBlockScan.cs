using AlpimiAPI.Entities.EScheduleSettings;
using alpimi_planner_backend.Collisions.CollisionDataObjects;
using alpimi_planner_backend.Collisions.CollisionUtils;

namespace alpimi_planner_backend.Collisions.CollisionDetectionLogic
{
    public class CollisionBlockScan
    {
        public static CollisionScanResults scan(
            Guid scanBlockId,
            DateOnly scanDate,
            CollisionScanBlocks collisionScanBlocks,
            ScheduleSettings scheduleSettings,
            CollisionScanStaticData collisionScanStaticData,
            LogMaker logMaker
        )
        {
            CollisionScanResults results = new CollisionScanResults();

            logMaker.writeLog("***Starting block scan");

            CollisionAddList collisionAddList = LocalCollisionDetectionManager.scanForCollisions(
                scanBlockId,
                scanDate,
                collisionScanBlocks,
                scheduleSettings,
                collisionScanStaticData,
                logMaker
            );

            if (collisionAddList != null)
            {
                results.AddList.ObjectCollisions.AddRange(collisionAddList.ObjectCollisions);
                results.AddList.TargetCollisions.AddRange(collisionAddList.TargetCollisions);
                results.AddList.ObjectTargetCollisions.AddRange(
                    collisionAddList.ObjectTargetCollisions
                );

                results.RemoveList.Days.Add(scanDate);
                results.RemoveList.LessonBlockIds.Add(scanBlockId);
                results.RemoveList.Weeks.Add(DateOnlyUtils.getWeekStart(scanDate));
            }

            return results;
        }
    }
}
