﻿using AlpimiAPI.Database;
using AlpimiAPI.Entities.EScheduleSettings;
using AlpimiAPI.Entities.EScheduleSettings.Queries;
using AlpimiAPI.Responses;
using MediatR;
using Microsoft.AspNetCore.Mvc;

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

        public GetAllDayOffByScheduleHandler(IDbService dbService)
        {
            _dbService = dbService;
        }

        public async Task<(IEnumerable<DayOff>?, int)> Handle(
            GetAllDayOffByScheduleQuery request,
            CancellationToken cancellationToken
        )
        {
            List<ErrorObject> errors = new List<ErrorObject>();
            if (request.Pagination.PerPage < 0)
            {
                errors.Add(new ErrorObject("Bad PerPage"));
            }
            if (request.Pagination.Offset < 0)
            {
                errors.Add(new ErrorObject("Bad Page"));
            }
            if (
                request.Pagination.SortOrder.ToLower() != "asc"
                && request.Pagination.SortOrder.ToLower() != "desc"
            )
            {
                errors.Add(new ErrorObject("Bad SortOrder"));
            }
            if (
                request.Pagination.SortBy.ToLower() != "id"
                && request.Pagination.SortBy.ToLower() != "name"
                && request.Pagination.SortBy.ToLower() != "schoolhour"
                && request.Pagination.SortBy.ToLower() != "userid"
            )
            {
                errors.Add(new ErrorObject("Bad SortBy"));
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
                            SELECT COUNT(*)
                            FROM [DayOff] do
                            INNER JOIN [ScheduleSettings] ss ON ss.[Id] = do.[ScheduleSettingsId]
                            WHERE ss.[ScheduleId] = @ScheduleId
                            ",
                        request
                    );
                    daysOff = await _dbService.GetAll<DayOff>(
                        @"
                            SELECT do.[Id], do.[Name], [From],[To],[ScheduleSettingsId] FROM [DayOff] do
                            INNER JOIN [ScheduleSettings] ss ON ss.[Id] = do.[ScheduleSettingsId]
                            WHERE ss.[ScheduleId] = @ScheduleId ORDER BY'"
                            + request.Pagination.SortBy
                            + "' "
                            + request.Pagination.SortOrder
                            + " OFFSET "
                            + request.Pagination.Offset
                            + " ROWS FETCH NEXT "
                            + request.Pagination.PerPage
                            + " ROWS ONLY;",
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
                        @"SELECT do.[Id], do.[Name], [From],[To],[ScheduleSettingsId] FROM [DayOff] do
                            INNER JOIN [ScheduleSettings] ss ON ss.[Id] = do.[ScheduleSettingsId]
                            INNER JOIN [Schedule] s ON s.[Id]=ss.[ScheduleId]
                            WHERE s.[UserId] = @FilteredId AND ss.[ScheduleId] = @ScheduleId ORDER BY'"
                            + request.Pagination.SortBy
                            + "' "
                            + request.Pagination.SortOrder
                            + " OFFSET "
                            + request.Pagination.Offset
                            + " ROWS FETCH NEXT "
                            + request.Pagination.PerPage
                            + " ROWS ONLY;",
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