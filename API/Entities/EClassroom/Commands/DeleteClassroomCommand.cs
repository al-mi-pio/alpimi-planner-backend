using AlpimiAPI.Database;
using MediatR;

namespace AlpimiAPI.Entities.EClassroom.Commands
{
    public record DeleteClassroomCommand(Guid Id, Guid FilteredId, string Role) : IRequest;

    public class DeleteClassroomHandler : IRequestHandler<DeleteClassroomCommand>
    {
        private readonly IDbService _dbService;

        public DeleteClassroomHandler(IDbService dbService)
        {
            _dbService = dbService;
        }

        public async Task Handle(
            DeleteClassroomCommand request,
            CancellationToken cancellationToken
        )
        {
            switch (request.Role)
            {
                case "Admin":
                    await _dbService.Delete(
                        @"
                            DELETE [Classroom] 
                            WHERE [Id] = @Id;",
                        request
                    );
                    break;
                default:
                    await _dbService.Delete(
                        @"
                            DELETE c
                            FROM [Classroom] c
                            INNER JOIN [Schedule] s ON s.[Id] = c.[ScheduleId]
                            WHERE s.[UserId] = @FilteredId AND c.[Id] = @Id;",
                        request
                    );
                    break;
            }
        }
    }
}
