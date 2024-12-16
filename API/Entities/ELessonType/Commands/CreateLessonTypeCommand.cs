using AlpimiAPI.Database;
using AlpimiAPI.Entities.ELessonType.DTO;
using AlpimiAPI.Entities.ESchedule;
using AlpimiAPI.Entities.ESchedule.Queries;
using AlpimiAPI.Locales;
using AlpimiAPI.Responses;
using MediatR;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Localization;

namespace AlpimiAPI.Entities.ELessonType.Commands
{
    public record CreateLessonTypeCommand(
        Guid Id,
        CreateLessonTypeDTO dto,
        Guid FilteredId,
        string Role
    ) : IRequest<Guid>;

    public class CreateLessonTypeHandler : IRequestHandler<CreateLessonTypeCommand, Guid>
    {
        private readonly IDbService _dbService;
        private readonly IStringLocalizer<Errors> _str;

        public CreateLessonTypeHandler(IDbService dbService, IStringLocalizer<Errors> str)
        {
            _dbService = dbService;
            _str = str;
        }

        public async Task<Guid> Handle(
            CreateLessonTypeCommand request,
            CancellationToken cancellationToken
        )
        {
            if (request.dto.Color < 0 || request.dto.Color > 359)
            {
                throw new ApiErrorException([new ErrorObject(_str["badParameter", "Color"])]);
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

            var lessonTypeName = await _dbService.Get<LessonType>(
                @"
                    SELECT 
                    [Id]
                    FROM [LessonType] 
                    WHERE [Name] = @Name AND [ScheduleId] = @ScheduleId; ",
                request.dto
            );

            if (lessonTypeName != null)
            {
                throw new ApiErrorException(
                    [new ErrorObject(_str["alreadyExists", "LessonType", request.dto.Name])]
                );
            }

            var insertedId = await _dbService.Post<Guid>(
                $@"
                    INSERT INTO [LessonType] 
                    ([Id], [Name], [Color], [ScheduleId])
                    OUTPUT  
                    INSERTED.Id                    
                    VALUES (
                    '{request.Id}',   
                    @Name,
                    @Color,
                    @ScheduleId); ",
                request.dto
            );

            return insertedId;
        }
    }
}
