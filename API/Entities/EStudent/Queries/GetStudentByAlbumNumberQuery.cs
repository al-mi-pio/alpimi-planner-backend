using AlpimiAPI.Database;
using AlpimiAPI.Entities.EGroup;
using AlpimiAPI.Entities.EGroup.Queries;
using MediatR;
using Microsoft.AspNetCore.Mvc;

namespace AlpimiAPI.Entities.EStudent.Queries
{
    public record GetStudentByAlbumNumberQuery(string AlbumNumber, Guid ScheduleId)
        : IRequest<Student?>;

    public class GetStudentByAlbumNumberHandler
        : IRequestHandler<GetStudentByAlbumNumberQuery, Student?>
    {
        private readonly IDbService _dbService;

        public GetStudentByAlbumNumberHandler(IDbService dbService)
        {
            _dbService = dbService;
        }

        public async Task<Student?> Handle(
            GetStudentByAlbumNumberQuery request,
            CancellationToken cancellationToken
        )
        {
            Student? student = await _dbService.Get<Student?>(
                @"
                    SELECT 
                    st.[Id], [AlbumNumber], [GroupId] 
                    FROM [Student] st
                    INNER JOIN [Group] g ON g.[Id] = st.[GroupId]
                    INNER JOIN [Schedule] s ON g.[ScheduleId] = s.[Id]
                    INNER JOIN [ScheduleSettings] ss ON ss.[ScheduleId] = s.[Id]
                    WHERE st.[AlbumNumber] = @AlbumNumber AND ss.[IsPublic] = 'TRUE' AND s.[Id] = @ScheduleId;",
                request
            );

            if (student != null)
            {
                GetGroupHandler getGroupHandler = new GetGroupHandler(_dbService);
                GetGroupQuery getGroupQuery = new GetGroupQuery(
                    student.GroupId,
                    new Guid(),
                    "Admin"
                );
                ActionResult<Group?> group = await getGroupHandler.Handle(
                    getGroupQuery,
                    cancellationToken
                );
                student.Group = group.Value!;
            }

            return student;
        }
    }
}
