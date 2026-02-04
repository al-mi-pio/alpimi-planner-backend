using System.Text.Json;
using AlpimiAPI.Entities.ECollision;
using AlpimiAPI.Entities.ECollisionType.DTO;
using AlpimiAPI.Entities.EDayOff;
using AlpimiAPI.Entities.ELessonBlock;
using AlpimiAPI.Entities.EScheduleSettings;
using alpimi_planner_backend.Collisions.CollisionDataObjects;
using alpimi_planner_backend.Collisions.CollisionUtils;

namespace alpimi_planner_backend.Collisions.CollisionDetectionLogic
{
    public class LocalCollisionDetectionManager
    {
        public static CollisionAddList scanForCollisions(
            Guid scanBlockId,
            DateOnly scanDate,
            CollisionScanBlocks collisionScanBlocks,
            ScheduleSettings scheduleSettings,
            CollisionScanStaticData collisionScanStaticData,
            LogMaker logMaker
        )
        {
            logMaker.writeLog("**Local");
            CollisionAddList results = new CollisionAddList();

            foreach (CollisionTypeData collisionType in collisionScanStaticData.CollisionTypes)
            {
                CollisionLessonBlock? lessonBlock = collisionScanBlocks.lessonBlocks.FirstOrDefault(
                    block => block.Id == scanBlockId
                );
                CollisionScanBlocks tempScanBlocks = new CollisionScanBlocks();
                tempScanBlocks.daysOffFull = new List<DayOff>(collisionScanBlocks.daysOffFull);
                tempScanBlocks.lessonBlocks = new List<CollisionLessonBlock>(
                    collisionScanBlocks.lessonBlocks
                );
                if (lessonBlock != null)
                {
                    tempScanBlocks.lessonBlocks.RemoveAll(block => block.Id == scanBlockId);
                }
                List<CollisionBase> collisions = FilterInterpreter.Interpret(
                    collisionType,
                    lessonBlock,
                    scanDate,
                    collisionScanBlocks,
                    scheduleSettings,
                    collisionScanStaticData,
                    logMaker
                );

                if (collisionType.category == "lessonblock")
                {
                    results.ObjectCollisions.AddRange(collisions);
                }
                else if (collisionType.category == "day" || collisionType.category == "week")
                {
                    results.TargetCollisions.AddRange(collisions);
                }
                else
                {
                    results.ObjectTargetCollisions.AddRange(collisions);
                }
            }

            return results;
        }
    }
}
