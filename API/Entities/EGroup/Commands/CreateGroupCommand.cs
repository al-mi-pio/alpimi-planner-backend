using AlpimiAPI.Database;
using AlpimiAPI.Entities.EDayOff.Commands;
using AlpimiAPI.Entities.EGroup.DTO;
using AlpimiAPI.Entities.EHistory.DTO;
using AlpimiAPI.Entities.ESchedule;
using AlpimiAPI.Entities.ESchedule.Queries;
using AlpimiAPI.Entities.ESubgroup;
using AlpimiAPI.Locales;
using AlpimiAPI.Responses;
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
        private readonly IStringLocalizer<Fields> _strFields;

        public CreateGroupHandler(
            IDbService dbService,
            IStringLocalizer<Errors> str,
            IStringLocalizer<Fields> strFields
        )
        {
            _dbService = dbService;
            _str = str;
            _strFields = strFields;
        }

        public async Task<Guid> Handle(
            CreateGroupCommand request,
            CancellationToken cancellationToken
        )
        {
            if (request.dto.StudentCount < 1)
            {
                throw new ApiErrorException(
                    [
                        new FieldErrorObject(
                            "StudentCount",
                            _str["badParameter", _strFields["StudentCount"]]
                        )
                    ]
                );
            }

            GetScheduleHandler getScheduleHandler = new GetScheduleHandler(_dbService);
            GetScheduleQuery getScheduleQuery = new GetScheduleQuery(
                request.dto.ScheduleId!.Value,
                request.FilteredId,
                request.Role
            );
            ActionResult<Schedule?> schedule = await getScheduleHandler.Handle(
                getScheduleQuery,
                cancellationToken
            );

            if (schedule.Value == null)
            {
                throw new ApiErrorException(
                    [new ErrorObject(_str["resourceNotFound", "Schedule", request.dto.ScheduleId])]
                );
            }

            var groupName = await _dbService.Get<Group>(
                @"
                    SELECT 
                    [Id]
                    FROM [Group] 
                    WHERE [Name] = @Name AND [ScheduleId] = @ScheduleId;",
                request.dto
            );

            if (groupName != null)
            {
                throw new ApiErrorException(
                    [new ErrorObject(_str["alreadyExists", "Group", request.dto.Name!])]
                );
            }

            var subgroupName = await _dbService.Get<Subgroup>(
                $@"
                    SELECT 
                    sg.[Id]
                    FROM [Subgroup] sg
                    INNER JOIN [Group] g ON g.[Id] = sg.[GroupId]
                    WHERE sg.[Name] = @Name AND g.[ScheduleId] = @ScheduleId;",
                request.dto
            );

            if (subgroupName != null)
            {
                throw new ApiErrorException(
                    [new ErrorObject(_str["alreadyExists", "Subgroup", request.dto.Name!])]
                );
            }

            var insertedId = await _dbService.Post<Guid>(
                $@"
                    INSERT INTO [Group] 
                    ([Id], [Name], [StudentCount], [ScheduleId])
                    OUTPUT 
                    INSERTED.Id                    
                    VALUES (
                    '{request.Id}',   
                    @Name,
                    @StudentCount,
                    @ScheduleId);",
                request.dto
            );

            AddToHistoryHandler addToHistoryHandler = new AddToHistoryHandler(_dbService);
            AddToHistoryDTO addToHistoryDTO = new AddToHistoryDTO
            {
                Id = Guid.NewGuid(),
                Timestamp = DateTime.Now,
                AffectedEntityId = insertedId,
                AffectedEntity = "Group",
                Command = "Create",
                ReversaleDTO = null,
                CollisionChecked = false,
                ScheduleId = schedule.Value.Id
            };
            AddToHistoryCommand addToHistoryCommand = new AddToHistoryCommand(addToHistoryDTO);
            await addToHistoryHandler.Handle(addToHistoryCommand, cancellationToken);

            return insertedId;
        }
    }
}
