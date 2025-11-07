using System.Text.Json;
using AlpimiAPI.Database;
using AlpimiAPI.Entities.ECollision;
using AlpimiAPI.Entities.EHistory;
using AlpimiAPI.Entities.ELessonBlock;
using alpimi_planner_backend.Collisions.CollisionDataObjects;
using alpimi_planner_backend.Collisions.CollisionUtils;
using Azure.Core;
using Castle.Components.DictionaryAdapter.Xml;
using MediatR;
using Sprache;

namespace alpimi_planner_backend.Collisions.CollisionDataGatherers
{
    public class GetCollisionScanInfo
    {
        public record GuidPar(Guid Id);

        public record BlockScanInfo(Guid Id, DateOnly LessonDate);

        public record DayOffDates(DateOnly From, DateOnly To);

        public static async Task<List<CollisionScanInfo>> getScanData(
            Guid scheduleId,
            IDbService dbService,
            CancellationToken cancellationToken,
            LogMaker logMaker
        )
        {
            IDbService _dbService = dbService;
            IEnumerable<History>? history;
            var guidPar = new GuidPar(scheduleId);

            history = await _dbService.GetAll<History>(
                $@"
                    SELECT
                    h.[Id], h.[Timestamp], h.[AffectedEntityId], h.[AffectedEntity], h.[Command], h.[ReversaleDTO], h.[IsUndone], h.[CollisionChecked], h.[ScheduleId]
                    FROM [History] h
                    WHERE h.[ScheduleId] = @Id
                    AND h.[CollisionChecked] = 0
                    ORDER BY h.[Timestamp];",
                guidPar
            );

            List<CollisionScanInfo> collisionScanInfo = new List<CollisionScanInfo>();

            if (history != null)
            {
                logMaker.writeLog("--History");

                List<CollisionScanInfo> scanList = new List<CollisionScanInfo>();
                foreach (var entry in history)
                {
                    if (logMaker.isLogEnabled())
                    {
                        logMaker.writeLog(
                            (
                                entry.CollisionChecked
                                + "\t"
                                + entry.Command
                                + "\t"
                                + entry.AffectedEntity
                                + "\t"
                                + entry.AffectedEntityId
                                + "\t"
                                + entry.Id
                            ).ToString()
                        );
                    }

                    GuidPar blockId = new GuidPar(entry.AffectedEntityId);
                    BlockScanInfo? blockScanInfo;
                    DayOffDates? dayOffDates;
                    IEnumerable<BlockScanInfo>? clusterScanInfo;

                    switch (entry.AffectedEntity)
                    {
                        case "LessonBlock":
                            switch (entry.Command)
                            {
                                case "Create":
                                    blockScanInfo = await _dbService.Get<BlockScanInfo>(
                                        @"
                                            SELECT 
                                            [Id], [LessonDate]
                                            FROM [LessonBlock] 
                                            WHERE [Id] = @Id;",
                                        blockId
                                    );
                                    if (blockScanInfo != null)
                                    {
                                        collisionScanInfo.Add(
                                            new CollisionScanInfo(
                                                entry.Id,
                                                "Block",
                                                blockScanInfo.Id,
                                                blockScanInfo.LessonDate
                                            )
                                        );
                                    }
                                    break;
                                case "Delete":
                                    if (entry.ReversaleDTO != null)
                                    {
                                        collisionScanInfo.Add(
                                            new CollisionScanInfo(
                                                entry.Id,
                                                "Block",
                                                entry.AffectedEntityId,
                                                DateOnly.FromDateTime(
                                                    JsonDocument
                                                        .Parse(entry.ReversaleDTO)
                                                        .RootElement.GetProperty("LessonDate")
                                                        .GetDateTime()
                                                )
                                            )
                                        );
                                    }
                                    break;
                                case "Patch":
                                    blockScanInfo = await _dbService.Get<BlockScanInfo>(
                                        @"
                                            SELECT 
                                            [Id], [LessonDate]
                                            FROM [LessonBlock] 
                                            WHERE [Id] = @Id;",
                                        blockId
                                    );
                                    if (blockScanInfo != null)
                                    {
                                        collisionScanInfo.Add(
                                            new CollisionScanInfo(
                                                entry.Id,
                                                "Block",
                                                blockScanInfo.Id,
                                                blockScanInfo.LessonDate
                                            )
                                        );
                                    }
                                    break;
                                default:
                                    break;
                            }
                            break;
                        case "LessonBlockCluster":
                            switch (entry.Command)
                            {
                                case "Create":
                                    clusterScanInfo = await _dbService.GetAll<BlockScanInfo>(
                                        @"
                                            SELECT 
                                            [Id], [LessonDate]
                                            FROM [LessonBlock] 
                                            WHERE [ClusterId] = @Id;",
                                        blockId
                                    );
                                    if (clusterScanInfo != null)
                                    {
                                        foreach (var block in clusterScanInfo)
                                        {
                                            collisionScanInfo.Add(
                                                new CollisionScanInfo(
                                                    entry.Id,
                                                    "Block",
                                                    block.Id,
                                                    block.LessonDate
                                                )
                                            );
                                        }
                                    }
                                    break;
                                case "Delete":
                                    collisionScanInfo.Add(
                                        new CollisionScanInfo(entry.Id, "Full", null, null)
                                    );
                                    break;
                                case "Patch":
                                    clusterScanInfo = await _dbService.GetAll<BlockScanInfo>(
                                        @"
                                            SELECT 
                                            [Id], [LessonDate]
                                            FROM [LessonBlock] 
                                            WHERE [ClusterId] = @Id;",
                                        blockId
                                    );
                                    if (clusterScanInfo != null)
                                    {
                                        foreach (var block in clusterScanInfo)
                                        {
                                            collisionScanInfo.Add(
                                                new CollisionScanInfo(
                                                    entry.Id,
                                                    "Block",
                                                    block.Id,
                                                    block.LessonDate
                                                )
                                            );
                                        }
                                    }
                                    break;
                                default:
                                    break;
                            }
                            break;
                        case "Lesson":
                            switch (entry.Command)
                            {
                                case "Create":

                                    break;
                                case "Delete":
                                    collisionScanInfo.Add(
                                        new CollisionScanInfo(entry.Id, "Full", null, null)
                                    );
                                    break;
                                case "Patch":
                                    clusterScanInfo = await _dbService.GetAll<BlockScanInfo>(
                                        @"
                                            SELECT 
                                            [Id], [LessonDate]
                                            FROM [LessonBlock] 
                                            WHERE [LessonId] = @Id;",
                                        blockId
                                    );
                                    if (clusterScanInfo != null)
                                    {
                                        foreach (var block in clusterScanInfo)
                                        {
                                            collisionScanInfo.Add(
                                                new CollisionScanInfo(
                                                    entry.Id,
                                                    "Block",
                                                    block.Id,
                                                    block.LessonDate
                                                )
                                            );
                                        }
                                    }
                                    break;
                                default:
                                    break;
                            }
                            break;
                        case "Classroom":
                            switch (entry.Command)
                            {
                                case "Create":

                                    break;
                                case "Delete":
                                    collisionScanInfo.Add(
                                        new CollisionScanInfo(entry.Id, "Full", null, null)
                                    );
                                    break;
                                case "Patch":
                                    clusterScanInfo = await _dbService.GetAll<BlockScanInfo>(
                                        @"
                                            SELECT 
                                            [Id], [LessonDate]
                                            FROM [LessonBlock] 
                                            WHERE [ClassroomId] = @Id;",
                                        blockId
                                    );
                                    if (clusterScanInfo != null)
                                    {
                                        foreach (var block in clusterScanInfo)
                                        {
                                            collisionScanInfo.Add(
                                                new CollisionScanInfo(
                                                    entry.Id,
                                                    "Block",
                                                    block.Id,
                                                    block.LessonDate
                                                )
                                            );
                                        }
                                    }
                                    break;
                                default:
                                    break;
                            }
                            break;
                        case "ClassroomType":
                            switch (entry.Command)
                            {
                                case "Create":

                                    break;
                                case "Delete":
                                    collisionScanInfo.Add(
                                        new CollisionScanInfo(entry.Id, "Full", null, null)
                                    );
                                    break;
                                case "Patch":
                                    collisionScanInfo.Add(
                                        new CollisionScanInfo(entry.Id, "Full", null, null)
                                    );
                                    break;
                                default:
                                    break;
                            }
                            break;
                        case "LessonType":
                            switch (entry.Command)
                            {
                                case "Create":

                                    break;
                                case "Delete":
                                    collisionScanInfo.Add(
                                        new CollisionScanInfo(entry.Id, "Full", null, null)
                                    );
                                    break;
                                case "Patch":
                                    clusterScanInfo = await _dbService.GetAll<BlockScanInfo>(
                                        @"
                                            SELECT 
                                            lb.[Id], lb.[LessonDate]
                                            FROM [LessonBlock] lb
                                            INNER JOIN [Lesson] l ON lb.[LessonId] = l.[Id]
                                            WHERE l.[LessonTypeId] = @Id;",
                                        blockId
                                    );
                                    if (clusterScanInfo != null)
                                    {
                                        foreach (var block in clusterScanInfo)
                                        {
                                            collisionScanInfo.Add(
                                                new CollisionScanInfo(
                                                    entry.Id,
                                                    "Block",
                                                    block.Id,
                                                    block.LessonDate
                                                )
                                            );
                                        }
                                    }
                                    break;
                                default:
                                    break;
                            }
                            break;
                        case "Teacher":
                            switch (entry.Command)
                            {
                                case "Create":

                                    break;
                                case "Delete":
                                    collisionScanInfo.Add(
                                        new CollisionScanInfo(entry.Id, "Full", null, null)
                                    );
                                    break;
                                case "Patch":
                                    clusterScanInfo = await _dbService.GetAll<BlockScanInfo>(
                                        @"
                                            SELECT 
                                            lb.[Id], lb.[LessonDate]
                                            FROM [LessonBlock] lb
                                            INNER JOIN [Lesson] l ON lb.[LessonId] = l.[Id]
                                            WHERE l.[TeacherId] = @Id;",
                                        blockId
                                    );
                                    if (clusterScanInfo != null)
                                    {
                                        foreach (var block in clusterScanInfo)
                                        {
                                            collisionScanInfo.Add(
                                                new CollisionScanInfo(
                                                    entry.Id,
                                                    "Block",
                                                    block.Id,
                                                    block.LessonDate
                                                )
                                            );
                                        }
                                    }
                                    break;
                                default:
                                    break;
                            }
                            break;
                        case "Availability":
                            switch (entry.Command)
                            {
                                case "Create":
                                    clusterScanInfo = await _dbService.GetAll<BlockScanInfo>(
                                        @"
                                            SELECT 
                                            lb.[Id], lb.[LessonDate]
                                            FROM [LessonBlock] lb
                                            INNER JOIN [Lesson] l ON lb.[LessonId] = l.[Id]
                                            INNER JOIN [Teacher] t ON l.[TeacherId] = t.[Id]
                                            INNER JOIN [Availability] a ON t.[Id] = a.[TeacherId]
                                            WHERE a.[Id] = @Id;",
                                        blockId
                                    );
                                    if (clusterScanInfo != null)
                                    {
                                        foreach (var block in clusterScanInfo)
                                        {
                                            collisionScanInfo.Add(
                                                new CollisionScanInfo(
                                                    entry.Id,
                                                    "Block",
                                                    block.Id,
                                                    block.LessonDate
                                                )
                                            );
                                        }
                                    }
                                    break;
                                case "Delete":
                                    collisionScanInfo.Add(
                                        new CollisionScanInfo(entry.Id, "Full", null, null)
                                    );
                                    break;
                                case "Patch":
                                    clusterScanInfo = await _dbService.GetAll<BlockScanInfo>(
                                        @"
                                            SELECT 
                                            lb.[Id], lb.[LessonDate]
                                            FROM [LessonBlock] lb
                                            INNER JOIN [Lesson] l ON lb.[LessonId] = l.[Id]
                                            INNER JOIN [Teacher] t ON l.[TeacherId] = t.[Id]
                                            INNER JOIN [Availability] a ON t.[Id] = a.[TeacherId]
                                            WHERE a.[Id] = @Id;",
                                        blockId
                                    );
                                    if (clusterScanInfo != null)
                                    {
                                        foreach (var block in clusterScanInfo)
                                        {
                                            collisionScanInfo.Add(
                                                new CollisionScanInfo(
                                                    entry.Id,
                                                    "Block",
                                                    block.Id,
                                                    block.LessonDate
                                                )
                                            );
                                        }
                                    }
                                    break;
                                default:
                                    break;
                            }
                            break;
                        case "Group":
                            switch (entry.Command)
                            {
                                case "Create":

                                    break;
                                case "Delete":
                                    collisionScanInfo.Add(
                                        new CollisionScanInfo(entry.Id, "Full", null, null)
                                    );
                                    break;
                                case "Patch":
                                    collisionScanInfo.Add(
                                        new CollisionScanInfo(entry.Id, "Full", null, null)
                                    );
                                    break;
                                default:
                                    break;
                            }
                            break;
                        case "Subgroup":
                            switch (entry.Command)
                            {
                                case "Create":

                                    break;
                                case "Delete":
                                    collisionScanInfo.Add(
                                        new CollisionScanInfo(entry.Id, "Full", null, null)
                                    );
                                    break;
                                case "Patch":
                                    collisionScanInfo.Add(
                                        new CollisionScanInfo(entry.Id, "Full", null, null)
                                    );
                                    break;
                                default:
                                    break;
                            }
                            break;
                        case "Student":
                            switch (entry.Command)
                            {
                                case "Create":
                                    collisionScanInfo.Add(
                                        new CollisionScanInfo(entry.Id, "Full", null, null)
                                    );
                                    break;
                                case "Delete":
                                    collisionScanInfo.Add(
                                        new CollisionScanInfo(entry.Id, "Full", null, null)
                                    );
                                    break;
                                case "Patch":
                                    collisionScanInfo.Add(
                                        new CollisionScanInfo(entry.Id, "Full", null, null)
                                    );
                                    break;
                                default:
                                    break;
                            }
                            break;
                        case "DayOff":
                            switch (entry.Command)
                            {
                                case "Create":
                                    dayOffDates = await _dbService.Get<DayOffDates>(
                                        @"
                                            SELECT 
                                            [From], [To]
                                            FROM [DayOff] 
                                            WHERE [Id] = @Id;",
                                        blockId
                                    );
                                    if (dayOffDates != null)
                                    {
                                        DateOnly endDate = dayOffDates.To;
                                        for (
                                            DateOnly startDate = dayOffDates.From;
                                            startDate <= endDate;
                                            startDate = startDate.AddDays(1)
                                        )
                                        {
                                            collisionScanInfo.Add(
                                                new CollisionScanInfo(
                                                    entry.Id,
                                                    "Day",
                                                    null,
                                                    startDate
                                                )
                                            );
                                        }
                                    }
                                    break;
                                case "Delete":
                                    if (entry.ReversaleDTO != null)
                                    {
                                        DateOnly _startDate = DateOnly.FromDateTime(
                                            JsonDocument
                                                .Parse(entry.ReversaleDTO)
                                                .RootElement.GetProperty("From")
                                                .GetDateTime()
                                        );
                                        DateOnly endDate = DateOnly.FromDateTime(
                                            JsonDocument
                                                .Parse(entry.ReversaleDTO)
                                                .RootElement.GetProperty("LessonDate")
                                                .GetDateTime()
                                        );
                                        for (
                                            DateOnly startDate = _startDate;
                                            startDate <= endDate;
                                            startDate = startDate.AddDays(1)
                                        )
                                        {
                                            collisionScanInfo.Add(
                                                new CollisionScanInfo(
                                                    entry.Id,
                                                    "Day",
                                                    null,
                                                    startDate
                                                )
                                            );
                                        }
                                    }
                                    break;
                                case "Patch":
                                    dayOffDates = await _dbService.Get<DayOffDates>(
                                        @"
                                            SELECT 
                                            [From], [To]
                                            FROM [DayOff] 
                                            WHERE [Id] = @Id;",
                                        blockId
                                    );
                                    if (dayOffDates != null)
                                    {
                                        DateOnly endDate = dayOffDates.To;
                                        for (
                                            DateOnly startDate = dayOffDates.From;
                                            startDate <= endDate;
                                            startDate = startDate.AddDays(1)
                                        )
                                        {
                                            collisionScanInfo.Add(
                                                new CollisionScanInfo(
                                                    entry.Id,
                                                    "Day",
                                                    null,
                                                    startDate
                                                )
                                            );
                                        }
                                    }
                                    break;
                                default:
                                    break;
                            }
                            break;
                        case "LessonPeriod":
                            switch (entry.Command)
                            {
                                case "Create":
                                    collisionScanInfo.Add(
                                        new CollisionScanInfo(entry.Id, "Full", null, null)
                                    );
                                    break;
                                case "Delete":
                                    collisionScanInfo.Add(
                                        new CollisionScanInfo(entry.Id, "Full", null, null)
                                    );
                                    break;
                                case "Patch":
                                    collisionScanInfo.Add(
                                        new CollisionScanInfo(entry.Id, "Full", null, null)
                                    );
                                    break;
                                default:
                                    break;
                            }
                            break;
                        default:
                            break;
                    }
                }
            }

            logMaker.writeLog("--History translated to scan info");
            if (logMaker.isLogEnabled())
            {
                foreach (var position in collisionScanInfo)
                {
                    logMaker.writeLog(
                        (
                            position.historyEntryId
                            + "\t"
                            + position.scanType
                            + "\t"
                            + position.scanBlockId
                            + "\t"
                            + position.scanDate
                        ).ToString()
                    );
                }
            }

            return collisionScanInfo;
        }
    }
}
