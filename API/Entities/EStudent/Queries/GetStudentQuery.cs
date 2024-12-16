using AlpimiAPI.Database;
using AlpimiAPI.Entities.EGroup;
using AlpimiAPI.Entities.EGroup.Queries;
using MediatR;
using Microsoft.AspNetCore.Mvc;

namespace AlpimiAPI.Entities.EStudent.Queries
{
    public record GetStudentQuery(Guid Id, Guid FilteredId, string Role) : IRequest<Student?>;

    public class GetStudentHandler : IRequestHandler<GetStudentQuery, Student?>
    {
        private readonly IDbService _dbService;

        public GetStudentHandler(IDbService dbService)
        {
            _dbService = dbService;
        }

        public async Task<Student?> Handle(
            GetStudentQuery request,
            CancellationToken cancellationToken
        )
        {
            Student? student;
            switch (request.Role)
            {
                case "Admin":
                    student = await _dbService.Get<Student?>(
                        @"
                            SELECT 
                            [Id], [AlbumNumber], [GroupId] 
                            FROM [Student] 
                            WHERE [Id] = @Id; ",
                        request
                    );
                    break;
                default:
                    student = await _dbService.Get<Student?>(
                        @"
                            SELECT 
                            st.[Id], [AlbumNumber], [GroupId] 
                            FROM [Student] st
                            INNER JOIN [Group] g ON g.[Id] = st.[GroupId]
                            INNER JOIN [Schedule] s ON g.[ScheduleId] = s.[Id]
                            WHERE st.[Id] = @Id AND s.[UserId] = @FilteredId; ",
                        request
                    );
                    break;
            }

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
