using AlpimiAPI.Entities.ESchedule.Queries;
using AlpimiAPI.Entities.EUser;
using AlpimiAPI.Entities.EUser.Queries;
using MediatR;
using Microsoft.AspNetCore.Mvc;

namespace AlpimiAPI.Entities.ESchedule.Commands
{
    public record UpdateScheduleCommand(
        Guid Id,
        string? Name,
        int? SchoolHour,
        Guid FilteredID,
        string Role
    ) : IRequest<Schedule?>;

    public class UpdateScheduleHandler : IRequestHandler<UpdateScheduleCommand, Schedule?>
    {
        private readonly IDbService _dbService;

        public UpdateScheduleHandler(IDbService dbService)
        {
            _dbService = dbService;
        }

        public async Task<Schedule?> Handle(
            UpdateScheduleCommand request,
            CancellationToken cancellationToken
        )
        {
            if (request.Name != null)
            {
                GetScheduleByNameHandler getScheduleByNameHandler = new GetScheduleByNameHandler(
                    _dbService
                );
                GetScheduleByNameQuery getScheduleByNameQuery = new GetScheduleByNameQuery(
                    request.Name,
                    new Guid(),
                    "Admin"
                );
                ActionResult<Schedule?> scheduleName = await getScheduleByNameHandler.Handle(
                    getScheduleByNameQuery,
                    cancellationToken
                );

                if (scheduleName.Value != null)
                {
                    throw new BadHttpRequestException("Name already taken");
                }
            }

            Schedule? schedule;
            switch (request.Role)
            {
                case "Admin":
                    schedule = await _dbService.Update<Schedule?>(
                        @"
                    UPDATE [Schedule] 
                    SET [Name]=CASE WHEN @Name IS NOT NULL THEN @Name 
                    ELSE [Name] END,[SchoolHour]=CASE WHEN @SchoolHour IS NOT NULL THEN @SchoolHour ELSE [SchoolHour] END 
                    OUTPUT INSERTED.[Id], INSERTED.[Name], INSERTED.[SchoolHour]
                    WHERE [Id]=@Id;",
                        request
                    );
                    break;
                default:
                    schedule = await _dbService.Update<Schedule?>(
                        @"
                     UPDATE [Schedule] 
                    SET [Name]=CASE WHEN @Name IS NOT NULL THEN @Name 
                    ELSE [Name] END,[SchoolHour]=CASE WHEN @SchoolHour IS NOT NULL THEN @SchoolHour ELSE [SchoolHour] END 
                    OUTPUT INSERTED.[Id], INSERTED.[Name], INSERTED.[SchoolHour]
                    WHERE [Id]=@Id and [UserID]=@FilteredID;",
                        request
                    );
                    break;
            }

            GetUserHandler getUserHandler = new GetUserHandler(_dbService);
            GetUserQuery getUserQuery = new GetUserQuery(request.FilteredID, new Guid(), "Admin");
            ActionResult<User?> user = await getUserHandler.Handle(getUserQuery, cancellationToken);
            if (schedule != null)
            {
                schedule.User = user.Value!;
                schedule.UserID = request.FilteredID;
            }
            return schedule;
        }
    }
}
