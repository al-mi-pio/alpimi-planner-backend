using AlpimiAPI.Database;
using AlpimiAPI.Entities.EGroup;
using AlpimiAPI.Entities.EGroup.Queries;
using AlpimiAPI.Entities.ELesson.DTO;
using AlpimiAPI.Entities.ELesson.Queries;
using AlpimiAPI.Entities.ELessonType;
using AlpimiAPI.Entities.ELessonType.Queries;
using AlpimiAPI.Locales;
using AlpimiAPI.Responses;
using MediatR;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Localization;

namespace AlpimiAPI.Entities.ELesson.Commands
{
    public record UpdateLessonCommand(Guid Id, UpdateLessonDTO dto, Guid FilteredId, string Role)
        : IRequest<Lesson?>;

    public class UpdateLessonHandler : IRequestHandler<UpdateLessonCommand, Lesson?>
    {
        private readonly IDbService _dbService;
        private readonly IStringLocalizer<Errors> _str;

        public UpdateLessonHandler(IDbService dbService, IStringLocalizer<Errors> str)
        {
            _dbService = dbService;
            _str = str;
        }

        public async Task<Lesson?> Handle(
            UpdateLessonCommand request,
            CancellationToken cancellationToken
        )
        {
            if (request.dto.AmountOfHours < 1)
            {
                throw new ApiErrorException(
                    [new ErrorObject(_str["badParameter", "AmountOfHours"])]
                );
            }

            GetLessonHandler getLessonHandler = new GetLessonHandler(_dbService);
            GetLessonQuery getLessonQuery = new GetLessonQuery(
                request.Id,
                request.FilteredId,
                request.Role
            );

            ActionResult<Lesson?> originalLesson = await getLessonHandler.Handle(
                getLessonQuery,
                cancellationToken
            );

            if (originalLesson.Value == null)
            {
                return null;
            }

            request.dto.Name = request.dto.Name ?? originalLesson.Value!.Name;
            request.dto.AmountOfHours =
                request.dto.AmountOfHours ?? originalLesson.Value!.AmountOfHours;

            var lessonName = await _dbService.GetAll<Lesson>(
                $@"
                    SELECT 
                    [Id]
                    FROM [Lesson] 
                    WHERE [Name] = @Name AND [SubgroupId] = '{originalLesson .Value .SubgroupId}' AND [Id] != '{request.Id}';",
                request.dto
            );

            if (lessonName!.Any())
            {
                throw new ApiErrorException(
                    [new ErrorObject(_str["alreadyExists", "Lesson", request.dto.Name])]
                );
            }

            var lesson = await _dbService.Update<Lesson?>(
                $@"
                    UPDATE [Lesson] 
                    SET
                    [Name] = @Name, [AmountOfHours] = @AmountOfHours 
                    OUTPUT
                    INSERTED.[Id],
                    INSERTED.[Name],
                    INSERTED.[AmountOfHours],
                    INSERTED.[LessonTypeId],
                    INSERTED.[SubgroupId]
                    WHERE [Id] = '{request.Id}';",
                request.dto
            );

            GetLessonTypeHandler getLessonTypeHandler = new GetLessonTypeHandler(_dbService);
            GetLessonTypeQuery getLessonTypeQuery = new GetLessonTypeQuery(
                lesson!.LessonTypeId,
                new Guid(),
                "Admin"
            );
            ActionResult<LessonType?> lessonType = await getLessonTypeHandler.Handle(
                getLessonTypeQuery,
                cancellationToken
            );
            lesson.LessonType = lessonType.Value!;

            return lesson;
        }
    }
}
