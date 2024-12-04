using AlpimiAPI.Database;
using MediatR;

namespace AlpimiAPI.Entities.EClassroomType.Commands
{
    public record DeleteClassroomTypeCommand(Guid Id, Guid FilteredId, string Role) : IRequest;

    public class DeleteClassroomTypeHandler : IRequestHandler<DeleteClassroomTypeCommand>
    {
        private readonly IDbService _dbService;

        public DeleteClassroomTypeHandler(IDbService dbService)
        {
            _dbService = dbService;
        }

        public async Task Handle(
            DeleteClassroomTypeCommand request,
            CancellationToken cancellationToken
        )
        {
            switch (request.Role)
            {
                case "Admin":
                    await _dbService.Delete(
                        @"
                            DELETE [ClassroomType] 
                            WHERE [Id] = @Id;",
                        request
                    );
                    break;
                default:
                    await _dbService.Delete(
                        @"
                            DELETE ct
                            FROM [ClassroomType] ct
                            INNER JOIN [Schedule] s ON s.[Id] = ct.[ScheduleId]
                            WHERE s.[UserId] = @FilteredId AND ct.[Id] = @Id;",
                        request
                    );
                    break;
            }
        }
    }
}
