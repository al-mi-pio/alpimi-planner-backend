using AlpimiAPI.Database;
using MediatR;

namespace AlpimiAPI.Entities.ETeacher.Commands
{
    public record DeleteTeacherCommand(Guid Id, Guid FilteredId, string Role) : IRequest;

    public class DeleteTeacherHandler : IRequestHandler<DeleteTeacherCommand>
    {
        private readonly IDbService _dbService;

        public DeleteTeacherHandler(IDbService dbService)
        {
            _dbService = dbService;
        }

        public async Task Handle(DeleteTeacherCommand request, CancellationToken cancellationToken)
        {
            switch (request.Role)
            {
                case "Admin":
                    await _dbService.Delete(
                        @"
                            DELETE [Teacher] 
                            WHERE [Id] = @Id;",
                        request
                    );
                    break;
                default:
                    await _dbService.Delete(
                        @"
                            DELETE t
                            FROM [Teacher] t
                            INNER JOIN [Schedule] s ON s.[Id] = t.[ScheduleId]
                            WHERE s.[UserId] = @FilteredId AND t.[Id] = @Id;",
                        request
                    );
                    break;
            }
        }
    }
}
