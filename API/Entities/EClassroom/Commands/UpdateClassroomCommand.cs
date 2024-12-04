using AlpimiAPI.Database;
using AlpimiAPI.Entities.EClassroom.DTO;
using AlpimiAPI.Entities.EClassroom.Queries;
using AlpimiAPI.Entities.ESchedule;
using AlpimiAPI.Entities.ESchedule.Queries;
using AlpimiAPI.Locales;
using AlpimiAPI.Responses;
using MediatR;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Localization;

namespace AlpimiAPI.Entities.EClassroom.Commands
{
    public record UpdateClassroomCommand(
        Guid Id,
        UpdateClassroomDTO dto,
        Guid FilteredId,
        string Role
    ) : IRequest<Classroom?>;

    public class UpdateClassroomHandler : IRequestHandler<UpdateClassroomCommand, Classroom?>
    {
        private readonly IDbService _dbService;
        private readonly IStringLocalizer<Errors> _str;

        public UpdateClassroomHandler(IDbService dbService, IStringLocalizer<Errors> str)
        {
            _dbService = dbService;
            _str = str;
        }

        public async Task<Classroom?> Handle(
            UpdateClassroomCommand request,
            CancellationToken cancellationToken
        )
        {
            if (request.dto.Capacity < 1)
            {
                throw new ApiErrorException([new ErrorObject(_str["badParameter", "Capacity"])]);
            }

            GetClassroomHandler getClassroomHandler = new GetClassroomHandler(_dbService);
            GetClassroomQuery getClassroomQuery = new GetClassroomQuery(
                request.Id,
                request.FilteredId,
                request.Role
            );
            ActionResult<Classroom?> originalClassroom = await getClassroomHandler.Handle(
                getClassroomQuery,
                cancellationToken
            );

            if (originalClassroom.Value == null)
            {
                return null;
            }

            request.dto.Name = request.dto.Name ?? originalClassroom.Value!.Name;

            var classroomName = await _dbService.Get<Classroom>(
                $@"
                    SELECT 
                    [Id]
                    FROM [Classroom] 
                    WHERE [Name] = @Name AND [ScheduleId] = '{originalClassroom .Value .ScheduleId}' AND [Id] != '{request.Id}';",
                request.dto
            );

            if (classroomName != null)
            {
                throw new ApiErrorException(
                    [new ErrorObject(_str["alreadyExists", "Classroom", request.dto.Name])]
                );
            }

            var classroom = await _dbService.Update<Classroom?>(
                $@"
                    UPDATE [Classroom] 
                    SET
                    [Name] = @Name, [Capacity] = @Capacity
                    OUTPUT
                    INSERTED.[Id],
                    INSERTED.[Name],
                    INSERTED.[Capacity],
                    INSERTED.[ScheduleId]
                    WHERE [Id] = '{request.Id}';",
                request.dto
            );

            GetScheduleHandler getScheduleHandler = new GetScheduleHandler(_dbService);
            GetScheduleQuery getScheduleQuery = new GetScheduleQuery(
                classroom!.ScheduleId,
                new Guid(),
                "Admin"
            );
            ActionResult<Schedule?> toInsertSchedule = await getScheduleHandler.Handle(
                getScheduleQuery,
                cancellationToken
            );
            classroom.Schedule = toInsertSchedule.Value!;

            return classroom;
        }
    }
}
