using AlpimiAPI.Database;
using AlpimiAPI.Entities.ELessonType;
using AlpimiAPI.Entities.ELessonType.Queries;
using AlpimiAPI.Entities.ESubgroup;
using AlpimiAPI.Entities.ESubgroup.Queries;
using MediatR;
using Microsoft.AspNetCore.Mvc;

namespace AlpimiAPI.Entities.ELesson.Queries
{
    public record GetLessonQuery(Guid Id, Guid FilteredId, string Role) : IRequest<Lesson?>;

    public class GetLessonHandler : IRequestHandler<GetLessonQuery, Lesson?>
    {
        private readonly IDbService _dbService;

        public GetLessonHandler(IDbService dbService)
        {
            _dbService = dbService;
        }

        public async Task<Lesson?> Handle(
            GetLessonQuery request,
            CancellationToken cancellationToken
        )
        {
            Lesson? lesson;
            switch (request.Role)
            {
                case "Admin":
                    lesson = await _dbService.Get<Lesson?>(
                        @"
                            SELECT 
                            [Id], [Name], [CurrentHours], [AmountOfHours], [LessonTypeId], [SubgroupId] 
                            FROM [Lesson] 
                            WHERE [Id] = @Id;",
                        request
                    );
                    break;
                default:
                    lesson = await _dbService.Get<Lesson?>(
                        @"
                            SELECT 
                            l.[Id], l.[Name], [CurrentHours], [AmountOfHours], l.[LessonTypeId], l.[SubgroupId]  
                            FROM [Lesson] l
                            INNER JOIN [LessonType] lt ON lt.[Id] = l.[LessonTypeId]
                            INNER JOIN [Schedule] s ON lt.[ScheduleId] = s.[Id]
                            WHERE l.[Id] = @Id AND s.[UserId] = @FilteredId;",
                        request
                    );
                    break;
            }

            if (lesson != null)
            {
                GetLessonTypeHandler getLessonTypeHandler = new GetLessonTypeHandler(_dbService);
                GetLessonTypeQuery getLessonTypeQuery = new GetLessonTypeQuery(
                    lesson.LessonTypeId,
                    new Guid(),
                    "Admin"
                );
                ActionResult<LessonType?> lessonType = await getLessonTypeHandler.Handle(
                    getLessonTypeQuery,
                    cancellationToken
                );
                lesson.LessonType = lessonType.Value!;

                GetSubgroupHandler getSubgroupHandler = new GetSubgroupHandler(_dbService);
                GetSubgroupQuery getSubgroupQuery = new GetSubgroupQuery(
                    lesson.SubgroupId,
                    new Guid(),
                    "Admin"
                );
                ActionResult<Subgroup?> subgroup = await getSubgroupHandler.Handle(
                    getSubgroupQuery,
                    cancellationToken
                );
                lesson.Subgroup = subgroup.Value!;
            }

            return lesson;
        }
    }
}
