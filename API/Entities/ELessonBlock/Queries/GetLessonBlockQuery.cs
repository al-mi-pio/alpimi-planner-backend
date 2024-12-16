using AlpimiAPI.Database;
using AlpimiAPI.Entities.EClassroom;
using AlpimiAPI.Entities.EClassroom.Queries;
using AlpimiAPI.Entities.ELesson;
using AlpimiAPI.Entities.ELesson.Queries;
using AlpimiAPI.Entities.ETeacher;
using AlpimiAPI.Entities.ETeacher.Queries;
using MediatR;
using Microsoft.AspNetCore.Mvc;

namespace AlpimiAPI.Entities.ELessonBlock.Queries
{
    public record GetLessonBlockQuery(Guid Id, Guid FilteredId, string Role)
        : IRequest<LessonBlock?>;

    public class GetLessonBlockHandler : IRequestHandler<GetLessonBlockQuery, LessonBlock?>
    {
        private readonly IDbService _dbService;

        public GetLessonBlockHandler(IDbService dbService)
        {
            _dbService = dbService;
        }

        public async Task<LessonBlock?> Handle(
            GetLessonBlockQuery request,
            CancellationToken cancellationToken
        )
        {
            LessonBlock? lessonBlock;
            switch (request.Role)
            {
                case "Admin":
                    lessonBlock = await _dbService.Get<LessonBlock?>(
                        @"
                            SELECT 
                            [Id], [LessonDate], [LessonStart], [LessonEnd], [LessonId], [ClassroomId], [TeacherId], [ClusterId] 
                            FROM [LessonBlock] 
                            WHERE [Id] = @Id; ",
                        request
                    );
                    break;
                default:
                    lessonBlock = await _dbService.Get<LessonBlock?>(
                        @"
                            SELECT 
                            lb.[Id], [LessonDate], [LessonStart], [LessonEnd], [LessonId], [ClassroomId], [TeacherId], [ClusterId]  
                            FROM [LessonBlock] lb
                            INNER JOIN [Lesson] l ON l.[Id] = lb.[LessonId]
                            INNER JOIN [LessonType] lt ON lt.[Id] = l.[LessonTypeId]
                            INNER JOIN [Schedule] s ON s.[Id] = lt.[ScheduleId]
                            WHERE lb.[Id] = @Id AND s.[UserId] = @FilteredId; ",
                        request
                    );
                    break;
            }

            if (lessonBlock != null)
            {
                GetLessonHandler getLessonHandler = new GetLessonHandler(_dbService);
                GetLessonQuery getLessonQuery = new GetLessonQuery(
                    lessonBlock.LessonId,
                    new Guid(),
                    "Admin"
                );
                ActionResult<Lesson?> lesson = await getLessonHandler.Handle(
                    getLessonQuery,
                    cancellationToken
                );
                lessonBlock.Lesson = lesson.Value!;

                if (lessonBlock.ClassroomId != null)
                {
                    GetClassroomHandler getClassroomHandler = new GetClassroomHandler(_dbService);
                    GetClassroomQuery getClassroomQuery = new GetClassroomQuery(
                        lessonBlock.ClassroomId.Value,
                        new Guid(),
                        "Admin"
                    );
                    ActionResult<Classroom?> classroom = await getClassroomHandler.Handle(
                        getClassroomQuery,
                        cancellationToken
                    );
                    lessonBlock.Classroom = classroom.Value!;
                }

                if (lessonBlock.TeacherId != null)
                {
                    GetTeacherHandler getTeacherHandler = new GetTeacherHandler(_dbService);
                    GetTeacherQuery getTeacherQuery = new GetTeacherQuery(
                        lessonBlock.TeacherId.Value,
                        new Guid(),
                        "Admin"
                    );
                    ActionResult<Teacher?> teacher = await getTeacherHandler.Handle(
                        getTeacherQuery,
                        cancellationToken
                    );
                    lessonBlock.Teacher = teacher.Value!;
                }
            }
            return lessonBlock;
        }
    }
}
