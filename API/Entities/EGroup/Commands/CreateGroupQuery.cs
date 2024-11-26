using AlpimiAPI.Database;
using AlpimiAPI.Entities.EGroup.DTO;
using AlpimiAPI.Entities.ESchedule;
using AlpimiAPI.Entities.ESchedule.Queries;
using AlpimiAPI.Responses;
using alpimi_planner_backend.API.Locales;
using MediatR;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Localization;

namespace AlpimiAPI.Entities.EGroup.Commands
{
    public record CreateGroupCommand(Guid Id, CreateGroupDTO dto, Guid FilteredId, string Role)
        : IRequest<Guid>;

    public class CreateGroupHandler : IRequestHandler<CreateGroupCommand, Guid>
    {
        private readonly IDbService _dbService;
        private readonly IStringLocalizer<Errors> _str;

        public CreateGroupHandler(IDbService dbService, IStringLocalizer<Errors> str)
        {
            _dbService = dbService;
            _str = str;
        }

        public async Task<Guid> Handle(
            CreateGroupCommand request,
            CancellationToken cancellationToken
        )
        {
            if (request.dto.StudentCount < 1)
            {
                throw new ApiErrorException(
                    [new ErrorObject(_str["badParameter", "StudentCount"])]
                );
            }

            GetScheduleHandler getScheduleHandler = new GetScheduleHandler(_dbService);
            GetScheduleQuery getScheduleQuery = new GetScheduleQuery(
                request.dto.ScheduleId,
                request.FilteredId,
                request.Role
            );

            ActionResult<Schedule?> schedule = await getScheduleHandler.Handle(
                getScheduleQuery,
                cancellationToken
            );
            if (schedule.Value == null)
            {
                throw new ApiErrorException([new ErrorObject(_str["notFound", "Schedule"])]);
            }

            var groupName = await _dbService.GetAll<Guid>(
                @"
                    SELECT 
                    [Id]
                    FROM [Group] 
                    WHERE [Name] = @Name AND [ScheduleId] = @ScheduleId;",
                request.dto
            );

            if (groupName!.Any())
            {
                throw new ApiErrorException(
                    [new ErrorObject(_str["alreadyExists", "Group", request.dto.Name])]
                );
            }

            var insertedId = await _dbService.Post<Guid>(
                $@"
                    INSERT INTO [Group] 
                    ([Id],[Name],[StudentCount],[ScheduleId])
                    OUTPUT 
                    INSERTED.Id                    
                    VALUES (
                    '{request.Id}',   
                    @Name,
                    @StudentCount,
                    @ScheduleId);",
                request.dto
            );

            return insertedId;
        }
    }
}
