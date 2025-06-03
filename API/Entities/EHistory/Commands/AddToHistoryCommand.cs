using AlpimiAPI.Database;
using AlpimiAPI.Entities.EHistory.DTO;
using AlpimiAPI.Utilities;
using MediatR;

namespace AlpimiAPI.Entities.EDayOff.Commands
{
    public record AddToHistoryCommand(AddToHistoryDTO dto) : IRequest<Guid>;

    public class AddToHistoryHandler : IRequestHandler<AddToHistoryCommand, Guid>
    {
        private readonly IDbService _dbService;

        public AddToHistoryHandler(IDbService dbService)
        {
            _dbService = dbService;
        }

        public async Task<Guid> Handle(
            AddToHistoryCommand request,
            CancellationToken cancellationToken
        )
        {
            var insertedId = await _dbService.Post<Guid>(
                $@"
                    INSERT INTO [History] 
                    ([Id], [Timestamp], [AffectedEntityId], [AffectedEntity], [Command], [ReversaleDTO], [IsUndone], [CollisionChecked], [ScheduleId])
                    OUTPUT 
                    INSERTED.Id                    
                    VALUES (
                    @Id,
                    @Timestamp,
                    @AffectedEntityId,
                    @AffectedEntity,
                    @Command,
                    @ReversaleDTO,
                    0,
                    @CollisionChecked,
                    @ScheduleId);",
                request.dto
            );

            await _dbService.Delete(
                $@"
                    DELETE 
                    FROM [History]
                    WHERE [Id] = (
                    SELECT [Id] 
                    FROM [History]
                    ORDER BY [Timestamp] DESC
                    OFFSET {Configuration.GetHistoryLimit()} ROWS
                    FETCH NEXT 1 ROWS ONLY);",
                request
            );

            return insertedId;
        }
    }
}
