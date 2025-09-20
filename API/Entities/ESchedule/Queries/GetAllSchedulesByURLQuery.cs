using AlpimiAPI.Database;
using AlpimiAPI.Entities.EUser;
using AlpimiAPI.Entities.EUser.Queries;
using AlpimiAPI.Locales;
using AlpimiAPI.Responses;
using MediatR;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Localization;

namespace AlpimiAPI.Entities.ESchedule.Queries
{
    public record GetAllSchedulesByURLQuery(
        string URL,
        Guid FilteredId,
        string Role,
        PaginationParams Pagination
    ) : IRequest<(IEnumerable<Schedule>?, int)>;

    public class GetAllSchedulesByURLHandler
        : IRequestHandler<GetAllSchedulesByURLQuery, (IEnumerable<Schedule>?, int)>
    {
        private readonly IDbService _dbService;
        private readonly IStringLocalizer<Errors> _str;
        private readonly IStringLocalizer<Fields> _strFields;

        public GetAllSchedulesByURLHandler(
            IDbService dbService,
            IStringLocalizer<Errors> str,
            IStringLocalizer<Fields> strFields
        )
        {
            _dbService = dbService;
            _str = str;
            _strFields = strFields;
        }

        public async Task<(IEnumerable<Schedule>?, int)> Handle(
            GetAllSchedulesByURLQuery request,
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
            if (request.Pagination.SortBy != "Id" && request.Pagination.SortBy != "Name")
            {
                errors.Add(
                    new FieldErrorObject("sortBy", _str["badParameter", _strFields["SortBy"]])
                );
            }

            if (errors.Count != 0)
            {
                throw new ApiErrorException(errors);
            }

            IEnumerable<Schedule>? schedules;
            int count;
            switch (request.Role)
            {
                case "Admin":
                    count = await _dbService.Get<int>(
                        @"
                            SELECT 
                            COUNT(*) 
                            FROM [Schedule] s
                            INNER JOIN [User] u ON u.[Id] = s.[UserId]
                            WHERE u.[CustomURL] = @URL;",
                        request
                    );
                    schedules = await _dbService.GetAll<Schedule>(
                        $@"
                            SELECT 
                            s.[Id], [Name], [UserId], [ModifyDate]
                            FROM [Schedule] s
                            INNER JOIN [User] u ON u.[Id] = s.[UserId]
                            WHERE u.[CustomURL] = @URL
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
                            FROM [Schedule] s
                            INNER JOIN [User] u ON u.[Id] = s.[UserId]
                            INNER JOIN [ScheduleSettings] ss ON ss.[ScheduleId] = s.[Id]
                            WHERE (s.[UserId] = @FilteredId OR ss.[IsPublic] = 'TRUE') AND u.[CustomURL] = @URL;",
                        request
                    );
                    schedules = await _dbService.GetAll<Schedule>(
                        $@"
                            SELECT 
                            s.[Id], [Name], [UserId], [ModifyDate] 
                            FROM [Schedule] s
                            INNER JOIN [User] u ON u.[Id] = s.[UserId]
                            INNER JOIN [ScheduleSettings] ss ON ss.[ScheduleId] = s.[Id]
                            WHERE (s.[UserId] = @FilteredId OR ss.[IsPublic] = 'TRUE') AND u.[CustomURL] = @URL
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
                            FROM [Schedule] s
                            INNER JOIN [User] u ON u.[Id] = s.[UserId]
                            INNER JOIN [ScheduleSettings] ss ON ss.[ScheduleId] = s.[Id]
                            WHERE u.[CustomURL] = @URL AND ss.[IsPublic] = 'TRUE';",
                        request
                    );
                    schedules = await _dbService.GetAll<Schedule>(
                        $@"
                            SELECT 
                            s.[Id], [Name], [UserId], [ModifyDate] 
                            FROM [Schedule] s
                            INNER JOIN [User] u ON u.[Id] = s.[UserId]
                            INNER JOIN [ScheduleSettings] ss ON ss.[ScheduleId] = s.[Id]
                            WHERE u.[CustomURL] = @URL AND ss.[IsPublic] = 'TRUE'
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

            if (schedules != null)
            {
                Dictionary<Guid, User> userMap = new Dictionary<Guid, User>();
                foreach (var schedule in schedules)
                {
                    if (!userMap.ContainsKey(schedule.UserId))
                    {
                        GetUserHandler getUserHandler = new GetUserHandler(_dbService);
                        GetUserQuery getUserQuery = new GetUserQuery(
                            schedule.UserId,
                            new Guid(),
                            "Admin"
                        );
                        ActionResult<User?> user = await getUserHandler.Handle(
                            getUserQuery,
                            cancellationToken
                        );

                        userMap.Add(schedule.UserId, user.Value!);
                    }
                    schedule.User = userMap[schedule.UserId];
                }
            }

            return (schedules, count);
        }
    }
}
