using AlpimiAPI.Database;
using AlpimiAPI.Entities.ELessonPeriod;
using AlpimiAPI.Entities.EScheduleSettings;
using alpimi_planner_backend.Collisions.CollisionDataGatherers;
using alpimi_planner_backend.Collisions.CollisionDataManipulators;
using alpimi_planner_backend.Collisions.CollisionDataObjects;
using alpimi_planner_backend.Collisions.CollisionUtils;

namespace alpimi_planner_backend.Collisions.CollisionDetectionLogic
{
    public class GlobalCollisionDetectionManager
    {
        public static async Task<bool> scanForCollisions(
            Guid scheduleId,
            IDbService dbService,
            CancellationToken cancellationToken,
            LogMaker logMaker
        )
        {
            logMaker.writeLog("--Starting scan");

            ScheduleSettings? scheduleSettings = await DataHarvester.GetScheduleSettings(
                scheduleId,
                dbService,
                cancellationToken,
                logMaker
            );

            if (scheduleSettings != null)
            {
                List<CollisionScanInfo> scanList = await GetCollisionScanInfo.getScanInfo(
                    scheduleId,
                    dbService,
                    cancellationToken,
                    logMaker
                );

                if (scanList.Count() > 0)
                {
                    CollisionScanStaticData collisionScanStaticData =
                        await DataHarvester.GetCollisionScanStaticData(
                            scheduleId,
                            dbService,
                            cancellationToken,
                            logMaker
                        );

                    logMaker.writeLog("Lesson periods:");
                    foreach (LessonPeriod period in collisionScanStaticData.LessonPeriods)
                    {
                        var periodTime = (period.Start.Hour * 60) + period.Start.Minute;
                        logMaker.writeLog(periodTime.ToString());
                    }

                    CollisionScanInfo? fullScan = scanList.FirstOrDefault(info =>
                        info.scanType == "Full"
                    );

                    if (fullScan != null)
                    {
                        await RegionalCollisionDetectionManager.scanForCollisions(
                            fullScan,
                            scheduleSettings,
                            collisionScanStaticData,
                            scheduleId,
                            dbService,
                            cancellationToken,
                            logMaker
                        );

                        //Ustawić sprawdzenie kolizji dla wszystkich na true
                        await HistoryUpdater.SetCollisionCheckedTrue(
                            scheduleId,
                            dbService,
                            cancellationToken,
                            logMaker
                        );
                    }
                    else
                    {
                        foreach (CollisionScanInfo scan in scanList)
                        {
                            await RegionalCollisionDetectionManager.scanForCollisions(
                                scan,
                                scheduleSettings,
                                collisionScanStaticData,
                                scheduleId,
                                dbService,
                                cancellationToken,
                                logMaker
                            );
                            //Ustawić sprawdzenie kolizji dla jednego na true
                            await HistoryUpdater.SetCollisionCheckedTrue(
                                scan.historyEntryId,
                                dbService,
                                cancellationToken,
                                logMaker
                            );
                        }
                    }
                }
            }

            return true;
        }
    }
}
