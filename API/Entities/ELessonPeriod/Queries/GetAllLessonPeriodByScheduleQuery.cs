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

        public GetAllLessonPeriodByScheduleHandler(
            IDbService dbService,
            IStringLocalizer<Errors> str
        )
        {
            _dbService = dbService;
            _str = str;
        }

        public async Task<(IEnumerable<LessonPeriod>?, int)> Handle(
            GetAllLessonPeriodByScheduleQuery request,
            CancellationToken cancellationToken
        )
        {
            List<ErrorObject> errors = new List<ErrorObject>();
            if (request.Pagination.PerPage < 0)
            {
                errors.Add(new ErrorObject(_str["badParameter", "PerPage"]));
            }
            if (request.Pagination.Offset < 0)
            {
                errors.Add(new ErrorObject(_str["badParameter", "Page"]));
            }
            if (
                request.Pagination.SortOrder.ToLower() != "asc"
                && request.Pagination.SortOrder.ToLower() != "desc"
            )
            {
                errors.Add(new ErrorObject(_str["badParameter", "SortOrder"]));
            }
            if (request.Pagination.SortBy != "Id" && request.Pagination.SortBy != "Start")
            {
                errors.Add(new ErrorObject(_str["badParameter", "SortBy"]));
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
                            WHERE ss.[ScheduleId] = @ScheduleId",
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
                            {request.Pagination.PerPage} ROWS ONLY; ",
                        request
                    );
                    break;
                default:
                    count = await _dbService.Get<int>(
                        @"
                            SELECT COUNT(*)
                            FROM [LessonPeriod] lp
                            INNER JOIN [ScheduleSettings] ss ON ss.[Id] = lp.[ScheduleSettingsId]
                            INNER JOIN [Schedule] s ON s.[Id]=ss.[ScheduleId]
                            WHERE s.[UserId] = @FilteredId AND ss.[ScheduleId] =@ScheduleId
                            ",
                        request
                    );
                    lessonPeriods = await _dbService.GetAll<LessonPeriod>(
                        $@"
                            SELECT 
                            lp.[Id], [Start], [ScheduleSettingsId] 
                            FROM [LessonPeriod] lp
                            INNER JOIN [ScheduleSettings] ss ON ss.[Id] = lp.[ScheduleSettingsId]
                            INNER JOIN [Schedule] s ON s.[Id]=ss.[ScheduleId]
                            WHERE s.[UserId] = @FilteredId AND ss.[ScheduleId] = @ScheduleId 
                            ORDER BY
                            {request.Pagination.SortBy}
                            {request.Pagination.SortOrder}
                            OFFSET
                            {request.Pagination.Offset} ROWS
                            FETCH NEXT
                            {request.Pagination.PerPage} ROWS ONLY; ",
                        request
                    );
                    break;
            }
            if (lessonPeriods != null)
            {
                foreach (var lessonPeriod in lessonPeriods)
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
                    lessonPeriod.ScheduleSettings = scheduleSettings.Value!;
                }
            }
            return (lessonPeriods, count);
        }
    }
}
