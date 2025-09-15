using System.Text.Json;
using AlpimiAPI.Database;
using AlpimiAPI.Entities.EDayOff.Commands;
using AlpimiAPI.Entities.EHistory.DTO;
using AlpimiAPI.Entities.EStudent.DTO;
using AlpimiAPI.Entities.EStudent.Queries;
using MediatR;
using Microsoft.AspNetCore.Mvc;

namespace AlpimiAPI.Entities.EStudent.Commands
{
    public record DeleteStudentCommand(Guid Id, Guid FilteredId, string Role) : IRequest;

    public class DeleteStudentHandler : IRequestHandler<DeleteStudentCommand>
    {
        private readonly IDbService _dbService;

        public DeleteStudentHandler(IDbService dbService)
        {
            _dbService = dbService;
        }

        public async Task Handle(DeleteStudentCommand request, CancellationToken cancellationToken)
        {
            GetStudentHandler getStudentHandler = new GetStudentHandler(_dbService);
            GetStudentQuery getStudentQuery = new GetStudentQuery(
                request.Id,
                request.FilteredId,
                request.Role
            );
            ActionResult<Student?> student = await getStudentHandler.Handle(
                getStudentQuery,
                cancellationToken
            );
            if (student.Value != null)
            {
                var subgroups = await _dbService.GetAll<Guid>(
                    @"
                        SELECT
                        sg.[Id]    
                        FROM [Subgroup] sg
                        LEFT JOIN [StudentSubgroup] ssg on ssg.[StudentId] = sg.[Id]
                        LEFT JOIN [Student] s on s.[Id] = ssg.[StudentId]
                        WHERE s.[Id] = @Id;
                    ",
                    request
                );
                CreateStudentDTO reversaleDTO = new CreateStudentDTO
                {
                    AlbumNumber = student.Value.AlbumNumber,
                    GroupId = student.Value.GroupId,
                    SubgroupIds = subgroups
                };
                AddToHistoryHandler addToHistoryHandler = new AddToHistoryHandler(_dbService);
                AddToHistoryDTO addToHistoryDTO = new AddToHistoryDTO
                {
                    Id = Guid.NewGuid(),
                    Timestamp = DateTime.Now,
                    AffectedEntityId = request.Id,
                    AffectedEntity = "Student",
                    Command = "Delete",
                    ReversaleDTO = JsonSerializer.Serialize(reversaleDTO),
                    CollisionChecked = false,
                    ScheduleId = student.Value.Group.ScheduleId,
                };
                AddToHistoryCommand addToHistoryCommand = new AddToHistoryCommand(addToHistoryDTO);
                await addToHistoryHandler.Handle(addToHistoryCommand, cancellationToken);
            }

            switch (request.Role)
            {
                case "Admin":
                    await _dbService.Delete(
                        @"
                            DELETE [Student] 
                            WHERE [Id] = @Id;",
                        request
                    );
                    break;
                default:
                    await _dbService.Delete(
                        @"
                            DELETE st
                            FROM [Student] st
                            INNER JOIN [Group] g on g.[Id] = st.[GroupId]
                            INNER JOIN [Schedule] s ON s.[Id] = g.[ScheduleId]
                            WHERE s.[UserId] = @FilteredId AND st.[Id] = @Id;",
                        request
                    );
                    break;
            }
        }
    }
}
