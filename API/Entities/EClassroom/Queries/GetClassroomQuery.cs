using AlpimiAPI.Database;
using AlpimiAPI.Entities.ESchedule;
using AlpimiAPI.Entities.ESchedule.Queries;
using MediatR;
using Microsoft.AspNetCore.Mvc;

namespace AlpimiAPI.Entities.EClassroom.Queries
{
    public record GetClassroomQuery(Guid Id, Guid FilteredId, string Role) : IRequest<Classroom?>;

    public class GetClassroomHandler : IRequestHandler<GetClassroomQuery, Classroom?>
    {
        private readonly IDbService _dbService;

        public GetClassroomHandler(IDbService dbService)
        {
            _dbService = dbService;
        }

        public async Task<Classroom?> Handle(
            GetClassroomQuery request,
            CancellationToken cancellationToken
        )
        {
            Classroom? classroom;
            switch (request.Role)
            {
                case "Admin":
                    classroom = await _dbService.Get<Classroom?>(
                        @"
                            SELECT 
                            [Id], [Name], [Capacity], [ScheduleId] 
                            FROM [Classroom] 
                            WHERE [Id] = @Id;",
                        request
                    );
                    break;
                default:
                    classroom = await _dbService.Get<Classroom?>(
                        @"
                            SELECT 
                            c.[Id], c.[Name], [Capacity], [ScheduleId] 
                            FROM [Classroom] c
                            INNER JOIN [Schedule] s ON c.[ScheduleId] = s.[Id]
                            WHERE c.[Id] = @Id AND s.[UserId] = @FilteredId;",
                        request
                    );
                    break;
            }

            if (classroom != null)
            {
                GetScheduleHandler getScheduleHandler = new GetScheduleHandler(_dbService);
                GetScheduleQuery getScheduleQuery = new GetScheduleQuery(
                    classroom.ScheduleId,
                    new Guid(),
                    "Admin"
                );
                ActionResult<Schedule?> schedule = await getScheduleHandler.Handle(
                    getScheduleQuery,
                    cancellationToken
                );
                classroom.Schedule = schedule.Value!;
            }
            return classroom;
        }
    }
}
