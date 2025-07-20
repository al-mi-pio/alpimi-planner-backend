using AlpimiAPI.Database;
using AlpimiAPI.Entities.ELessonPeriod;
using AlpimiAPI.Entities.EScheduleSettings;
using AlpimiAPI.Entities.EScheduleSettings.Queries;
using AlpimiAPI.Locales;
using AlpimiAPI.Responses;
using MediatR;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Localization;

namespace AlpimiAPI.Entities.ELessonPeriod.Queries
{
    public record GetAllLessonPeriodByScheduleQuery(
        Guid ScheduleId,
        Guid FilteredId,
        string Role,
        PaginationParams Pagination
    ) : IRequest<(IEnumerable<LessonPeriod>?, int)>;

    public class GetAllLessonPeriodByScheduleHandler
        : IRequestHandler<GetAllLessonPeriodByScheduleQuery, (IEnumerable<LessonPeriod>?, int)>
    {
        private readonly IDbService _dbService;
        private readonly IStringLocalizer<Errors> _str;
        private readonly IStringLocalizer<Fields> _strFields;

        public GetAllLessonPeriodByScheduleHandler(
            IDbService dbService,
            IStringLocalizer<Errors> str,
            IStringLocalizer<Fields> strFields
        )
        {
            _dbService = dbService;
            _str = str;
            _strFields = strFields;
        }

        public async Task<(IEnumerable<LessonPeriod>?, int)> Handle(
            GetAllLessonPeriodByScheduleQuery request,
            CancellationToken cancellationToken
        )
        {
            List<ErrorObject> errors = new List<ErrorObject>();
            if (request.Pagination.PerPage < 0)
            {
                errors.Add(
                    new FieldErrorObject("perPage", _str["badParameter", _strFields["PerPage"]])
                );
            }
            if (request.Pagination.Offset < 0)
            {
                errors.Add(new FieldErrorObject("page", _str["badParameter", _strFields["Page"]]));
            }
            if (
                request.Pagination.SortOrder.ToLower() != "asc"
                && request.Pagination.SortOrder.ToLower() != "desc"
            )
            {
                errors.Add(
                    new FieldErrorObject("sortOrder", _str["badParameter", _strFields["SortOrder"]])
                );
            }
            if (request.Pagination.SortBy != "Id" && request.Pagination.SortBy != "Start")
            {
                errors.Add(
                    new FieldErrorObject("sortBy", _str["badParameter", _strFields["SortBy"]])
                );
            }

            if (errors.Count != 0)
            {
                throw new ApiErrorException(errors);
            }

            IEnumerable<LessonPeriod>? lessonPeriods;
            int count;
            switch (request.Role)
            {
                case "Admin":
                    count = await _dbService.Get<int>(
                        @"
                            SELECT 
                            COUNT(*)
                            FROM [LessonPeriod] lp
                            INNER JOIN [ScheduleSettings] ss ON ss.[Id] = lp.[ScheduleSettingsId]
                            WHERE ss.[ScheduleId] = @ScheduleId;",
                        request
                    );
                    lessonPeriods = await _dbService.GetAll<LessonPeriod>(
                        $@"
                            SELECT
                            lp.[Id], [Start], [ScheduleSettingsId] 
                            FROM [LessonPeriod] lp
                            INNER JOIN [ScheduleSettings] ss ON ss.[Id] = lp.[ScheduleSettingsId]
                            WHERE ss.[ScheduleId] = @ScheduleId 
                            ORDER BY
                            {request.Pagination.SortBy}
                            {request.Pagination.SortOrder}
                            OFFSET
                            {request.Pagination.Offset} ROWS
                            FETCH NEXT
                            {request.Pagination.PerPage} ROWS ONLY;",
                        request
                    );
                    break;
                case "User":
                    count = await _dbService.Get<int>(
                        @"
                            SELECT
                            COUNT(*)
                            FROM [LessonPeriod] lp
                            INNER JOIN [ScheduleSettings] ss ON ss.[Id] = lp.[ScheduleSettingsId]
                            INNER JOIN [Schedule] s ON s.[Id]=ss.[ScheduleId]
                            WHERE (s.[UserId] = @FilteredId OR ss.[IsPublic] = 'TRUE') AND ss.[ScheduleId] =@ScheduleId;",
                        request
                    );
                    lessonPeriods = await _dbService.GetAll<LessonPeriod>(
                        $@"
                            SELECT 
                            lp.[Id], [Start], [ScheduleSettingsId] 
                            FROM [LessonPeriod] lp
                            INNER JOIN [ScheduleSettings] ss ON ss.[Id] = lp.[ScheduleSettingsId]
                            INNER JOIN [Schedule] s ON s.[Id]=ss.[ScheduleId]
                            WHERE (s.[UserId] = @FilteredId OR ss.[IsPublic] = 'TRUE') AND ss.[ScheduleId] = @ScheduleId 
                            ORDER BY
                            {request.Pagination.SortBy}
                            {request.Pagination.SortOrder}
                            OFFSET
                            {request.Pagination.Offset} ROWS
                            FETCH NEXT
                            {request.Pagination.PerPage} ROWS ONLY;",
                        request
                    );
                    break;
                default:
                    count = await _dbService.Get<int>(
                        @"
                            SELECT
                            COUNT(*)
                            FROM [LessonPeriod] lp
                            INNER JOIN [ScheduleSettings] ss ON ss.[Id] = lp.[ScheduleSettingsId]
                            INNER JOIN [Schedule] s ON s.[Id]=ss.[ScheduleId]
                            WHERE ss.[IsPublic] = 'TRUE' AND ss.[ScheduleId] =@ScheduleId;",
                        request
                    );
                    lessonPeriods = await _dbService.GetAll<LessonPeriod>(
                        $@"
                            SELECT 
                            lp.[Id], [Start], [ScheduleSettingsId] 
                            FROM [LessonPeriod] lp
                            INNER JOIN [ScheduleSettings] ss ON ss.[Id] = lp.[ScheduleSettingsId]
                            INNER JOIN [Schedule] s ON s.[Id]=ss.[ScheduleId]
                            WHERE ss.[IsPublic] = 'TRUE' AND ss.[ScheduleId] = @ScheduleId 
                            ORDER BY
                            {request.Pagination.SortBy}
                            {request.Pagination.SortOrder}
                            OFFSET
                            {request.Pagination.Offset} ROWS
                            FETCH NEXT
                            {request.Pagination.PerPage} ROWS ONLY;",
                        request
                    );
                    break;
            }

            if (lessonPeriods != null)
            {
                Dictionary<Guid, ScheduleSettings> scheduleSettingsMap =
                    new Dictionary<Guid, ScheduleSettings>();
                foreach (var lessonPeriod in lessonPeriods)
                {
                    if (!scheduleSettingsMap.ContainsKey(lessonPeriod.ScheduleSettingsId))
                    {
                        GetScheduleSettingsHandler getScheduleSettingsHandler =
                            new GetScheduleSettingsHandler(_dbService);
                        GetScheduleSettingsQuery getScheduleSettingsQuery =
                            new GetScheduleSettingsQuery(
                                lessonPeriod.ScheduleSettingsId,
                                new Guid(),
                                "Admin"
                            );
                        ActionResult<ScheduleSettings?> scheduleSettings =
                            await getScheduleSettingsHandler.Handle(
                                getScheduleSettingsQuery,
                                cancellationToken
                            );

                        scheduleSettingsMap.Add(
                            lessonPeriod.ScheduleSettingsId,
                            scheduleSettings.Value!
                        );
                    }
                    lessonPeriod.ScheduleSettings = scheduleSettingsMap[
                        lessonPeriod.ScheduleSettingsId
                    ];
                }
            }

            return (lessonPeriods, count);
        }
    }
}
