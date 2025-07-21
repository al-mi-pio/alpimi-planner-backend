using AlpimiAPI.Database;
using AlpimiAPI.Entities.EDayOff.Commands;
using AlpimiAPI.Entities.EGroup;
using AlpimiAPI.Entities.EGroup.Queries;
using AlpimiAPI.Entities.EHistory.DTO;
using AlpimiAPI.Entities.ESchedule;
using AlpimiAPI.Entities.ESubgroup.DTO;
using AlpimiAPI.Locales;
using AlpimiAPI.Responses;
using MediatR;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Localization;

namespace AlpimiAPI.Entities.ESubgroup.Commands
{
    public record CreateSubgroupCommand(
        Guid Id,
        CreateSubgroupDTO dto,
        Guid FilteredId,
        string Role
    ) : IRequest<Guid>;

    public class CreateSubgroupHandler : IRequestHandler<CreateSubgroupCommand, Guid>
    {
        private readonly IDbService _dbService;
        private readonly IStringLocalizer<Errors> _str;
        private readonly IStringLocalizer<Fields> _strFields;

        public CreateSubgroupHandler(
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
            CreateSubgroupCommand request,
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

            GetGroupHandler getGroupHandler = new GetGroupHandler(_dbService);
            GetGroupQuery getGroupQuery = new GetGroupQuery(
                request.dto.GroupId!.Value,
                request.FilteredId,
                request.Role
            );
            ActionResult<Group?> group = await getGroupHandler.Handle(
                getGroupQuery,
                cancellationToken
            );

            if (group.Value == null)
            {
                throw new ApiErrorException(
                    [
                        new ErrorObject(
                            _str["resourceNotFound", _strFields["Group"], request.dto.GroupId]
                        )
                    ]
                );
            }

            if (group.Value.StudentCount < request.dto.StudentCount)
            {
                throw new ApiErrorException([new ErrorObject(_str["tooManyStudents"])]);
            }

            var groupName = await _dbService.GetAll<Group>(
                $@"
                    SELECT 
                    [Id]
                    FROM [Group] 
                    WHERE [Name] = @Name AND [ScheduleId]='{group.Value.ScheduleId}';",
                request.dto
            );

            if (groupName!.Any())
            {
                throw new ApiErrorException(
                    [
                        new FieldErrorObject(
                            "name",
                            _str["alreadyExists", _strFields["Group"], request.dto.Name!]
                        )
                    ]
                );
            }

            var subgroupName = await _dbService.GetAll<Subgroup>(
                @"
                    SELECT 
                    [Id]
                    FROM [Subgroup] 
                    WHERE [Name] = @Name AND [GroupId] = @GroupId;",
                request.dto
            );

            if (subgroupName!.Any())
            {
                throw new ApiErrorException(
                    [
                        new FieldErrorObject(
                            "name",
                            _str["alreadyExists", _strFields["Subgroup"], request.dto.Name!]
                        )
                    ]
                );
            }

            var insertedId = await _dbService.Post<Guid>(
                $@"
                    INSERT INTO [Subgroup] 
                    ([Id], [Name], [StudentCount], [GroupId])
                    OUTPUT 
                    INSERTED.Id                    
                    VALUES (
                    '{request.Id}',   
                    @Name,
                    @StudentCount,
                    @GroupId);",
                request.dto
            );

            AddToHistoryHandler addToHistoryHandler = new AddToHistoryHandler(_dbService);
            AddToHistoryDTO addToHistoryDTO = new AddToHistoryDTO
            {
                Id = Guid.NewGuid(),
                Timestamp = DateTime.Now,
                AffectedEntityId = insertedId,
                AffectedEntity = "Subgroup",
                Command = "Create",
                ReversaleDTO = null,
                CollisionChecked = false,
                ScheduleId = group.Value.ScheduleId
            };
            AddToHistoryCommand addToHistoryCommand = new AddToHistoryCommand(addToHistoryDTO);
            await addToHistoryHandler.Handle(addToHistoryCommand, cancellationToken);

            return insertedId;
        }
    }
}
