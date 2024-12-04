using AlpimiAPI.Database;
using AlpimiAPI.Entities.EClassroom.DTO;
using AlpimiAPI.Entities.ESchedule;
using AlpimiAPI.Entities.ESchedule.Queries;
using AlpimiAPI.Locales;
using AlpimiAPI.Responses;
using MediatR;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Localization;

namespace AlpimiAPI.Entities.EClassroom.Commands
{
    public record CreateClassroomCommand(
        Guid Id,
        CreateClassroomDTO dto,
        Guid FilteredId,
        string Role
    ) : IRequest<Guid>;

    public class CreateClassroomHandler : IRequestHandler<CreateClassroomCommand, Guid>
    {
        private readonly IDbService _dbService;
        private readonly IStringLocalizer<Errors> _str;

        public CreateClassroomHandler(IDbService dbService, IStringLocalizer<Errors> str)
        {
            _dbService = dbService;
            _str = str;
        }

        public async Task<Guid> Handle(
            CreateClassroomCommand request,
            CancellationToken cancellationToken
        )
        {
            if (request.dto.Capacity < 1)
            {
                throw new ApiErrorException([new ErrorObject(_str["badParameter", "Capacity"])]);
            }

            GetScheduleHandler getScheduleHandler = new GetScheduleHandler(_dbService);
            GetScheduleQuery getScheduleQuery = new GetScheduleQuery(
                request.dto.ScheduleId,
                request.FilteredId,
                request.Role
            );

            ActionResult<Schedule?> schedule = await getScheduleHandler.Handle(
                getScheduleQuery,
                cancellationToken
            );
            if (schedule.Value == null)
            {
                throw new ApiErrorException(
                    [new ErrorObject(_str["resourceNotFound", "Schedule", request.dto.ScheduleId])]
                );
            }

            var classroomName = await _dbService.Get<Classroom>(
                @"
                    SELECT 
                    [Id]
                    FROM [Classroom] 
                    WHERE [Name] = @Name AND [ScheduleId] = @ScheduleId;",
                request.dto
            );

            if (classroomName != null)
            {
                throw new ApiErrorException(
                    [new ErrorObject(_str["alreadyExists", "Classroom", request.dto.Name])]
                );
            }

            var insertedId = await _dbService.Post<Guid>(
                $@"
                    INSERT INTO [Classroom] 
                    ([Id],[Name],[Capacity],[ScheduleId])
                    OUTPUT 
                    INSERTED.Id                    
                    VALUES (
                    '{request.Id}',   
                    @Name,
                    @Capacity,
                    @ScheduleId);",
                request.dto
            );

            return insertedId;
        }
    }
}
