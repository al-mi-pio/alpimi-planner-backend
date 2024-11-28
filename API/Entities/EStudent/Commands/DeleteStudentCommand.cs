using AlpimiAPI.Database;
using MediatR;

namespace AlpimiAPI.Entities.EStudent.Commands
{
    public record DeleteStudentCommand(Guid Id, Guid FilteredId, string Role) : IRequest;

    public class DeleteStudentHandler : IRequestHandler<DeleteStudentCommand>
    {
        private readonly IDbService _dbService;

        public DeleteStudentHandler(IDbService dbService)
        {
            _dbService = dbService;
        }

        public async Task Handle(DeleteStudentCommand request, CancellationToken cancellationToken)
        {
            switch (request.Role)
            {
                case "Admin":
                    await _dbService.Delete(
                        @"
                            DELETE [Student] 
                            WHERE [Id] = @Id;",
                        request
                    );
                    break;
                default:
                    await _dbService.Delete(
                        @"
                            DELETE st
                            FROM [Student] st
                            INNER JOIN [Group] g on g.[Id] = st.[GroupId]
                            INNER JOIN [Schedule] s ON s.[Id] = g.[ScheduleId]
                            WHERE s.[UserId] = @FilteredId AND st.[Id] = @Id;",
                        request
                    );
                    break;
            }
        }
    }
}
