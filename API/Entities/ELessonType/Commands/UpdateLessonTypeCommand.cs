using System.Text.Json;
using AlpimiAPI.Database;
using AlpimiAPI.Entities.EDayOff.Commands;
using AlpimiAPI.Entities.EHistory.DTO;
using AlpimiAPI.Entities.ELessonType.DTO;
using AlpimiAPI.Entities.ELessonType.Queries;
using AlpimiAPI.Locales;
using AlpimiAPI.Responses;
using MediatR;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Localization;

namespace AlpimiAPI.Entities.ELessonType.Commands
{
    public record UpdateLessonTypeCommand(
        Guid Id,
        UpdateLessonTypeDTO dto,
        Guid FilteredId,
        string Role
    ) : IRequest<LessonType?>;

    public class UpdateLessonTypeHandler : IRequestHandler<UpdateLessonTypeCommand, LessonType?>
    {
        private readonly IDbService _dbService;
        private readonly IStringLocalizer<Errors> _str;
        private readonly IStringLocalizer<Fields> _strFields;

        public UpdateLessonTypeHandler(
            IDbService dbService,
            IStringLocalizer<Errors> str,
            IStringLocalizer<Fields> strFields
        )
        {
            _dbService = dbService;
            _str = str;
            _strFields = strFields;
        }

        public async Task<LessonType?> Handle(
            UpdateLessonTypeCommand request,
            CancellationToken cancellationToken
        )
        {
            if (request.dto.Color != null)
            {
                if (request.dto.Color < 0 || request.dto.Color > 359)
                {
                    throw new ApiErrorException(
                        [new FieldErrorObject("Color", _str["badParameter", _strFields["Color"]])]
                    );
                }
            }

            GetLessonTypeHandler getLessonTypeHandler = new GetLessonTypeHandler(_dbService);
            GetLessonTypeQuery getLessonTypeQuery = new GetLessonTypeQuery(
                request.Id,
                request.FilteredId,
                request.Role
            );
            ActionResult<LessonType?> originalLessonType = await getLessonTypeHandler.Handle(
                getLessonTypeQuery,
                cancellationToken
            );

            if (originalLessonType.Value == null)
            {
                return null;
            }

            UpdateLessonTypeDTO reversaleDTOLessonType = new UpdateLessonTypeDTO
            {
                Name = originalLessonType.Value!.Name,
                Color = originalLessonType.Value.Color
            };

            request.dto.Name = request.dto.Name ?? originalLessonType.Value!.Name;
            request.dto.Color = request.dto.Color ?? originalLessonType.Value.Color;

            var lessonTypeName = await _dbService.Get<LessonType>(
                $@"
                    SELECT 
                    [Id]
                    FROM [LessonType] 
                    WHERE [Name] = @Name AND [ScheduleId] = '{originalLessonType .Value .ScheduleId}' AND [Id] != '{request.Id}';",
                request.dto
            );

            if (lessonTypeName != null)
            {
                throw new ApiErrorException(
                    [new ErrorObject(_str["alreadyExists", "LessonType", request.dto.Name])]
                );
            }

            var lessonType = await _dbService.Update<LessonType?>(
                $@"
                    UPDATE [LessonType] 
                    SET
                    [Name] = @Name, [Color] = @Color
                    OUTPUT
                    INSERTED.[Id],
                    INSERTED.[Name],
                    INSERTED.[Color],
                    INSERTED.[ScheduleId]
                    WHERE [Id] = '{request.Id}';",
                request.dto
            );

            lessonType!.Schedule = originalLessonType.Value.Schedule;

            AddToHistoryHandler addToHistoryHandler = new AddToHistoryHandler(_dbService);
            AddToHistoryDTO addToHistoryDTO = new AddToHistoryDTO
            {
                Id = Guid.NewGuid(),
                Timestamp = DateTime.Now,
                AffectedEntityId = request.Id,
                AffectedEntity = "LessonType",
                Command = "Patch",
                ReversaleDTO = JsonSerializer.Serialize(reversaleDTOLessonType),
                CollisionChecked = false,
                ScheduleId = originalLessonType.Value.ScheduleId,
            };
            AddToHistoryCommand addToHistoryCommand = new AddToHistoryCommand(addToHistoryDTO);
            await addToHistoryHandler.Handle(addToHistoryCommand, cancellationToken);

            return lessonType;
        }
    }
}
