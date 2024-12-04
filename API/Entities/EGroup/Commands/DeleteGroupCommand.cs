using AlpimiAPI.Database;
using MediatR;

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
