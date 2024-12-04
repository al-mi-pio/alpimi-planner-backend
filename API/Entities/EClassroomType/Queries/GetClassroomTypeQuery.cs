using AlpimiAPI.Database;
using AlpimiAPI.Entities.ESchedule;
using AlpimiAPI.Entities.ESchedule.Queries;
using MediatR;
using Microsoft.AspNetCore.Mvc;

namespace AlpimiAPI.Entities.EClassroomType.Queries
{
    public record GetClassroomTypeQuery(Guid Id, Guid FilteredId, string Role)
        : IRequest<ClassroomType?>;

    public class GetClassroomTypeHandler : IRequestHandler<GetClassroomTypeQuery, ClassroomType?>
    {
        private readonly IDbService _dbService;

        public GetClassroomTypeHandler(IDbService dbService)
        {
            _dbService = dbService;
        }

        public async Task<ClassroomType?> Handle(
            GetClassroomTypeQuery request,
            CancellationToken cancellationToken
        )
        {
            ClassroomType? teacher;
            switch (request.Role)
            {
                case "Admin":
                    teacher = await _dbService.Get<ClassroomType?>(
                        @"
                            SELECT 
                            [Id], [Name], [ScheduleId] 
                            FROM [ClassroomType] 
                            WHERE [Id] = @Id;",
                        request
                    );
                    break;
                default:
                    teacher = await _dbService.Get<ClassroomType?>(
                        @"
                            SELECT 
                            ct.[Id], ct.[Name], [ScheduleId] 
                            FROM [ClassroomType] ct
                            INNER JOIN [Schedule] s ON ct.[ScheduleId]=s.[Id]
                            WHERE ct.[Id] = @Id AND s.[UserId] = @FilteredId;",
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
