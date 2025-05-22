using AlpimiAPI.Database;
using MediatR;

namespace AlpimiAPI.Entities.ESubgroup.Commands
{
    public record DeleteSubgroupCommand(Guid Id, Guid FilteredId, string Role) : IRequest;

    public class DeleteSubgroupHandler : IRequestHandler<DeleteSubgroupCommand>
    {
        private readonly IDbService _dbService;

        public DeleteSubgroupHandler(IDbService dbService)
        {
            _dbService = dbService;
        }

        public async Task Handle(DeleteSubgroupCommand request, CancellationToken cancellationToken)
        {
            switch (request.Role)
            {
                case "Admin":
                    await _dbService.Delete(
                        @"
                            DELETE FROM [LessonSubgroup]
                            WHERE [SubgroupId] = @Id;",
                        request
                    );
                    await _dbService.Delete(
                        @"
                            DELETE FROM [StudentSubgroup]
                            WHERE [SubgroupId] = @Id;",
                        request
                    );
                    await _dbService.Delete(
                        @"
                            DELETE [Subgroup] 
                            WHERE [Id] = @Id;",
                        request
                    );
                    break;
                default:
                    await _dbService.Delete(
                        @"
                            DELETE lsg
                            FROM [LessonSubgroup] lsg
                            INNER JOIN [LessonType] lt on lt.[Id] = lsg.[LessonTypeId]
                            INNER JOIN [Schedule] s ON s.[Id] = lt.[ScheduleId]
                            WHERE s.[UserId] = @FilteredId AND lsg.[SubgroupId] = @Id;",
                        request
                    );
                    await _dbService.Delete(
                        @"
                            DELETE ssg
                            FROM [StudentSubgroup] ssg
                            INNER JOIN [Subgroup] sg ON sg.[Id] = ssg.[SubgroupId]
                            INNER JOIN [Group] g ON g.[Id] = sg.[GroupId]
                            INNER JOIN [Schedule] s ON s.[Id] = g.[ScheduleId]
                            WHERE s.[UserId] = @FilteredId AND ssg.[SubgroupId] = @Id;",
                        request
                    );
                    await _dbService.Delete(
                        @"
                            DELETE sg
                            FROM [Subgroup] sg
                            INNER JOIN [Group] g ON g.[Id] = sg.[GroupId]
                            INNER JOIN [Schedule] s ON s.[Id] = g.[ScheduleId]
                            WHERE s.[UserId] = @FilteredId AND sg.[Id] = @Id;",
                        request
                    );
                    break;
            }
        }
    }
}
