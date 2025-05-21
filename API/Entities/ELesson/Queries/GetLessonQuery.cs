using AlpimiAPI.Database;
using AlpimiAPI.Entities.ELessonType;
using AlpimiAPI.Entities.ELessonType.Queries;
using AlpimiAPI.Entities.ETeacher;
using AlpimiAPI.Entities.ETeacher.Queries;
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
                            [Id], [Name], [CurrentHours], [AmountOfHours], [LessonTypeId], [TeacherId] 
                            FROM [Lesson] 
                            WHERE [Id] = @Id;",
                        request
                    );
                    break;
                default:
                    lesson = await _dbService.Get<Lesson?>(
                        @"
                            SELECT 
                            l.[Id], l.[Name], [CurrentHours], [AmountOfHours], l.[LessonTypeId], l.[TeacherId]  
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

                GetTeacherHandler getTeacherHandler = new GetTeacherHandler(_dbService);
                GetTeacherQuery getTeacherQuery = new GetTeacherQuery(
                    lesson.TeacherId,
                    new Guid(),
                    "Admin"
                );
                ActionResult<Teacher?> subgroup = await getTeacherHandler.Handle(
                    getTeacherQuery,
                    cancellationToken
                );
                lesson.Teacher = subgroup.Value!;
            }

            return lesson;
        }
    }
}
