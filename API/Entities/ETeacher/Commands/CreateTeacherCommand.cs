using AlpimiAPI.Database;
using AlpimiAPI.Entities.ESchedule;
using AlpimiAPI.Entities.ESchedule.Queries;
using AlpimiAPI.Entities.ETeacher.DTO;
using AlpimiAPI.Locales;
using AlpimiAPI.Responses;
using MediatR;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Localization;

namespace AlpimiAPI.Entities.ETeacher.Commands
{
    public record CreateTeacherCommand(Guid Id, CreateTeacherDTO dto, Guid FilteredId, string Role)
        : IRequest<Guid>;

    public class CreateTeacherHandler : IRequestHandler<CreateTeacherCommand, Guid>
    {
        private readonly IDbService _dbService;
        private readonly IStringLocalizer<Errors> _str;

        public CreateTeacherHandler(IDbService dbService, IStringLocalizer<Errors> str)
        {
            _dbService = dbService;
            _str = str;
        }

        public async Task<Guid> Handle(
            CreateTeacherCommand request,
            CancellationToken cancellationToken
        )
        {
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
                throw new ApiErrorException([new ErrorObject(_str["notFound", "Schedule"])]);
            }

            var teacherName = await _dbService.GetAll<Teacher>(
                @"
                    SELECT 
                    [Id]
                    FROM [Teacher] 
                    WHERE [Name] = @Name AND [Surname] = @Surname  AND [ScheduleId] = @ScheduleId;",
                request.dto
            );

            if (teacherName!.Any())
            {
                throw new ApiErrorException(
                    [
                        new ErrorObject(
                            _str[
                                "alreadyExists",
                                "Teacher",
                                request.dto.Name + " " + request.dto.Surname
                            ]
                        )
                    ]
                );
            }

            var insertedId = await _dbService.Post<Guid>(
                $@"
                    INSERT INTO [Teacher] 
                    ([Id],[Name],[Surname],[ScheduleId])
                    OUTPUT 
                    INSERTED.Id                    
                    VALUES (
                    '{request.Id}',   
                    @Name,
                    @Surname,
                    @ScheduleId);",
                request.dto
            );

            return insertedId;
        }
    }
}
