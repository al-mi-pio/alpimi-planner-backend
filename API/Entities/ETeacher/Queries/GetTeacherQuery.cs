using AlpimiAPI.Database;
using AlpimiAPI.Entities.ESchedule;
using AlpimiAPI.Entities.ESchedule.Queries;
using MediatR;
using Microsoft.AspNetCore.Mvc;

namespace AlpimiAPI.Entities.ETeacher.Queries
{
    public record GetTeacherQuery(Guid Id, Guid FilteredId, string Role) : IRequest<Teacher?>;

    public class GetTeacherHandler : IRequestHandler<GetTeacherQuery, Teacher?>
    {
        private readonly IDbService _dbService;

        public GetTeacherHandler(IDbService dbService)
        {
            _dbService = dbService;
        }

        public async Task<Teacher?> Handle(
            GetTeacherQuery request,
            CancellationToken cancellationToken
        )
        {
            Teacher? teacher;
            switch (request.Role)
            {
                case "Admin":
                    teacher = await _dbService.Get<Teacher?>(
                        @"
                            SELECT 
                            [Id], [Name], [Surname], [ScheduleId] 
                            FROM [Teacher] 
                            WHERE [Id] = @Id;",
                        request
                    );
                    break;
                default:
                    teacher = await _dbService.Get<Teacher?>(
                        @"
                            SELECT 
                            t.[Id], t.[Name], [Surname], [ScheduleId] 
                            FROM [Teacher] t
                            INNER JOIN [Schedule] s ON t.[ScheduleId]=s.[Id]
                            WHERE t.[Id] = @Id AND s.[UserId] = @FilteredId;",
                        request
                    );
                    break;
            }

            if (teacher != null)
            {
                GetScheduleHandler getScheduleHandler = new GetScheduleHandler(_dbService);
                GetScheduleQuery getScheduleQuery = new GetScheduleQuery(
                    teacher.ScheduleId,
                    new Guid(),
                    "Admin"
                );
                ActionResult<Schedule?> schedule = await getScheduleHandler.Handle(
                    getScheduleQuery,
                    cancellationToken
                );
                teacher.Schedule = schedule.Value!;
            }

            return teacher;
        }
    }
}
