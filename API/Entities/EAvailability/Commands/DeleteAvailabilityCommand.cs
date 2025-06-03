using System.Text.Json;
using AlpimiAPI.Database;
using AlpimiAPI.Entities.EAvailability.DTO;
using AlpimiAPI.Entities.EDayOff.Commands;
using AlpimiAPI.Entities.EHistory.DTO;
using AlpimiAPI.Entities.ETeacher;
using AlpimiAPI.Entities.ETeacher.Queries;
using MediatR;
using Microsoft.AspNetCore.Mvc;

namespace AlpimiAPI.Entities.EAvailability.Commands
{
    public record DeleteAvailabilityCommand(Guid Id, Guid FilteredId, string Role) : IRequest;

    public class DeleteAvailabilityHandler : IRequestHandler<DeleteAvailabilityCommand>
    {
        private readonly IDbService _dbService;

        public DeleteAvailabilityHandler(IDbService dbService)
        {
            _dbService = dbService;
        }

        public async Task Handle(
            DeleteAvailabilityCommand request,
            CancellationToken cancellationToken
        )
        {
            Availability? availability;

            switch (request.Role)
            {
                case "Admin":
                    availability = await _dbService.Get<Availability?>(
                        @" 
                            SELECT
                            a.[Id], [WeekDay], [Start], [End], [TeacherId]
                            FROM [Availability]                          
                            WHERE [Id] = @Id;",
                        request
                    );
                    await _dbService.Delete(
                        @"
                            DELETE [Availability] 
                            WHERE [Id] = @Id;",
                        request
                    );
                    break;
                default:
                    availability = await _dbService.Get<Availability?>(
                        @" 
                            SELECT
                            a.[Id], [WeekDay], [Start], [End], [TeacherId]
                            FROM [Availability] a
                            INNER JOIN [Teacher] t ON t.[Id] = a.[TeacherId]
                            INNER JOIN [Schedule] s ON s.[Id] = t.[ScheduleId]
                            WHERE s.[UserId] = @FilteredId AND a.[Id] = @Id;",
                        request
                    );
                    await _dbService.Delete(
                        @"
                            DELETE a
                            FROM [Availability] a
                            INNER JOIN [Teacher] t ON t.[Id] = a.[TeacherId]
                            INNER JOIN [Schedule] s ON s.[Id] = t.[ScheduleId]
                            WHERE s.[UserId] = @FilteredId AND a.[Id] = @Id;",
                        request
                    );
                    break;
            }
            if (availability != null)
            {
                GetTeacherHandler getTeacherHandler = new GetTeacherHandler(_dbService);
                GetTeacherQuery getTeacherQuery = new GetTeacherQuery(
                    availability.TeacherId,
                    new Guid(),
                    "Admin"
                );
                ActionResult<Teacher?> schedule = await getTeacherHandler.Handle(
                    getTeacherQuery,
                    cancellationToken
                );
                availability.Teacher = schedule.Value!;

                CreateavAvailabilityDTO reversaleDTO = new CreateavAvailabilityDTO
                {
                    WeekDay = availability.WeekDay,
                    Start = availability.Start,
                    End = availability.End,
                    TeacherId = availability.TeacherId
                };
                AddToHistoryHandler addToHistoryHandler = new AddToHistoryHandler(_dbService);
                AddToHistoryDTO addToHistoryDTO = new AddToHistoryDTO
                {
                    Id = Guid.NewGuid(),
                    Timestamp = DateTime.Now,
                    AffectedEntityId = request.Id,
                    AffectedEntity = "Availability",
                    Command = "Delete",
                    ReversaleDTO = JsonSerializer.Serialize(reversaleDTO),
                    CollisionChecked = true,
                    ScheduleId = availability.Teacher.ScheduleId,
                };
                AddToHistoryCommand addToHistoryCommand = new AddToHistoryCommand(addToHistoryDTO);
                await addToHistoryHandler.Handle(addToHistoryCommand, cancellationToken);
            }
        }
    }
}
