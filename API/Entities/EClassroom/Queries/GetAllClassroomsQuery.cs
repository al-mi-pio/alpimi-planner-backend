using AlpimiAPI.Database;
using AlpimiAPI.Entities.ESchedule;
using AlpimiAPI.Entities.ESchedule.Queries;
using AlpimiAPI.Locales;
using AlpimiAPI.Responses;
using MediatR;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Localization;

namespace AlpimiAPI.Entities.EClassroom.Queries
{
    public record GetAllClassroomsQuery(
        Guid Id,
        Guid FilteredId,
        string Role,
        PaginationParams Pagination
    ) : IRequest<(IEnumerable<Classroom>?, int)>;

    public class GetAllClassroomsHandler
        : IRequestHandler<GetAllClassroomsQuery, (IEnumerable<Classroom>?, int)>
    {
        private readonly IDbService _dbService;
        private readonly IStringLocalizer<Errors> _str;

        public GetAllClassroomsHandler(IDbService dbService, IStringLocalizer<Errors> str)
        {
            _dbService = dbService;
            _str = str;
        }

        public async Task<(IEnumerable<Classroom>?, int)> Handle(
            GetAllClassroomsQuery request,
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
                request.Pagination.SortBy != "Id"
                && request.Pagination.SortBy != "Name"
                && request.Pagination.SortBy != "Capacity"
            )
            {
                errors.Add(new ErrorObject(_str["badParameter", "SortBy"]));
            }

            if (errors.Count != 0)
            {
                throw new ApiErrorException(errors);
            }

            IEnumerable<Classroom>? classrooms;
            int count;
            switch (request.Role)
            {
                case "Admin":
                    count = await _dbService.Get<int>(
                        @"
                            SELECT 
                            COUNT(*)
                            FROM [Classroom] c
                            LEFT JOIN [ClassroomClassroomType] cct ON cct.[ClassroomId] = c.[Id]
                            LEFT JOIN [ClassroomType] ct on ct.[Id] = cct.[ClassroomTypeId]
                            WHERE c.[ScheduleId] = @Id OR ct.[Id] = @Id",
                        request
                    );
                    classrooms = await _dbService.GetAll<Classroom>(
                        $@"
                            SELECT
                            c.[Id], c.[Name], c.[Capacity], c.[ScheduleId] 
                            FROM [Classroom] c
                            LEFT JOIN [ClassroomClassroomType] cct ON cct.[ClassroomId] = c.[Id]
                            LEFT JOIN [ClassroomType] ct on ct.[Id] = cct.[ClassroomTypeId]
                            WHERE c.[ScheduleId] = @Id OR ct.[Id] = @Id
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
                            FROM [Classroom] c
                            INNER JOIN [Schedule] s ON s.[Id] = c.[ScheduleId]
                            LEFT JOIN [ClassroomClassroomType] cct ON cct.[ClassroomId] = c.[Id]
                            LEFT JOIN [ClassroomType] ct on ct.[Id] = cct.[ClassroomTypeId]
                            WHERE s.[UserId] = @FilteredId AND (c.[ScheduleId] = @Id OR ct.[Id] = @Id);",
                        request
                    );
                    classrooms = await _dbService.GetAll<Classroom>(
                        $@"
                            SELECT 
                            c.[Id], c.[Name], c.[Capacity], [ScheduleId] 
                            FROM [Classroom] c
                            INNER JOIN [Schedule] s ON s.[Id] = c.[ScheduleId]
                            LEFT JOIN [ClassroomClassroomType] cct ON cct.[ClassroomId] = c.[Id]
                            LEFT JOIN [ClassroomType] ct on ct.[Id] = cct.[ClassroomTypeId]
                            WHERE s.[UserId] = @FilteredId AND (c.[ScheduleId] = @Id OR ct.[Id] = @Id)
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
            if (classrooms != null)
            {
                foreach (var classroom in classrooms)
                {
                    GetScheduleHandler getScheduleHandler = new GetScheduleHandler(_dbService);
                    GetScheduleQuery getScheduleQuery = new GetScheduleQuery(
                        classroom.ScheduleId,
                        new Guid(),
                        "Admin"
                    );
                    ActionResult<Schedule?> schedule = await getScheduleHandler.Handle(
                        getScheduleQuery,
                        cancellationToken
                    );
                    classroom.Schedule = schedule.Value!;
                }
            }
            return (classrooms, count);
        }
    }
}
