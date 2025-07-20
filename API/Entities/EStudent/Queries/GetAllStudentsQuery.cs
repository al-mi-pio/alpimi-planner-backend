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
        private readonly IStringLocalizer<Fields> _strFields;

        public GetAllStudentsHandler(
            IDbService dbService,
            IStringLocalizer<Errors> str,
            IStringLocalizer<Fields> strFields
        )
        {
            _dbService = dbService;
            _str = str;
            _strFields = strFields;
        }

        public async Task<(IEnumerable<Student>?, int)> Handle(
            GetAllStudentsQuery request,
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
            if (request.Pagination.SortBy != "Id" && request.Pagination.SortBy != "AlbumNumber")
            {
                errors.Add(
                    new FieldErrorObject("SortBy", _str["badParameter", _strFields["SortBy"]])
                );
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
                            WHERE st.[GroupId] = @Id OR g.[ScheduleId] = @Id OR sg.[Id] = @Id;",
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
                            {request.Pagination.PerPage} ROWS ONLY;",
                        request
                    );
                    break;
                default:
                    count = await _dbService.Get<int>(
                        @"
                            SELECT
                            COUNT(*)
                            FROM [Student] st
                            INNER JOIN [Group] g ON g.[Id] = st.[GroupId]
                            INNER JOIN [Schedule] s ON s.[Id] = g.[ScheduleId]
                            LEFT JOIN [StudentSubgroup] ssg ON ssg.[StudentId] = st.[Id]
                            LEFT JOIN [Subgroup] sg ON sg.[Id] = ssg.[SubgroupId]
                            WHERE s.[UserId] = @FilteredId AND (st.[GroupId] = @Id OR g.[ScheduleId] = @Id OR sg.[Id] = @Id);",
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
                            {request.Pagination.PerPage} ROWS ONLY;",
                        request
                    );
                    break;
            }

            if (students != null)
            {
                Dictionary<Guid, Group> groupMap = new Dictionary<Guid, Group>();
                foreach (var student in students)
                {
                    if (!groupMap.ContainsKey(student.GroupId))
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

                        groupMap.Add(student.GroupId, group.Value!);
                    }
                    student.Group = groupMap[student.GroupId];
                }
            }

            return (students, count);
        }
    }
}
