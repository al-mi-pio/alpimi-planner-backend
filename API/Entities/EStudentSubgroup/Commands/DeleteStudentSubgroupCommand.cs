using AlpimiAPI.Database;
using MediatR;

namespace AlpimiAPI.Entities.EStudentSubgroup.Commands
{
    public record DeleteStudentSubgroupCommand(
        Guid StudentId,
        Guid SubgroupId,
        Guid FilteredId,
        string Role
    ) : IRequest;

    public class DeleteStudentSubgroupHandler : IRequestHandler<DeleteStudentSubgroupCommand>
    {
        private readonly IDbService _dbService;

        public DeleteStudentSubgroupHandler(IDbService dbService)
        {
            _dbService = dbService;
        }

        public async Task Handle(
            DeleteStudentSubgroupCommand request,
            CancellationToken cancellationToken
        )
        {
            switch (request.Role)
            {
                case "Admin":
                    await _dbService.Delete(
                        @"
                            DELETE [StudentSubgroup] 
                            WHERE [StudentId] = @StudentId AND [SubgroupId] = @SubgroupId;",
                        request
                    );
                    break;
                default:
                    await _dbService.Delete(
                        @"
                            DELETE ssg
                            FROM [StudentSubgroup] ssg
                            INNER JOIN [Subgroup] sg ON sg.[Id] = ssg.[SubgroupId]
                            INNER JOIN [Group] g ON g.[Id] = sg.[GroupId]
                            INNER JOIN [Schedule] s ON s.[Id] = g.[ScheduleId]
                            WHERE s.[UserId] = @FilteredId AND ssg.[StudentId] = @StudentId AND ssg.[SubgroupId] = @SubgroupId;",
                        request
                    );
                    break;
            }
        }
    }
}
