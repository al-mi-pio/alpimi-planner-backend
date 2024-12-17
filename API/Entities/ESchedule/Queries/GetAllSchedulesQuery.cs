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

    public class GetSchedulesHandler
        : IRequestHandler<GetAllSchedulesQuery, (IEnumerable<Schedule>?, int)>
    {
        private readonly IDbService _dbService;
        private readonly IStringLocalizer<Errors> _str;

        public GetSchedulesHandler(IDbService dbService, IStringLocalizer<Errors> str)
        {
            _dbService = dbService;
            _str = str;
        }

        public async Task<(IEnumerable<Schedule>?, int)> Handle(
            GetAllSchedulesQuery request,
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
            if (request.Pagination.SortBy != "Id" && request.Pagination.SortBy != "Name")
            {
                errors.Add(new ErrorObject(_str["badParameter", "SortBy"]));
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
                            from [Schedule];",
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
                            from [Schedule] 
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
                foreach (var schedule in schedules)
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
                    schedule.User = user.Value!;
                }
            }

            return (schedules, count);
        }
    }
}
