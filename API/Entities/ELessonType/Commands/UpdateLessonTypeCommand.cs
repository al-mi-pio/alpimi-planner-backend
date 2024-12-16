using AlpimiAPI.Database;
using AlpimiAPI.Entities.ELessonType.DTO;
using AlpimiAPI.Entities.ELessonType.Queries;
using AlpimiAPI.Entities.ESchedule;
using AlpimiAPI.Entities.ESchedule.Queries;
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

        public UpdateLessonTypeHandler(IDbService dbService, IStringLocalizer<Errors> str)
        {
            _dbService = dbService;
            _str = str;
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
                    throw new ApiErrorException([new ErrorObject(_str["badParameter", "Color"])]);
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

            request.dto.Name = request.dto.Name ?? originalLessonType.Value!.Name;

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

            return lessonType;
        }
    }
}
