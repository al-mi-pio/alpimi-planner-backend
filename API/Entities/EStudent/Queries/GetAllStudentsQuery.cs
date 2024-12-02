using AlpimiAPI.Database;
using AlpimiAPI.Entities.EGroup;
using AlpimiAPI.Entities.EGroup.Queries;
using AlpimiAPI.Locales;
using AlpimiAPI.Responses;
using MediatR;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Localization;

namespace AlpimiAPI.Entities.EStudent.Queries
{
    public record GetAllStudentsQuery(
        Guid Id,
        Guid FilteredId,
        string Role,
        PaginationParams Pagination
    ) : IRequest<(IEnumerable<Student>?, int)>;

    public class GetAllStudentsHandler
        : IRequestHandler<GetAllStudentsQuery, (IEnumerable<Student>?, int)>
    {
        private readonly IDbService _dbService;
        private readonly IStringLocalizer<Errors> _str;

        public GetAllStudentsHandler(IDbService dbService, IStringLocalizer<Errors> str)
        {
            _dbService = dbService;
            _str = str;
        }

        public async Task<(IEnumerable<Student>?, int)> Handle(
            GetAllStudentsQuery request,
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
            if (request.Pagination.SortBy != "id" && request.Pagination.SortBy != "AlbumNumber")
            {
                errors.Add(new ErrorObject(_str["badParameter", "SortBy"]));
            }

            if (errors.Count != 0)
            {
                throw new ApiErrorException(errors);
            }

            IEnumerable<Student>? students;
            int count;
            switch (request.Role)
            {
                case "Admin":
                    count = await _dbService.Get<int>(
                        @"
                            SELECT 
                            COUNT(*)
                            FROM [Student] st
                            INNER JOIN [Group] g ON g.[Id] = st.[GroupId]
                            LEFT JOIN [StudentSubgroup] ssg ON ssg.[StudentId] = st.[Id]
                            LEFT JOIN [Subgroup] sg ON sg.[Id] = ssg.[SubgroupId]
                            WHERE st.[GroupId] = @Id OR g.[ScheduleId] = @Id OR sg.[Id] = @Id",
                        request
                    );
                    students = await _dbService.GetAll<Student>(
                        $@"
                            SELECT
                            st.[Id], [AlbumNumber], st.[GroupId] 
                            FROM [Student] st
                            INNER JOIN [Group] g ON g.[Id] = st.[GroupId]
                            LEFT JOIN [StudentSubgroup] ssg ON ssg.[StudentId] = st.[Id]
                            LEFT JOIN [Subgroup] sg ON sg.[Id] = ssg.[SubgroupId]
                            WHERE st.[GroupId] = @Id OR g.[ScheduleId] = @Id OR sg.[Id] = @Id
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
                            FROM [Student] st
                            INNER JOIN [Group] g ON g.[Id] = st.[GroupId]
                            INNER JOIN [Schedule] s ON s.[Id] = g.[ScheduleId]
                            LEFT JOIN [StudentSubgroup] ssg ON ssg.[StudentId] = st.[Id]
                            LEFT JOIN [Subgroup] sg ON sg.[Id] = ssg.[SubgroupId]
                            WHERE s.[UserId] = @FilteredId AND (st.[GroupId] = @Id OR g.[ScheduleId] = @Id OR sg.[Id] = @Id)
                            ",
                        request
                    );
                    students = await _dbService.GetAll<Student>(
                        $@"
                            SELECT 
                            st.[Id], [AlbumNumber], st.[GroupId] 
                            FROM [Student] st
                            INNER JOIN [Group] g ON g.[Id] = st.[GroupId]
                            INNER JOIN [Schedule] s ON s.[Id] = g.[ScheduleId]
                            LEFT JOIN [StudentSubgroup] ssg ON ssg.[StudentId] = st.[Id]
                            LEFT JOIN [Subgroup] sg ON sg.[Id] = ssg.[SubgroupId]
                            WHERE s.[UserId] = @FilteredId AND (st.[GroupId] = @Id OR g.[ScheduleId] = @Id OR sg.[Id] = @Id)
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
            if (students != null)
            {
                foreach (var student in students)
                {
                    GetGroupHandler getGroupHandler = new GetGroupHandler(_dbService);
                    GetGroupQuery getGroupQuery = new GetGroupQuery(
                        student.GroupId,
                        new Guid(),
                        "Admin"
                    );
                    ActionResult<Group?> group = await getGroupHandler.Handle(
                        getGroupQuery,
                        cancellationToken
                    );
                    student.Group = group.Value!;
                }
            }
            return (students, count);
        }
    }
}
