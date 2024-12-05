using AlpimiAPI.Database;
using AlpimiAPI.Entities.ESchedule;
using AlpimiAPI.Entities.ESchedule.Queries;
using MediatR;
using Microsoft.AspNetCore.Mvc;

namespace AlpimiAPI.Entities.ELessonType.Queries
{
    public record GetLessonTypeQuery(Guid Id, Guid FilteredId, string Role) : IRequest<LessonType?>;

    public class GetLessonTypeHandler : IRequestHandler<GetLessonTypeQuery, LessonType?>
    {
        private readonly IDbService _dbService;

        public GetLessonTypeHandler(IDbService dbService)
        {
            _dbService = dbService;
        }

        public async Task<LessonType?> Handle(
            GetLessonTypeQuery request,
            CancellationToken cancellationToken
        )
        {
            LessonType? lessonType;
            switch (request.Role)
            {
                case "Admin":
                    lessonType = await _dbService.Get<LessonType?>(
                        @"
                            SELECT 
                            [Id], [Name], [Color], [ScheduleId] 
                            FROM [LessonType] 
                            WHERE [Id] = @Id;",
                        request
                    );
                    break;
                default:
                    lessonType = await _dbService.Get<LessonType?>(
                        @"
                            SELECT 
                            lt.[Id], lt.[Name], lt.[Color], lt.[ScheduleId] 
                            FROM [LessonType] lt
                            INNER JOIN [Schedule] s ON lt.[ScheduleId] = s.[Id]
                            WHERE lt.[Id] = @Id AND s.[UserId] = @FilteredId;",
                        request
                    );
                    break;
            }

            if (lessonType != null)
            {
                GetScheduleHandler getScheduleHandler = new GetScheduleHandler(_dbService);
                GetScheduleQuery getScheduleQuery = new GetScheduleQuery(
                    lessonType.ScheduleId,
                    new Guid(),
                    "Admin"
                );
                ActionResult<Schedule?> schedule = await getScheduleHandler.Handle(
                    getScheduleQuery,
                    cancellationToken
                );
                lessonType.Schedule = schedule.Value!;
            }
            return lessonType;
        }
    }
}
