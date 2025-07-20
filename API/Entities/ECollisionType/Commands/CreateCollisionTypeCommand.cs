using AlpimiAPI.Database;
using AlpimiAPI.Entities.ECollisionType.DTO;
using AlpimiAPI.Entities.EDayOff.Commands;
using AlpimiAPI.Entities.EHistory.DTO;
using AlpimiAPI.Entities.ESchedule;
using AlpimiAPI.Entities.ESchedule.Queries;
using AlpimiAPI.Locales;
using AlpimiAPI.Responses;
using MediatR;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Localization;

namespace AlpimiAPI.Entities.ECollisionType.Commands
{
    public record CreateCollisionTypeCommand(
        Guid Id,
        CreateCollisionTypeDTO dto,
        Guid FilteredId,
        string Role
    ) : IRequest<Guid>;

    public class CreateCollisionTypeHandler : IRequestHandler<CreateCollisionTypeCommand, Guid>
    {
        private readonly IDbService _dbService;
        private readonly IStringLocalizer<Errors> _str;

        public CreateCollisionTypeHandler(IDbService dbService, IStringLocalizer<Errors> str)
        {
            _dbService = dbService;
            _str = str;
        }

        public async Task<Guid> Handle(
            CreateCollisionTypeCommand request,
            CancellationToken cancellationToken
        )
        {
            if (request.dto.Weight > 1 || request.dto.Weight < 0)
            {
                throw new ApiErrorException([new ErrorObject(_str["badParameter", "Weight"])]);
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

            var collisionTypeName = await _dbService.Get<CollisionType>(
                @"
                    SELECT 
                    [Id]
                    FROM [CollisionType] 
                    WHERE [Name] = @Name AND [ScheduleId] = @ScheduleId;",
                request.dto
            );

            if (collisionTypeName != null)
            {
                throw new ApiErrorException(
                    [new ErrorObject(_str["alreadyExists", "CollisionType", request.dto.Name])]
                );
            }

            var insertedId = await _dbService.Post<Guid>(
                $@"
                    INSERT INTO [CollisionType] 
                    ([Id], [Name], [Description], [Weight], [Filter], [Category], [ScheduleId])
                    OUTPUT 
                    INSERTED.Id                    
                    VALUES (
                    '{request.Id}',   
                    @Name,
                    @Description,
                    @Weight,
                    @Filter,
                    @Category,
                    @ScheduleId);",
                request.dto
            );

            AddToHistoryHandler addToHistoryHandler = new AddToHistoryHandler(_dbService);
            AddToHistoryDTO addToHistoryDTO = new AddToHistoryDTO
            {
                Id = Guid.NewGuid(),
                Timestamp = DateTime.Now,
                AffectedEntityId = insertedId,
                AffectedEntity = "CollisionType",
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
