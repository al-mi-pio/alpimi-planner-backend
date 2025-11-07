using AlpimiAPI.Database;
using AlpimiAPI.Entities.EAvailability;
using AlpimiAPI.Entities.EClassroom;
using AlpimiAPI.Entities.EClassroom.Queries;
using AlpimiAPI.Entities.EClassroomType;
using AlpimiAPI.Entities.ECollisionType;
using AlpimiAPI.Entities.EDayOff;
using AlpimiAPI.Entities.EGroup;
using AlpimiAPI.Entities.ELesson;
using AlpimiAPI.Entities.ELesson.Queries;
using AlpimiAPI.Entities.ELessonBlock;
using AlpimiAPI.Entities.ELessonPeriod;
using AlpimiAPI.Entities.EScheduleSettings;
using AlpimiAPI.Entities.ESubgroup;
using AlpimiAPI.Entities.ESubgroup.Queries;
using AlpimiAPI.Entities.ETeacher;
using AlpimiAPI.Utilities;
using alpimi_planner_backend.Collisions.CollisionDataObjects;
using alpimi_planner_backend.Collisions.CollisionUtils;
using Azure.Core;
using Microsoft.AspNetCore.Mvc;
using static alpimi_planner_backend.Collisions.CollisionDataGatherers.GetCollisionScanInfo;
using static alpimi_planner_backend.Collisions.CollisionDataObjects.CollisionScanBlocks;

namespace alpimi_planner_backend.Collisions.CollisionDataGatherers
{
    public class DataHarvester
    {
        public record GuidPar(Guid Id);

        public record TimeIntervalPar(Guid Id, DateOnly FromDate, DateOnly ToDate);

        public static async Task<ScheduleSettings?> getScheduleSettings(
            Guid scheduleId,
            IDbService dbService,
            CancellationToken cancellationToken,
            LogMaker logMaker
        )
        {
            logMaker.writeLog("--Getting schedule settings");
            var guidPar = new GuidPar(scheduleId);
            ScheduleSettings? scheduleSettings = await dbService.Get<ScheduleSettings?>(
                @"
                            SELECT
                            [Id], [SchoolHour], [SchoolYearStart], [SchoolYearEnd], [ScheduleId], [SchoolDays], [IsPublic]
                            FROM [ScheduleSettings] 
                            WHERE [Id] = @Id OR [ScheduleId] = @Id;",
                guidPar
            );
            if (scheduleSettings != null)
            {
                if (logMaker.isLogEnabled())
                {
                    logMaker.writeLog(
                        scheduleSettings.Id
                            + "\t"
                            + scheduleSettings.SchoolHour
                            + "\t"
                            + scheduleSettings.SchoolYearStart
                            + "\t"
                            + scheduleSettings.SchoolYearEnd
                            + "\t"
                            + scheduleSettings.ScheduleId
                            + "\t"
                            + scheduleSettings.SchoolDays
                            + "\t"
                            + scheduleSettings.IsPublic
                    );
                }
            }
            return scheduleSettings;
        }

        public static async Task<CollisionScanStaticData> GetCollisionScanStaticData(
            Guid scheduleId,
            IDbService dbService,
            CancellationToken cancellationToken,
            LogMaker logMaker
        )
        {
            logMaker.writeLog("--Getting collision static data");

            CollisionScanStaticData collisionScanStaticData = new CollisionScanStaticData();

            var guidPar = new GuidPar(scheduleId);

            logMaker.writeLog("--Getting lesson periods");
            collisionScanStaticData.LessonPeriods = await dbService.GetAll<LessonPeriod>(
                $@"
                            SELECT
                            lp.[Id], [Start], [ScheduleSettingsId] 
                            FROM [LessonPeriod] lp
                            INNER JOIN [ScheduleSettings] ss ON ss.[Id] = lp.[ScheduleSettingsId]
                            WHERE ss.[ScheduleId] = @Id 
                            ORDER BY [Start];",
                guidPar
            );
            if (collisionScanStaticData.LessonPeriods != null)
            {
                if (logMaker.isLogEnabled())
                {
                    foreach (var position in collisionScanStaticData.LessonPeriods)
                    {
                        logMaker.writeLog(
                            position.Id + "\t" + position.Start + "\t" + position.ScheduleSettingsId
                        );
                    }
                }
            }

            logMaker.writeLog("--Getting availabilities");
            collisionScanStaticData.Availabilities = await dbService.GetAll<Availability>(
                $@"
                            SELECT
                            a.[Id], a.[WeekDay], a.[Start], a.[End], a.[TeacherId] 
                            FROM [Availability] a
                            INNER JOIN [Teacher] t ON t.[Id] = a.[TeacherId]
                            WHERE t.[ScheduleId] = @Id
                            ORDER BY a.[TeacherId];",
                guidPar
            );
            if (collisionScanStaticData.Availabilities != null)
            {
                if (logMaker.isLogEnabled())
                {
                    foreach (var position in collisionScanStaticData.Availabilities)
                    {
                        logMaker.writeLog(
                            position.Id
                                + "\t"
                                + position.WeekDay
                                + "\t"
                                + position.Start
                                + "\t"
                                + position.End
                                + "\t"
                                + position.TeacherId
                        );
                    }
                }
            }

            logMaker.writeLog("--Getting groups");
            collisionScanStaticData.Groups = await dbService.GetAll<Group>(
                $@"
                            SELECT
                            [Id], [Name], [StudentCount], [ScheduleId] 
                            FROM [Group]
                            WHERE [ScheduleId] = @Id 
                            ORDER BY [Id];",
                guidPar
            );
            if (collisionScanStaticData.Groups != null)
            {
                if (logMaker.isLogEnabled())
                {
                    foreach (var position in collisionScanStaticData.Groups)
                    {
                        logMaker.writeLog(
                            position.Id
                                + "\t"
                                + position.Name
                                + "\t"
                                + position.StudentCount
                                + "\t"
                                + position.ScheduleId
                        );
                    }
                }
            }

            logMaker.writeLog("--Getting subgroups");
            collisionScanStaticData.Subgroups = await dbService.GetAll<Subgroup>(
                $@"
                            SELECT
                            sg.[Id], sg.[Name], sg.[StudentCount], sg.[GroupId] 
                            FROM [Subgroup] sg
                            INNER JOIN [Group] g ON sg.[GroupId]  = g.[Id]
                            WHERE g.[ScheduleId] = @Id
                            ORDER BY [Id];",
                guidPar
            );
            if (collisionScanStaticData.Subgroups != null)
            {
                if (logMaker.isLogEnabled())
                {
                    foreach (var position in collisionScanStaticData.Subgroups)
                    {
                        logMaker.writeLog(
                            position.Id
                                + "\t"
                                + position.Name
                                + "\t"
                                + position.StudentCount
                                + "\t"
                                + position.GroupId
                        );
                    }
                }
            }

            logMaker.writeLog("--Getting collision types");
            collisionScanStaticData.CollisionTypes = await dbService.GetAll<CollisionType>(
                $@"
                            SELECT
                            [Id], [Name], [Description], [Weight], [Filter], [Category], [ScheduleId]
                            FROM [CollisionType]
                            WHERE [ScheduleId] = @Id 
                            ORDER BY [Id];",
                guidPar
            );
            if (collisionScanStaticData.CollisionTypes != null)
            {
                if (logMaker.isLogEnabled())
                {
                    foreach (var position in collisionScanStaticData.CollisionTypes)
                    {
                        logMaker.writeLog(
                            position.Id
                                + "\t"
                                + position.Name
                                + "\t"
                                + position.Description
                                + "\t"
                                + position.Weight
                                + "\t"
                                + position.Filter
                                + "\t"
                                + position.Category
                                + "\t"
                                + position.ScheduleId
                        );
                    }
                }
            }

            return collisionScanStaticData;
        }

        public static async Task<CollisionScanBlocks> GetCollisionScanBlocks(
            CollisionScanInfo collisionScanInfo,
            Guid scheduleId,
            IDbService dbService,
            CancellationToken cancellationToken,
            LogMaker logMaker
        )
        {
            logMaker.writeLog("--Getting collision scan blocks");
            CollisionScanBlocks collisionScanBlocks = new CollisionScanBlocks();
            if (collisionScanInfo.scanDate.HasValue)
            {
                logMaker.writeLog(
                    DateOnlyUtils.getWeekStart(collisionScanInfo.scanDate.Value).ToString()
                );
                logMaker.writeLog(
                    DateOnlyUtils.getWeekEnd(collisionScanInfo.scanDate.Value).ToString()
                );
            }

            if (collisionScanInfo.scanType == "Full")
            {
                GuidPar guidPar = new GuidPar(scheduleId);
                IEnumerable<CollisionLessonBlock>? collisionLessonBlocks =
                    new List<CollisionLessonBlock>();
                collisionLessonBlocks = await dbService.GetAll<CollisionLessonBlock>(
                    $@"
                            SELECT
                            lb.[Id], [LessonDate], [LessonStart], [LessonEnd], lb.[LessonId], [ClassroomId], [ClusterId]
                            FROM [LessonBlock] lb
                            INNER JOIN [Lesson] l ON l.[Id] = lb.[LessonId]
                            INNER JOIN [LessonType] lt ON lt.[Id] = l.[LessonTypeId]
                            WHERE lt.[ScheduleId] = @Id
                            ORDER BY lb.[Id];",
                    guidPar
                );

                if (collisionLessonBlocks != null)
                {
                    collisionScanBlocks.lessonBlocks = collisionLessonBlocks.ToList();

                    Dictionary<Guid, Lesson> lessonMap = new Dictionary<Guid, Lesson>();
                    Dictionary<Guid, Classroom> classroomMap = new Dictionary<Guid, Classroom>();
                    Dictionary<Guid, IEnumerable<Subgroup>?> subgroupsMap =
                        new Dictionary<Guid, IEnumerable<Subgroup>?>();
                    Dictionary<Guid, IEnumerable<ClassroomType>?> classroomTypeLessonMap =
                        new Dictionary<Guid, IEnumerable<ClassroomType>?>();
                    Dictionary<Guid, IEnumerable<ClassroomType>?> classroomTypeClassroomMap =
                        new Dictionary<Guid, IEnumerable<ClassroomType>?>();

                    foreach (var block in collisionScanBlocks.lessonBlocks)
                    {
                        if (!lessonMap.ContainsKey(block.LessonId))
                        {
                            GetLessonHandler getLessonHandler = new GetLessonHandler(dbService);
                            GetLessonQuery getLessonQuery = new GetLessonQuery(
                                block.LessonId,
                                new Guid(),
                                "Admin"
                            );
                            ActionResult<Lesson?> lesson = await getLessonHandler.Handle(
                                getLessonQuery,
                                cancellationToken
                            );

                            lessonMap.Add(block.LessonId, lesson.Value!);
                        }
                        block.Lesson = lessonMap[block.LessonId];

                        if (block.ClassroomId != null)
                        {
                            if (!classroomMap.ContainsKey(block.ClassroomId.Value))
                            {
                                GetClassroomHandler getClassroomHandler = new GetClassroomHandler(
                                    dbService
                                );
                                GetClassroomQuery getClassroomQuery = new GetClassroomQuery(
                                    block.ClassroomId.Value,
                                    new Guid(),
                                    "Admin"
                                );
                                ActionResult<Classroom?> classroom =
                                    await getClassroomHandler.Handle(
                                        getClassroomQuery,
                                        cancellationToken
                                    );

                                classroomMap.Add(block.ClassroomId.Value, classroom.Value!);
                            }
                            block.Classroom = classroomMap[block.ClassroomId.Value];
                        }

                        if (!subgroupsMap.ContainsKey(block.LessonId))
                        {
                            GuidPar lessonId = new GuidPar(block.LessonId);
                            IEnumerable<Subgroup>? subgroups = await dbService.GetAll<Subgroup>(
                                $@"
                                    SELECT
                                    sg.[Id], sg.[Name], sg.[StudentCount], sg.[GroupId] 
                                    FROM [Subgroup] sg
                                    LEFT JOIN [LessonSubgroup] lsg ON lsg.[SubgroupId] = sg.[Id]
                                    LEFT JOIN [Lesson] l ON l.[Id] = lsg.[LessonId]
                                    WHERE l.[Id] = @Id
                                    ORDER BY [Id];",
                                lessonId
                            );
                            subgroupsMap.Add(block.LessonId, subgroups);
                        }
                        block.Subgroups = subgroupsMap[block.LessonId];

                        if (!classroomTypeLessonMap.ContainsKey(block.LessonId))
                        {
                            GuidPar lessonId = new GuidPar(block.LessonId);
                            IEnumerable<ClassroomType>? classroomTypes =
                                await dbService.GetAll<ClassroomType>(
                                    $@"
                                    SELECT
                                    ct.[Id], ct.[Name], ct.[ScheduleId] 
                                    FROM [ClassroomType] ct
                                    LEFT JOIN [LessonClassroomType] lct ON lct.[ClassroomTypeId] = ct.[Id]
                                    LEFT JOIN [Lesson] l ON l.[Id] = lct.[LessonId]
                                    WHERE l.[Id] = @Id
                                    ORDER BY [Id];",
                                    lessonId
                                );
                            classroomTypeLessonMap.Add(block.LessonId, classroomTypes);
                        }
                        block.ClassroomTypeslesson = classroomTypeLessonMap[block.LessonId];

                        if (block.ClassroomId != null)
                        {
                            Guid notNullClassroomId = block.ClassroomId.Value;
                            if (!classroomTypeClassroomMap.ContainsKey(notNullClassroomId))
                            {
                                GuidPar classroomId = new GuidPar(notNullClassroomId);
                                IEnumerable<ClassroomType>? classroomTypes =
                                    await dbService.GetAll<ClassroomType>(
                                        $@"
                                    SELECT
                                    ct.[Id], ct.[Name], ct.[ScheduleId] 
                                    FROM [ClassroomType] ct
                                    LEFT JOIN [ClassroomClassroomType] cct ON cct.[ClassroomTypeId] = ct.[Id]
                                    LEFT JOIN [Classroom] c ON c.[Id] = cct.[ClassroomId]
                                    WHERE c.[Id] = @Id
                                    ORDER BY [Id];",
                                        classroomId
                                    );
                                classroomTypeClassroomMap.Add(notNullClassroomId, classroomTypes);
                            }
                            block.ClassroomTypesClassroom = classroomTypeClassroomMap[
                                notNullClassroomId
                            ];
                        }
                    }
                }

                IEnumerable<DayOff>? daysOff = new List<DayOff>();
                daysOff = await dbService.GetAll<DayOff>(
                    $@"
                            SELECT
                            do.[Id], do.[Name], [From], [To], [ScheduleSettingsId] 
                            FROM [DayOff] do
                            INNER JOIN [ScheduleSettings] ss ON ss.[Id] = do.[ScheduleSettingsId]
                            WHERE ss.[ScheduleId] = @Id 
                            ORDER BY [Id];",
                    guidPar
                );
                if (daysOff != null)
                {
                    collisionScanBlocks.daysOffFull = daysOff.ToList();

                    collisionScanBlocks.daysOff = new List<DayOffInstance>();
                    foreach (DayOff dayOff in collisionScanBlocks.daysOffFull)
                    {
                        for (
                            DateOnly startDate = dayOff.From;
                            startDate <= dayOff.To;
                            startDate = startDate.AddDays(1)
                        )
                        {
                            collisionScanBlocks.daysOff.Add(
                                new DayOffInstance(dayOff.Id, dayOff.Name, startDate)
                            );
                        }
                    }
                }
            }
            else
            {
                if (collisionScanInfo.scanDate != null)
                {
                    TimeIntervalPar timeIntervalPar = new TimeIntervalPar(
                        scheduleId,
                        DateOnlyUtils.getWeekStart(collisionScanInfo.scanDate.Value),
                        DateOnlyUtils.getWeekEnd(collisionScanInfo.scanDate.Value)
                    );
                    GuidPar guidPar = new GuidPar(scheduleId);
                    IEnumerable<CollisionLessonBlock>? collisionLessonBlocks =
                        new List<CollisionLessonBlock>();
                    collisionLessonBlocks = await dbService.GetAll<CollisionLessonBlock>(
                        $@"
                            SELECT
                            lb.[Id], [LessonDate], [LessonStart], [LessonEnd], lb.[LessonId], [ClassroomId], [ClusterId]
                            FROM [LessonBlock] lb
                            INNER JOIN [Lesson] l ON l.[Id] = lb.[LessonId]
                            INNER JOIN [LessonType] lt ON lt.[Id] = l.[LessonTypeId]
                            WHERE lt.[ScheduleId] = @Id AND [LessonDate] BETWEEN @FromDate AND @ToDate
                            ORDER BY lb.[Id];",
                        timeIntervalPar
                    );

                    if (collisionLessonBlocks != null)
                    {
                        collisionScanBlocks.lessonBlocks = collisionLessonBlocks.ToList();

                        Dictionary<Guid, Lesson> lessonMap = new Dictionary<Guid, Lesson>();
                        Dictionary<Guid, Classroom> classroomMap =
                            new Dictionary<Guid, Classroom>();
                        Dictionary<Guid, IEnumerable<Subgroup>?> subgroupsMap =
                            new Dictionary<Guid, IEnumerable<Subgroup>?>();
                        Dictionary<Guid, IEnumerable<ClassroomType>?> classroomTypeLessonMap =
                            new Dictionary<Guid, IEnumerable<ClassroomType>?>();
                        Dictionary<Guid, IEnumerable<ClassroomType>?> classroomTypeClassroomMap =
                            new Dictionary<Guid, IEnumerable<ClassroomType>?>();

                        foreach (var block in collisionScanBlocks.lessonBlocks)
                        {
                            if (!lessonMap.ContainsKey(block.LessonId))
                            {
                                GetLessonHandler getLessonHandler = new GetLessonHandler(dbService);
                                GetLessonQuery getLessonQuery = new GetLessonQuery(
                                    block.LessonId,
                                    new Guid(),
                                    "Admin"
                                );
                                ActionResult<Lesson?> lesson = await getLessonHandler.Handle(
                                    getLessonQuery,
                                    cancellationToken
                                );

                                lessonMap.Add(block.LessonId, lesson.Value!);
                            }
                            block.Lesson = lessonMap[block.LessonId];

                            if (block.ClassroomId != null)
                            {
                                if (!classroomMap.ContainsKey(block.ClassroomId.Value))
                                {
                                    GetClassroomHandler getClassroomHandler =
                                        new GetClassroomHandler(dbService);
                                    GetClassroomQuery getClassroomQuery = new GetClassroomQuery(
                                        block.ClassroomId.Value,
                                        new Guid(),
                                        "Admin"
                                    );
                                    ActionResult<Classroom?> classroom =
                                        await getClassroomHandler.Handle(
                                            getClassroomQuery,
                                            cancellationToken
                                        );

                                    classroomMap.Add(block.ClassroomId.Value, classroom.Value!);
                                }
                                block.Classroom = classroomMap[block.ClassroomId.Value];
                            }

                            if (!subgroupsMap.ContainsKey(block.LessonId))
                            {
                                GuidPar lessonId = new GuidPar(block.LessonId);
                                IEnumerable<Subgroup>? subgroups = await dbService.GetAll<Subgroup>(
                                    $@"
                                    SELECT
                                    sg.[Id], sg.[Name], sg.[StudentCount], sg.[GroupId] 
                                    FROM [Subgroup] sg
                                    LEFT JOIN [LessonSubgroup] lsg ON lsg.[SubgroupId] = sg.[Id]
                                    LEFT JOIN [Lesson] l ON l.[Id] = lsg.[LessonId]
                                    WHERE l.[Id] = @Id
                                    ORDER BY [Id];",
                                    lessonId
                                );
                                subgroupsMap.Add(block.LessonId, subgroups);
                            }
                            block.Subgroups = subgroupsMap[block.LessonId];

                            if (!classroomTypeLessonMap.ContainsKey(block.LessonId))
                            {
                                GuidPar lessonId = new GuidPar(block.LessonId);
                                IEnumerable<ClassroomType>? classroomTypes =
                                    await dbService.GetAll<ClassroomType>(
                                        $@"
                                    SELECT
                                    ct.[Id], ct.[Name], ct.[ScheduleId] 
                                    FROM [ClassroomType] ct
                                    LEFT JOIN [LessonClassroomType] lct ON lct.[ClassroomTypeId] = ct.[Id]
                                    LEFT JOIN [Lesson] l ON l.[Id] = lct.[LessonId]
                                    WHERE l.[Id] = @Id
                                    ORDER BY [Id];",
                                        lessonId
                                    );
                                classroomTypeLessonMap.Add(block.LessonId, classroomTypes);
                            }
                            block.ClassroomTypeslesson = classroomTypeLessonMap[block.LessonId];

                            if (block.ClassroomId != null)
                            {
                                Guid notNullClassroomId = block.ClassroomId.Value;
                                if (!classroomTypeClassroomMap.ContainsKey(notNullClassroomId))
                                {
                                    GuidPar classroomId = new GuidPar(notNullClassroomId);
                                    IEnumerable<ClassroomType>? classroomTypes =
                                        await dbService.GetAll<ClassroomType>(
                                            $@"
                                    SELECT
                                    ct.[Id], ct.[Name], ct.[ScheduleId] 
                                    FROM [ClassroomType] ct
                                    LEFT JOIN [ClassroomClassroomType] cct ON cct.[ClassroomTypeId] = ct.[Id]
                                    LEFT JOIN [Classroom] c ON c.[Id] = cct.[ClassroomId]
                                    WHERE c.[Id] = @Id
                                    ORDER BY [Id];",
                                            classroomId
                                        );
                                    classroomTypeClassroomMap.Add(
                                        notNullClassroomId,
                                        classroomTypes
                                    );
                                }
                                block.ClassroomTypesClassroom = classroomTypeClassroomMap[
                                    notNullClassroomId
                                ];
                            }
                        }
                    }

                    IEnumerable<DayOff>? daysOff = new List<DayOff>();
                    daysOff = await dbService.GetAll<DayOff>(
                        $@"
                            SELECT
                            do.[Id], do.[Name], [From], [To], [ScheduleSettingsId] 
                            FROM [DayOff] do
                            INNER JOIN [ScheduleSettings] ss ON ss.[Id] = do.[ScheduleSettingsId]
                            WHERE ss.[ScheduleId] = @Id AND ([From] BETWEEN @FromDate AND @ToDate OR [To] BETWEEN @FromDate AND @ToDate)
                            ORDER BY [Id];",
                        timeIntervalPar
                    );
                    if (daysOff != null)
                    {
                        collisionScanBlocks.daysOffFull = daysOff.ToList();

                        collisionScanBlocks.daysOff = new List<DayOffInstance>();
                        foreach (DayOff dayOff in collisionScanBlocks.daysOffFull)
                        {
                            for (
                                DateOnly startDate = dayOff.From;
                                startDate <= dayOff.To;
                                startDate = startDate.AddDays(1)
                            )
                            {
                                collisionScanBlocks.daysOff.Add(
                                    new DayOffInstance(dayOff.Id, dayOff.Name, startDate)
                                );
                            }
                        }
                    }
                }
            }

            if (logMaker.isLogEnabled())
            {
                if (collisionScanBlocks.lessonBlocks != null)
                {
                    foreach (CollisionLessonBlock block in collisionScanBlocks.lessonBlocks)
                    {
                        logMaker.writeLog(("  - Data for Lessonblock: " + block.Id).ToString());
                        logMaker.writeLog(
                            (
                                "   * LessonBlock "
                                + block.Id
                                + " "
                                + block.LessonDate
                                + " "
                                + block.LessonStart
                                + " "
                                + block.LessonEnd
                                + " "
                                + block.LessonId
                                + " "
                                + block.ClassroomId
                                + " "
                                + block.ClusterId
                            ).ToString()
                        );
                        if (block.Lesson != null)
                        {
                            logMaker.writeLog(
                                (
                                    "   * Lesson "
                                    + block.Lesson.Id
                                    + " "
                                    + block.Lesson.Name
                                    + " "
                                    + block.Lesson.CurrentHours
                                    + " "
                                    + block.Lesson.AmountOfHours
                                    + " "
                                    + block.Lesson.LessonTypeId
                                    + " "
                                    + block.Lesson.TeacherId
                                ).ToString()
                            );
                            if (block.Lesson.LessonType != null)
                            {
                                logMaker.writeLog(
                                    (
                                        "   * LessonType "
                                        + block.Lesson.LessonType.Id
                                        + " "
                                        + block.Lesson.LessonType.Name
                                        + " "
                                        + block.Lesson.LessonType.Color
                                    ).ToString()
                                );
                            }
                            if (block.Lesson.Teacher != null)
                            {
                                logMaker.writeLog(
                                    (
                                        "   * Teacher "
                                        + block.Lesson.Teacher.Id
                                        + " "
                                        + block.Lesson.Teacher.Name
                                        + " "
                                        + block.Lesson.Teacher.Surname
                                    ).ToString()
                                );
                            }
                        }
                        if (block.Subgroups != null)
                        {
                            foreach (var subgroup in block.Subgroups)
                            {
                                logMaker.writeLog(
                                    (
                                        "   * Subgroup "
                                        + subgroup.Id
                                        + " "
                                        + subgroup.Name
                                        + " "
                                        + subgroup.StudentCount
                                        + " "
                                        + subgroup.GroupId
                                    ).ToString()
                                );
                            }
                        }
                        if (block.ClassroomTypeslesson != null)
                        {
                            foreach (var classroomType in block.ClassroomTypeslesson)
                            {
                                logMaker.writeLog(
                                    (
                                        "   * ClassroomType lesson: "
                                        + classroomType.Id
                                        + " "
                                        + classroomType.Name
                                        + " "
                                        + classroomType.ScheduleId
                                    ).ToString()
                                );
                            }
                        }
                        if (block.ClassroomTypesClassroom != null)
                        {
                            foreach (var classroomType in block.ClassroomTypesClassroom)
                            {
                                logMaker.writeLog(
                                    (
                                        "   * ClassroomType classroom: "
                                        + classroomType.Id
                                        + " "
                                        + classroomType.Name
                                        + " "
                                        + classroomType.ScheduleId
                                    ).ToString()
                                );
                            }
                        }
                    }
                }

                if (collisionScanBlocks.daysOff != null)
                {
                    foreach (DayOffInstance block in collisionScanBlocks.daysOff)
                    {
                        logMaker.writeLog(("  - Data for DayOff: " + block.Id).ToString());
                        logMaker.writeLog(
                            ("   * " + block.Id + " " + block.Name + " " + block.Date).ToString()
                        );
                    }
                }
            }

            return collisionScanBlocks;
        }
    }
}
