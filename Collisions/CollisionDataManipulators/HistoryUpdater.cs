using AlpimiAPI.Database;
using AlpimiAPI.Entities.ECollisionType;
using alpimi_planner_backend.Collisions.CollisionUtils;
using Azure.Core;

namespace alpimi_planner_backend.Collisions.CollisionDataManipulators
{
    public class HistoryUpdater
    {
        public record GuidPar(Guid Id);

        public static async Task<bool> SetCollisionCheckedTrue(
            Guid id,
            IDbService dbService,
            CancellationToken cancellationToken,
            LogMaker logMaker
        )
        {
            GuidPar guidPar = new GuidPar(id);

            await dbService.Update<bool>(
                $@"
                    UPDATE [History] 
                    SET
                    [CollisionChecked] = 1
                    WHERE [Id] = @Id OR [ScheduleId] = @Id;

                    SELECT CASE WHEN @@ROWCOUNT > 0 THEN 1 ELSE 0 END;",
                guidPar
            );
            return true;
        }
    }
}
