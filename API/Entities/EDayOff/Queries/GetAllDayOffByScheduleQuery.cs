using AlpimiAPI.Database;
using AlpimiAPI.Entities.EScheduleSettings;
using AlpimiAPI.Entities.EScheduleSettings.Queries;
using AlpimiAPI.Locales;
using AlpimiAPI.Responses;
using MediatR;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Localization;

namespace AlpimiAPI.Entities.EDayOff.Queries
{
    public record GetAllDayOffByScheduleQuery(
        Guid ScheduleId,
        Guid FilteredId,
        string Role,
        PaginationParams Pagination
    ) : IRequest<(IEnumerable<DayOff>?, int)>;

    public class GetAllDayOffByScheduleHandler
        : IRequestHandler<GetAllDayOffByScheduleQuery, (IEnumerable<DayOff>?, int)>
    {
        private readonly IDbService _dbService;
        private readonly IStringLocalizer<Errors> _str;

        public GetAllDayOffByScheduleHandler(IDbService dbService, IStringLocalizer<Errors> str)
        {
            _dbService = dbService;
            _str = str;
        }

        public async Task<(IEnumerable<DayOff>?, int)> Handle(
            GetAllDayOffByScheduleQuery request,
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
            if (
                request.Pagination.SortBy.ToLower() != "id"
                && request.Pagination.SortBy.ToLower() != "from"
                && request.Pagination.SortBy.ToLower() != "to"
            )
            {
                errors.Add(new ErrorObject(_str["badParameter", "SortBy"]));
            }

            if (errors.Count != 0)
            {
                throw new ApiErrorException(errors);
            }

            IEnumerable<DayOff>? daysOff;
            int count;
            switch (request.Role)
            {
                case "Admin":
                    count = await _dbService.Get<int>(
                        @"
                            SELECT 
                            COUNT(*)
                            FROM [DayOff] do
                            INNER JOIN [ScheduleSettings] ss ON ss.[Id] = do.[ScheduleSettingsId]
                            WHERE ss.[ScheduleId] = @ScheduleId",
                        request
                    );
                    daysOff = await _dbService.GetAll<DayOff>(
                        $@"
                            SELECT
                            do.[Id], do.[Name], [From],[To],[ScheduleSettingsId] 
                            FROM [DayOff] do
                            INNER JOIN [ScheduleSettings] ss ON ss.[Id] = do.[ScheduleSettingsId]
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
                            FROM [DayOff] do
                            INNER JOIN [ScheduleSettings] ss ON ss.[Id] = do.[ScheduleSettingsId]
                            INNER JOIN [Schedule] s ON s.[Id]=ss.[ScheduleId]
                            WHERE s.[UserId] = @FilteredId AND ss.[ScheduleId] = @ScheduleId
                            ",
                        request
                    );
                    daysOff = await _dbService.GetAll<DayOff>(
                        $@"
                            SELECT 
                            do.[Id], do.[Name], [From],[To],[ScheduleSettingsId]
                            FROM [DayOff] do
                            INNER JOIN [ScheduleSettings] ss ON ss.[Id] = do.[ScheduleSettingsId]
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
            if (daysOff != null)
            {
                foreach (var dayOff in daysOff)
                {
                    GetScheduleSettingsHandler getScheduleSettingsHandler =
                        new GetScheduleSettingsHandler(_dbService);
                    GetScheduleSettingsQuery getScheduleSettingsQuery =
                        new GetScheduleSettingsQuery(
                            dayOff.ScheduleSettingsId,
                            new Guid(),
                            "Admin"
                        );
                    ActionResult<ScheduleSettings?> scheduleSettings =
                        await getScheduleSettingsHandler.Handle(
                            getScheduleSettingsQuery,
                            cancellationToken
                        );
                    dayOff.ScheduleSettings = scheduleSettings.Value!;
                }
            }
            return (daysOff, count);
        }
    }
}
