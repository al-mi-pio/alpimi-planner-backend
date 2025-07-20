using AlpimiAPI.Database;
using AlpimiAPI.Entities.EDayOff.Commands;
using AlpimiAPI.Entities.EHistory.DTO;
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
        private readonly IStringLocalizer<Fields> _strFields;

        public CreateLessonTypeHandler(
            IDbService dbService,
            IStringLocalizer<Errors> str,
            IStringLocalizer<Fields> strFields
        )
        {
            _dbService = dbService;
            _str = str;
            _strFields = strFields;
        }

        public async Task<Guid> Handle(
            CreateLessonTypeCommand request,
            CancellationToken cancellationToken
        )
        {
            if (request.dto.Color < 0 || request.dto.Color > 359)
            {
                throw new ApiErrorException(
                    [new FieldErrorObject("color", _str["badParameter", _strFields["Color"]])]
                );
            }

            GetScheduleHandler getScheduleHandler = new GetScheduleHandler(_dbService);
            GetScheduleQuery getScheduleQuery = new GetScheduleQuery(
                request.dto.ScheduleId!.Value,
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
                    WHERE [Name] = @Name AND [ScheduleId] = @ScheduleId;",
                request.dto
            );

            if (lessonTypeName != null)
            {
                throw new ApiErrorException(
                    [
                        new FieldErrorObject(
                            "name",
                            _str["alreadyExists", _strFields["LessonType"], request.dto.Name!]
                        )
                    ]
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
                    @ScheduleId);",
                request.dto
            );

            AddToHistoryHandler addToHistoryHandler = new AddToHistoryHandler(_dbService);
            AddToHistoryDTO addToHistoryDTO = new AddToHistoryDTO
            {
                Id = Guid.NewGuid(),
                Timestamp = DateTime.Now,
                AffectedEntityId = insertedId,
                AffectedEntity = "LessonType",
                Command = "Create",
                ReversaleDTO = null,
                CollisionChecked = false,
                ScheduleId = schedule.Value.Id
            };
            AddToHistoryCommand addToHistoryCommand = new AddToHistoryCommand(addToHistoryDTO);
            await addToHistoryHandler.Handle(addToHistoryCommand, cancellationToken);

            return insertedId;
        }
    }
}
