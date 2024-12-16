using AlpimiAPI.Database;
using AlpimiAPI.Entities.ESchedule.DTO;
using AlpimiAPI.Entities.ESchedule.Queries;
using AlpimiAPI.Entities.EUser;
using AlpimiAPI.Entities.EUser.Queries;
using AlpimiAPI.Locales;
using AlpimiAPI.Responses;
using MediatR;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Localization;

namespace AlpimiAPI.Entities.ESchedule.Commands
{
    public record UpdateScheduleCommand(
        Guid Id,
        UpdateScheduleDTO dto,
        Guid FilteredId,
        string Role
    ) : IRequest<Schedule?>;

    public class UpdateScheduleHandler : IRequestHandler<UpdateScheduleCommand, Schedule?>
    {
        private readonly IDbService _dbService;
        private readonly IStringLocalizer<Errors> _str;

        public UpdateScheduleHandler(IDbService dbService, IStringLocalizer<Errors> str)
        {
            _dbService = dbService;
            _str = str;
        }

        public async Task<Schedule?> Handle(
            UpdateScheduleCommand request,
            CancellationToken cancellationToken
        )
        {
            GetScheduleHandler getScheduleHandler = new GetScheduleHandler(_dbService);
            GetScheduleQuery getScheduleQuery = new GetScheduleQuery(
                request.Id,
                request.FilteredId,
                request.Role
            );
            ActionResult<Schedule?> originalSchedule = await getScheduleHandler.Handle(
                getScheduleQuery,
                cancellationToken
            );
            if (originalSchedule.Value == null)
            {
                return null;
            }

            if (request.dto.Name != null)
            {
                GetScheduleByNameHandler getScheduleByNameHandler = new GetScheduleByNameHandler(
                    _dbService
                );
                GetScheduleByNameQuery getScheduleByNameQuery = new GetScheduleByNameQuery(
                    request.dto.Name,
                    request.FilteredId,
                    "User"
                );
                ActionResult<Schedule?> scheduleName = await getScheduleByNameHandler.Handle(
                    getScheduleByNameQuery,
                    cancellationToken
                );

                if (scheduleName.Value != null)
                {
                    if (scheduleName.Value.Id != request.Id)
                    {
                        throw new ApiErrorException(
                            [new ErrorObject(_str["alreadyExists", "Schedule", request.dto.Name])]
                        );
                    }
                }
            }

            request.dto.Name = request.dto.Name ?? originalSchedule.Value.Name;

            var schedule = await _dbService.Update<Schedule?>(
                $@"
                    UPDATE [Schedule] 
                    SET 
                    [Name] = @Name 
                    OUTPUT
                    INSERTED.[Id], 
                    INSERTED.[Name], 
                    INSERTED.[UserId]
                    WHERE [Id] = '{request.Id}'; ",
                request.dto
            );

            GetUserHandler getUserHandler = new GetUserHandler(_dbService);
            GetUserQuery getUserQuery = new GetUserQuery(schedule!.UserId, new Guid(), "Admin");
            ActionResult<User?> user = await getUserHandler.Handle(getUserQuery, cancellationToken);
            schedule.User = user.Value!;

            return schedule;
        }
    }
}
