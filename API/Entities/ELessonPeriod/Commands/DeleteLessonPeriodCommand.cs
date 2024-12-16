using AlpimiAPI.Database;
using MediatR;

namespace AlpimiAPI.Entities.ELessonPeriod.Commands
{
    public record DeleteLessonPeriodCommand(Guid Id, Guid FilteredId, string Role) : IRequest;

    public class DeleteLessonPeriodHandler : IRequestHandler<DeleteLessonPeriodCommand>
    {
        private readonly IDbService _dbService;

        public DeleteLessonPeriodHandler(IDbService dbService)
        {
            _dbService = dbService;
        }

        public async Task Handle(
            DeleteLessonPeriodCommand request,
            CancellationToken cancellationToken
        )
        {
            switch (request.Role)
            {
                case "Admin":
                    await _dbService.Delete(
                        @"
                            DELETE [LessonPeriod] 
                            WHERE [Id] = @Id; ",
                        request
                    );
                    break;
                default:
                    await _dbService.Delete(
                        @"
                            DELETE lp
                            FROM [LessonPeriod] lp
                            INNER JOIN [ScheduleSettings] ss ON ss.[Id] = lp.[ScheduleSettingsId]
                            INNER JOIN [Schedule] s ON s.[Id] = ss.[ScheduleId]
                            WHERE s.[UserId] = @FilteredId AND lp.[Id] = @Id; ",
                        request
                    );
                    break;
            }
        }
    }
}
