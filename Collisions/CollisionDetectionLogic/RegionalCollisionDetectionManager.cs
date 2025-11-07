using AlpimiAPI.Database;
using AlpimiAPI.Entities.EScheduleSettings;
using alpimi_planner_backend.Collisions.CollisionDataGatherers;
using alpimi_planner_backend.Collisions.CollisionDataObjects;
using alpimi_planner_backend.Collisions.CollisionUtils;

namespace alpimi_planner_backend.Collisions.CollisionDetectionLogic
{
    public class RegionalCollisionDetectionManager
    {
        public static async Task<bool> scanForCollisions(
            CollisionScanInfo collisionScanInfo,
            ScheduleSettings scheduleSettings,
            CollisionScanStaticData collisionScanStaticData,
            Guid scheduleId,
            IDbService dbService,
            CancellationToken cancellationToken,
            LogMaker logMaker
        )
        {
            logMaker.writeLog(
                "--Starting scan for: "
                    + (
                        collisionScanInfo.historyEntryId
                        + "\t"
                        + collisionScanInfo.scanType
                        + "\t"
                        + collisionScanInfo.scanBlockId
                        + "\t"
                        + collisionScanInfo.scanDate
                    ).ToString()
            );

            CollisionScanBlocks collisionScanBlocks = await DataHarvester.GetCollisionScanBlocks(
                collisionScanInfo,
                scheduleId,
                dbService,
                cancellationToken,
                logMaker
            );

            CollisionScanResults results = new CollisionScanResults();

            if (collisionScanInfo.scanType == "Full")
            {
                results = CollisionFullScan.scan(
                    collisionScanBlocks,
                    scheduleSettings,
                    collisionScanStaticData,
                    logMaker
                );
            }
            else if (collisionScanInfo.scanType == "Day")
            {
                if (collisionScanInfo.scanDate != null)
                {
                    DateOnly scanDate = collisionScanInfo.scanDate.Value;
                    results = CollisionDayScan.scan(
                        scanDate,
                        collisionScanBlocks,
                        scheduleSettings,
                        collisionScanStaticData,
                        logMaker
                    );
                }
            }
            else if (collisionScanInfo.scanType == "Block")
            {
                if (collisionScanInfo.scanDate != null && collisionScanInfo.scanBlockId != null)
                {
                    DateOnly scanDate = collisionScanInfo.scanDate.Value;
                    Guid blockId = collisionScanInfo.scanBlockId.Value;
                    results = CollisionBlockScan.scan(
                        blockId,
                        scanDate,
                        collisionScanBlocks,
                        scheduleSettings,
                        collisionScanStaticData,
                        logMaker
                    );
                }
            }

            //Add & remove from database

            return true;
        }
    }
}
