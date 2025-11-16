using AlpimiAPI.Database;
using alpimi_planner_backend.Collisions.CollisionDataObjects;
using alpimi_planner_backend.Collisions.CollisionUtils;
using Azure.Core;

namespace alpimi_planner_backend.Collisions.CollisionDataManipulators
{
    public class CollisionDestroyer
    {
        public record GuidPar(Guid Id);

        public record StringPar(string Par);

        public static async Task<bool> DeleteCollisions(
            CollisionRemoveList removeList,
            Guid scheduleId,
            IDbService dbService,
            CancellationToken cancellationToken,
            LogMaker logMaker
        )
        {
            if (removeList.LessonBlockIds.Count() > 0)
            {
                foreach (Guid id in removeList.LessonBlockIds)
                {
                    string stringId = id.ToString();
                    StringPar stringPar = new StringPar(stringId);

                    await dbService.Delete(
                        @"
                            DELETE c
                            FROM [Collision] c
                            WHERE CollidingObject1 = @Par OR CollidingObject2 = @Par;",
                        stringPar
                    );
                }
            }

            if (removeList.Days.Count() > 0)
            {
                foreach (DateOnly day in removeList.Days)
                {
                    string stringDay = day.ToString();
                    StringPar stringPar = new StringPar(stringDay);

                    await dbService.Delete(
                        @"
                            DELETE c
                            FROM [Collision] c
                            WHERE CollidingObject1 = @Par",
                        stringPar
                    );
                }
            }

            if (removeList.Weeks.Count() > 0)
            {
                foreach (DateOnly day in removeList.Weeks)
                {
                    string stringDay = day.ToString();
                    StringPar stringPar = new StringPar(stringDay);

                    await dbService.Delete(
                        @"
                            DELETE c
                            FROM [Collision] c
                            WHERE CollidingObject1 = @Par",
                        stringPar
                    );
                }
            }

            return true;
        }

        public static async Task<bool> DeleteAllCollisions(
            Guid scheduleId,
            IDbService dbService,
            CancellationToken cancellationToken,
            LogMaker logMaker
        )
        {
            GuidPar guidPar = new GuidPar(scheduleId);

            await dbService.Delete(
                @"
                            DELETE c
                            FROM [Collision] c
                            INNER JOIN [CollisionType] ct ON ct.[Id] = c.[CollisionTypeId]
                            WHERE ct.[ScheduleId] = @Id;",
                guidPar
            );
            return true;
        }
    }
}
