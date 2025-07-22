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
    public record GetAllSchedulesQuery(Guid FilteredId, string Role, PaginationParams Pagination)
        : IRequest<(IEnumerable<Schedule>?, int)>;

    public class GetAllSchedulesHandler
        : IRequestHandler<GetAllSchedulesQuery, (IEnumerable<Schedule>?, int)>
    {
        private readonly IDbService _dbService;
        private readonly IStringLocalizer<Errors> _str;
        private readonly IStringLocalizer<Fields> _strFields;

        public GetAllSchedulesHandler(
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
            GetAllSchedulesQuery request,
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
                            FROM [Schedule];",
                        ""
                    );
                    schedules = await _dbService.GetAll<Schedule>(
                        $@"
                            SELECT 
                            [Id], [Name], [UserId]
                            FROM [Schedule] 
                            ORDER BY 
                            {request.Pagination.SortBy}
                            {request.Pagination.SortOrder}
                            OFFSET
                            {request.Pagination.Offset} ROWS
                            FETCH NEXT
                            {request.Pagination.PerPage} ROWS ONLY;",
                        request.Pagination
                    );
                    break;
                default:
                    count = await _dbService.Get<int>(
                        @"
                            SELECT 
                            COUNT(*) 
                            FROM [Schedule] 
                            WHERE [UserId] = @FilteredId;",
                        request
                    );
                    schedules = await _dbService.GetAll<Schedule>(
                        $@"
                            SELECT 
                            [Id], [Name], [UserId] 
                            FROM [Schedule]
                            WHERE [UserId] = @FilteredId
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
