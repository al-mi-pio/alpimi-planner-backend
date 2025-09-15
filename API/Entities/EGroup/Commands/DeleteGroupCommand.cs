using System.Text.Json;
using AlpimiAPI.Database;
using AlpimiAPI.Entities.EDayOff.Commands;
using AlpimiAPI.Entities.EGroup.DTO;
using AlpimiAPI.Entities.EGroup.Queries;
using AlpimiAPI.Entities.EHistory.DTO;
using MediatR;
using Microsoft.AspNetCore.Mvc;

namespace AlpimiAPI.Entities.EGroup.Commands
{
    public record DeleteGroupCommand(Guid Id, Guid FilteredId, string Role) : IRequest;

    public class DeleteGroupHandler : IRequestHandler<DeleteGroupCommand>
    {
        private readonly IDbService _dbService;

        public DeleteGroupHandler(IDbService dbService)
        {
            _dbService = dbService;
        }

        public async Task Handle(DeleteGroupCommand request, CancellationToken cancellationToken)
        {
            GetGroupHandler getGroupHandler = new GetGroupHandler(_dbService);
            GetGroupQuery getGroupQuery = new GetGroupQuery(
                request.Id,
                request.FilteredId,
                request.Role
            );
            ActionResult<Group?> group = await getGroupHandler.Handle(
                getGroupQuery,
                cancellationToken
            );
            if (group.Value != null)
            {
                CreateGroupDTO reversaleDTO = new CreateGroupDTO
                {
                    Name = group.Value.Name,
                    StudentCount = group.Value.StudentCount,
                    ScheduleId = group.Value.ScheduleId,
                };
                AddToHistoryHandler addToHistoryHandler = new AddToHistoryHandler(_dbService);
                AddToHistoryDTO addToHistoryDTO = new AddToHistoryDTO
                {
                    Id = Guid.NewGuid(),
                    Timestamp = DateTime.Now,
                    AffectedEntityId = request.Id,
                    AffectedEntity = "Group",
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
                            DELETE [Group] 
                            WHERE [Id] = @Id;",
                        request
                    );
                    break;
                default:
                    await _dbService.Delete(
                        @"
                            DELETE g
                            FROM [Group] g
                            INNER JOIN [Schedule] s ON s.[Id] = g.[ScheduleId]
                            WHERE s.[UserId] = @FilteredId AND g.[Id] = @Id;",
                        request
                    );
                    break;
            }
        }
    }
}
