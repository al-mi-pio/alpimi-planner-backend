using AlpimiAPI.Database;
using AlpimiAPI.Entities.EClassroomType;
using AlpimiAPI.Entities.EClassroomType.Queries;
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

            List<ErrorObject> errors = new List<ErrorObject>();
            if (request.dto.ClassroomTypeIds != null)
            {
                var duplicates = request
                    .dto.ClassroomTypeIds.GroupBy(g => g)
                    .Where(g => g.Count() > 1)
                    .Select(g => g.Key);

                if (duplicates.Any())
                {
                    List<ErrorObject> duplicateErrors = new List<ErrorObject>();
                    foreach (var duplicate in duplicates)
                    {
                        duplicateErrors.Add(
                            new ErrorObject(_str["duplicateData", "ClassroomType", duplicate])
                        );
                    }
                    throw new ApiErrorException(duplicateErrors);
                }

                foreach (var classroomTypeId in request.dto.ClassroomTypeIds)
                {
                    GetClassroomTypeHandler getClassroomTypeHandler = new GetClassroomTypeHandler(
                        _dbService
                    );
                    GetClassroomTypeQuery getClassroomTypeQuery = new GetClassroomTypeQuery(
                        classroomTypeId,
                        request.FilteredId,
                        request.Role
                    );
                    ActionResult<ClassroomType?> classroomType =
                        await getClassroomTypeHandler.Handle(
                            getClassroomTypeQuery,
                            cancellationToken
                        );

                    if (classroomType.Value == null)
                    {
                        errors.Add(
                            new ErrorObject(
                                _str["resourceNotFound", "ClassroomType", classroomTypeId]
                            )
                        );
                    }
                }
                if (errors.Count != 0)
                {
                    throw new ApiErrorException(errors);
                }

                var classroomTypes = await _dbService.GetAll<Guid>(
                    $@"
                        SELECT
                        ct.[Id]
                        FROM [ClassroomType] ct
                        LEFT JOIN [LessonClassroomType] lct ON lct.[ClassroomTypeId] = ct.[Id]
                        LEFT JOIN [Lesson] l ON l.[Id] = lct.[LessonId]
                        WHERE l.[Id] = @Id",
                    request
                );

                classroomTypes = classroomTypes ?? [];

                foreach (Guid classroomTypeId in request.dto.ClassroomTypeIds)
                {
                    if (!classroomTypes.Contains(classroomTypeId))
                    {
                        await _dbService.Post<Guid>(
                            $@"
                                INSERT INTO [LessonClassroomType] 
                                ([Id],[LessonId],[ClassroomTypeId])
                                OUTPUT 
                                INSERTED.Id                    
                                VALUES (
                                '{Guid.NewGuid()}',   
                                @Id,
                                '{classroomTypeId}');",
                            request
                        );
                    }
                }
                foreach (Guid classroomType in classroomTypes)
                {
                    if (!request.dto.ClassroomTypeIds.Contains(classroomType))
                    {
                        await _dbService.Delete(
                            $@"
                                DELETE [LessonClassroomType] 
                                WHERE [LessonId] = @Id AND [ClassroomTypeId] = '{classroomType}';",
                            request
                        );
                    }
                }
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
