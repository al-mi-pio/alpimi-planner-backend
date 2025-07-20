using AlpimiAPI.Database;
using AlpimiAPI.Entities.ESchedule;
using AlpimiAPI.Entities.ESchedule.Queries;
using AlpimiAPI.Locales;
using AlpimiAPI.Responses;
using MediatR;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Localization;

namespace AlpimiAPI.Entities.ETeacher.Queries
{
    public record GetAllTeachersByScheduleQuery(
        Guid ScheduleId,
        Guid FilteredId,
        string Role,
        PaginationParams Pagination
    ) : IRequest<(IEnumerable<Teacher>?, int)>;

    public class GetAllTeachersByScheduleHandler
        : IRequestHandler<GetAllTeachersByScheduleQuery, (IEnumerable<Teacher>?, int)>
    {
        private readonly IDbService _dbService;
        private readonly IStringLocalizer<Errors> _str;
        private readonly IStringLocalizer<Fields> _strFields;

        public GetAllTeachersByScheduleHandler(
            IDbService dbService,
            IStringLocalizer<Errors> str,
            IStringLocalizer<Fields> strFields
        )
        {
            _dbService = dbService;
            _str = str;
            _strFields = strFields;
        }

        public async Task<(IEnumerable<Teacher>?, int)> Handle(
            GetAllTeachersByScheduleQuery request,
            CancellationToken cancellationToken
        )
        {
            List<ErrorObject> errors = new List<ErrorObject>();
            if (request.Pagination.PerPage < 0)
            {
                errors.Add(
                    new FieldErrorObject("PerPage", _str["badParameter", _strFields["PerPage"]])
                );
            }
            if (request.Pagination.Offset < 0)
            {
                errors.Add(new FieldErrorObject("Page", _str["badParameter", _strFields["Page"]]));
            }
            if (
                request.Pagination.SortOrder.ToLower() != "asc"
                && request.Pagination.SortOrder.ToLower() != "desc"
            )
            {
                errors.Add(
                    new FieldErrorObject("SortOrder", _str["badParameter", _strFields["SortOrder"]])
                );
            }
            if (
                request.Pagination.SortBy != "Id"
                && request.Pagination.SortBy != "Name"
                && request.Pagination.SortBy != "Surname"
            )
            {
                errors.Add(
                    new FieldErrorObject("SortBy", _str["badParameter", _strFields["SortBy"]])
                );
            }

            if (errors.Count != 0)
            {
                throw new ApiErrorException(errors);
            }

            IEnumerable<Teacher>? teachers;
            int count;
            switch (request.Role)
            {
                case "Admin":
                    count = await _dbService.Get<int>(
                        @"
                            SELECT 
                            COUNT(*)
                            FROM [Teacher] 
                            WHERE [ScheduleId] = @ScheduleId;",
                        request
                    );
                    teachers = await _dbService.GetAll<Teacher>(
                        $@"
                            SELECT
                            [Id], [Name], [Surname], [ScheduleId] 
                            FROM [Teacher]
                            WHERE [ScheduleId] = @ScheduleId 
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
                            FROM [Teacher] t
                            INNER JOIN [Schedule] s ON s.[Id]=t.[ScheduleId]
                            WHERE s.[UserId] = @FilteredId AND t.[ScheduleId] = @ScheduleId;",
                        request
                    );
                    teachers = await _dbService.GetAll<Teacher>(
                        $@"
                            SELECT 
                            t.[Id], t.[Name], [Surname], [ScheduleId] 
                            FROM [Teacher] t
                            INNER JOIN [Schedule] s ON s.[Id] = t.[ScheduleId]
                            WHERE s.[UserId] = @FilteredId AND t.[ScheduleId] = @ScheduleId 
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

            if (teachers != null)
            {
                Dictionary<Guid, Schedule> scheduleMap = new Dictionary<Guid, Schedule>();
                foreach (var teacher in teachers)
                {
                    if (!scheduleMap.ContainsKey(teacher.ScheduleId))
                    {
                        GetScheduleHandler getScheduleHandler = new GetScheduleHandler(_dbService);
                        GetScheduleQuery getScheduleQuery = new GetScheduleQuery(
                            teacher.ScheduleId,
                            new Guid(),
                            "Admin"
                        );
                        ActionResult<Schedule?> schedule = await getScheduleHandler.Handle(
                            getScheduleQuery,
                            cancellationToken
                        );

                        scheduleMap.Add(teacher.ScheduleId, schedule.Value!);
                    }
                    teacher.Schedule = scheduleMap[teacher.ScheduleId];
                }
            }

            return (teachers, count);
        }
    }
}
