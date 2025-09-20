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
    public record GetScheduleByURLAndNameQuery(
        string URL,
        string Name,
        Guid FilteredId,
        string Role
    ) : IRequest<Schedule>;

    public class GetScheduleByURLAndNameHandler
        : IRequestHandler<GetScheduleByURLAndNameQuery, Schedule?>
    {
        private readonly IDbService _dbService;
        private readonly IStringLocalizer<Errors> _str;
        private readonly IStringLocalizer<Fields> _strFields;

        public GetScheduleByURLAndNameHandler(
            IDbService dbService,
            IStringLocalizer<Errors> str,
            IStringLocalizer<Fields> strFields
        )
        {
            _dbService = dbService;
            _str = str;
            _strFields = strFields;
        }

        public async Task<Schedule?> Handle(
            GetScheduleByURLAndNameQuery request,
            CancellationToken cancellationToken
        )
        {
            GetAllSchedulesByURLHandler getAllSchedulesByURLHandler =
                new GetAllSchedulesByURLHandler(_dbService, _str, _strFields);
            GetAllSchedulesByURLQuery getAllSchedulesByURLQuery = new GetAllSchedulesByURLQuery(
                request.URL,
                request.FilteredId,
                request.Role,
                new PaginationParams(int.MaxValue, 0, "Id", "asc")
            );
            var userSchedules = await getAllSchedulesByURLHandler.Handle(
                getAllSchedulesByURLQuery,
                cancellationToken
            );

            foreach (var item in userSchedules.Item1!)
            {
                if (item.Name == request.Name)
                {
                    return item;
                }
            }

            return null;
        }
    }
}
