using System.Text.Json;
using AlpimiAPI.Database;
using AlpimiAPI.Entities.ECollisionType.DTO;
using AlpimiAPI.Entities.ECollisionType.Queries;
using AlpimiAPI.Entities.EDayOff.Commands;
using AlpimiAPI.Entities.EHistory.DTO;
using MediatR;
using Microsoft.AspNetCore.Mvc;

namespace AlpimiAPI.Entities.ECollisionType.Commands
{
    public record DeleteCollisionTypeCommand(Guid Id, Guid FilteredId, string Role) : IRequest;

    public class DeleteCollisionTypeHandler : IRequestHandler<DeleteCollisionTypeCommand>
    {
        private readonly IDbService _dbService;

        public DeleteCollisionTypeHandler(IDbService dbService)
        {
            _dbService = dbService;
        }

        public async Task Handle(
            DeleteCollisionTypeCommand request,
            CancellationToken cancellationToken
        )
        {
            GetCollisionTypeHandler getCollisionTypeHandler = new GetCollisionTypeHandler(
                _dbService
            );
            GetCollisionTypeQuery getCollisionTypeQuery = new GetCollisionTypeQuery(
                request.Id,
                request.FilteredId,
                request.Role
            );
            ActionResult<CollisionType?> collisiontype = await getCollisionTypeHandler.Handle(
                getCollisionTypeQuery,
                cancellationToken
            );
            if (collisiontype.Value != null)
            {
                CreateCollisionTypeDTO reversaleDTO = new CreateCollisionTypeDTO
                {
                    Name = collisiontype.Value.Name,
                    Description = collisiontype.Value.Description,
                    Weight = collisiontype.Value.Weight,
                    Filter = collisiontype.Value.Filter,
                    Category = collisiontype.Value.Category,
                    ScheduleId = collisiontype.Value.ScheduleId,
                };
                AddToHistoryHandler addToHistoryHandler = new AddToHistoryHandler(_dbService);
                AddToHistoryDTO addToHistoryDTO = new AddToHistoryDTO
                {
                    Id = Guid.NewGuid(),
                    Timestamp = DateTime.Now,
                    AffectedEntityId = request.Id,
                    AffectedEntity = "CollisionType",
                    Command = "Delete",
                    ReversaleDTO = JsonSerializer.Serialize(reversaleDTO),
                    CollisionChecked = false,
                    ScheduleId = reversaleDTO.ScheduleId!.Value,
                };
                AddToHistoryCommand addToHistoryCommand = new AddToHistoryCommand(addToHistoryDTO);
                await addToHistoryHandler.Handle(addToHistoryCommand, cancellationToken);
            }
            switch (request.Role)
            {
                case "Admin":
                    await _dbService.Delete(
                        @"
                            DELETE [CollisionType] 
                            WHERE [Id] = @Id;",
                        request
                    );
                    break;
                default:
                    await _dbService.Delete(
                        @"
                            DELETE ct
                            FROM [CollisionType] ct
                            INNER JOIN [Schedule] s ON s.[Id] = ct.[ScheduleId]
                            WHERE s.[UserId] = @FilteredId AND ct.[Id] = @Id;",
                        request
                    );
                    break;
            }
        }
    }
}
