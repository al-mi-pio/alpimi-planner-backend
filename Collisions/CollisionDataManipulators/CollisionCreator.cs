using System.Collections.Generic;
using AlpimiAPI.Database;
using AlpimiAPI.Entities.ECollision;
using alpimi_planner_backend.Collisions.CollisionDataObjects;
using alpimi_planner_backend.Collisions.CollisionUtils;
using Azure.Core;

namespace alpimi_planner_backend.Collisions.CollisionDataManipulators
{
    public class CollisionCreator
    {
        public record GuidPar(Guid Id);

        public static async Task<bool> SaveCollisions(
            CollisionScanInfo scanInfo,
            CollisionScanResults scanResults,
            Guid scheduleId,
            IDbService dbService,
            CancellationToken cancellationToken,
            LogMaker logMaker
        )
        {
            CollisionAddList addList = scanResults.AddList;

            GuidPar guidPar = new GuidPar(scheduleId);

            IEnumerable<CollisionBase>? ignoredCollisions = await dbService.GetAll<CollisionBase>(
                $@"
                            SELECT
                            c.[Id], [CollidingObject1], [CollidingObject2], [Ignored], c.[CollisionTypeId]
                            FROM [Collision] c
                            INNER JOIN [CollisionType] ct ON ct.[Id] = c.[CollisionTypeId]
                            WHERE ct.[ScheduleId] = @Id AND [Ignored] = 1
                            ORDER BY c.[Id];",
                guidPar
            );

            if (scanInfo.scanType == "Full")
            {
                await CollisionDestroyer.DeleteAllCollisions(
                    scheduleId,
                    dbService,
                    cancellationToken,
                    logMaker
                );
            }
            else
            {
                await CollisionDestroyer.DeleteCollisions(
                    scanResults.RemoveList,
                    scheduleId,
                    dbService,
                    cancellationToken,
                    logMaker
                );
            }
            List<CollisionBase> fullAddList = new List<CollisionBase>();

            fullAddList.AddRange(addList.ObjectCollisions);
            fullAddList.AddRange(addList.TargetCollisions);
            fullAddList.AddRange(addList.ObjectTargetCollisions);

            foreach (CollisionBase collision in fullAddList)
            {
                if (ignoredCollisions != null)
                {
                    if (
                        ignoredCollisions.Any(x =>
                            x.CollisionTypeId == collision.CollisionTypeId
                            && x.CollidingObject1 == collision.CollidingObject1
                            && x.CollidingObject2 == collision.CollidingObject2
                        )
                    )
                    {
                        collision.Ignored = true;
                    }
                }

                Guid addedCollisionId = await dbService.Post<Guid>(
                    $@"
                    INSERT INTO [Collision] 
                    ([Id], [CollidingObject1], [CollidingObject2], [Ignored], [CollisionTypeId])
                    OUTPUT 
                    INSERTED.Id                    
                    VALUES (
                    '{collision.Id}',   
                    @CollidingObject1,
                    @CollidingObject2,
                    @Ignored,
                    @CollisionTypeId);",
                    collision
                );

                logMaker.writeLog("Collision added to database " + addedCollisionId);
            }

            return true;
        }
    }
}
