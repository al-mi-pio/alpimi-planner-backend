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
                    OUTPUT
                    INSERTED.[CollisionChecked]
                    WHERE [Id] = @Id OR [ScheduleId] = @Id;",
                guidPar
            );
            return true;
        }
    }
}
