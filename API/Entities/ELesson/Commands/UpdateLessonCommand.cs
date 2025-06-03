using System.Text.Json;
using AlpimiAPI.Database;
using AlpimiAPI.Entities.EClassroomType;
using AlpimiAPI.Entities.EClassroomType.Queries;
using AlpimiAPI.Entities.EDayOff.Commands;
using AlpimiAPI.Entities.EHistory.DTO;
using AlpimiAPI.Entities.ELesson.DTO;
using AlpimiAPI.Entities.ELesson.Queries;
using AlpimiAPI.Entities.ELessonType;
using AlpimiAPI.Entities.ELessonType.Queries;
using AlpimiAPI.Entities.ESubgroup;
using AlpimiAPI.Entities.ESubgroup.Queries;
using AlpimiAPI.Entities.ETeacher;
using AlpimiAPI.Entities.ETeacher.Queries;
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
            if (request.dto.AmountOfHours != null)
            {
                if (request.dto.AmountOfHours < 1)
                {
                    throw new ApiErrorException(
                        [new ErrorObject(_str["badParameter", "AmountOfHours"])]
                    );
                }
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

            UpdateLessonDTO reversaleDTOLesson = new UpdateLessonDTO
            {
                Name = originalLesson.Value!.Name,
                AmountOfHours = originalLesson.Value.AmountOfHours,
                TeacherId = originalLesson.Value.TeacherId,
                LessonTypeId = originalLesson.Value.LessonTypeId,
            };

            request.dto.Name = request.dto.Name ?? originalLesson.Value!.Name;
            request.dto.AmountOfHours =
                request.dto.AmountOfHours ?? originalLesson.Value!.AmountOfHours;
            request.dto.TeacherId = request.dto.TeacherId ?? originalLesson.Value!.TeacherId;
            request.dto.LessonTypeId =
                request.dto.LessonTypeId ?? originalLesson.Value!.LessonTypeId;

            List<ErrorObject> errors = new List<ErrorObject>();
            GetLessonTypeHandler getLessonTypeHandler = new GetLessonTypeHandler(_dbService);
            GetLessonTypeQuery getLessonTypeQuery = new GetLessonTypeQuery(
                request.dto.LessonTypeId.Value,
                request.FilteredId,
                request.Role
            );
            ActionResult<LessonType?> lessonType = await getLessonTypeHandler.Handle(
                getLessonTypeQuery,
                cancellationToken
            );

            if (lessonType.Value == null)
            {
                errors.Add(
                    new ErrorObject(
                        _str["resourceNotFound", "LessonType", request.dto.LessonTypeId]
                    )
                );
            }

            GetTeacherHandler getTeacherHandler = new GetTeacherHandler(_dbService);
            GetTeacherQuery getTeacherQuery = new GetTeacherQuery(
                request.dto.TeacherId.Value,
                request.FilteredId,
                request.Role
            );
            ActionResult<Teacher?> teacher = await getTeacherHandler.Handle(
                getTeacherQuery,
                cancellationToken
            );

            if (teacher.Value == null)
            {
                errors.Add(
                    new ErrorObject(_str["resourceNotFound", "Teacher", request.dto.TeacherId])
                );
            }

            if (errors.Count != 0)
            {
                throw new ApiErrorException(errors);
            }

            if (teacher.Value!.ScheduleId != lessonType.Value!.ScheduleId)
            {
                throw new ApiErrorException(
                    [new ErrorObject(_str["wrongSet", "Teacher", "Schedule", "LessonType"])]
                );
            }

            var lessonName = await _dbService.GetAll<Lesson>(
                $@"
                    SELECT 
                    [Id]
                    FROM [Lesson] 
                    WHERE [Name] = @Name AND [TeacherId] = '{originalLesson .Value .TeacherId}' AND [Id] != '{request.Id}';",
                request.dto
            );

            if (lessonName!.Any())
            {
                throw new ApiErrorException(
                    [new ErrorObject(_str["alreadyExists", "Lesson", request.dto.Name])]
                );
            }
            if (request.dto.SubgroupIds != null)
            {
                var duplicates = request
                    .dto.SubgroupIds.GroupBy(g => g)
                    .Where(g => g.Count() > 1)
                    .Select(g => g.Key);

                if (duplicates.Any())
                {
                    List<ErrorObject> duplicateErrors = new List<ErrorObject>();
                    foreach (var duplicate in duplicates)
                    {
                        duplicateErrors.Add(
                            new ErrorObject(_str["duplicateData", "Subgroup", duplicate])
                        );
                    }
                    throw new ApiErrorException(duplicateErrors);
                }

                foreach (var subgroupId in request.dto.SubgroupIds)
                {
                    GetSubgroupHandler getSubgroupHandler = new GetSubgroupHandler(_dbService);
                    GetSubgroupQuery getSubgroupQuery = new GetSubgroupQuery(
                        subgroupId,
                        request.FilteredId,
                        request.Role
                    );
                    ActionResult<Subgroup?> subgroup = await getSubgroupHandler.Handle(
                        getSubgroupQuery,
                        cancellationToken
                    );

                    if (subgroup.Value == null)
                    {
                        errors.Add(
                            new ErrorObject(_str["resourceNotFound", "Subgroup", subgroupId])
                        );
                    }
                    else if (
                        subgroup.Value.Group.ScheduleId
                        != originalLesson.Value.LessonType.ScheduleId
                    )
                    {
                        errors.Add(
                            new ErrorObject(_str["wrongSet", "Subgroup", "Schedule", "LessonType"])
                        );
                    }
                }

                if (errors.Count != 0)
                {
                    throw new ApiErrorException(errors);
                }

                var subgroups = await _dbService.GetAll<Guid>(
                    $@"
                        SELECT
                        sg.[Id]
                        FROM [Subgroup] sg
                        LEFT JOIN [LessonSubgroup] lsg ON lsg.[SubgroupId] = sg.[Id]
                        LEFT JOIN [Lesson] l ON l.[Id] = lsg.[LessonId]
                        WHERE l.[Id] = @Id;",
                    request
                );

                subgroups = subgroups ?? [];
                reversaleDTOLesson.SubgroupIds = subgroups;
                foreach (Guid subgroupId in request.dto.SubgroupIds)
                {
                    if (!subgroups.Contains(subgroupId))
                    {
                        await _dbService.Post<Guid>(
                            $@"
                                INSERT INTO [LessonSubgroup] 
                                ([Id], [LessonId], [SubgroupId])
                                OUTPUT 
                                INSERTED.Id                    
                                VALUES (
                                '{Guid.NewGuid()}',   
                                @Id,
                                '{subgroupId}');",
                            request
                        );
                    }
                }
                foreach (Guid subgroup in subgroups)
                {
                    if (!request.dto.SubgroupIds.Contains(subgroup))
                    {
                        await _dbService.Delete(
                            $@"
                                DELETE [LessonSubgroup] 
                                WHERE [LessonId] = @Id AND [SubgroupId] = '{subgroup}';",
                            request
                        );
                    }
                }
            }

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
                    else if (
                        classroomType.Value.ScheduleId != originalLesson.Value.LessonType.ScheduleId
                    )
                    {
                        errors.Add(
                            new ErrorObject(_str["wrongSet", "ClassroomType", "Schedule", "Lesson"])
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
                        WHERE l.[Id] = @Id;",
                    request
                );

                classroomTypes = classroomTypes ?? [];
                reversaleDTOLesson.ClassroomTypeIds = classroomTypes;
                foreach (Guid classroomTypeId in request.dto.ClassroomTypeIds)
                {
                    if (!classroomTypes.Contains(classroomTypeId))
                    {
                        await _dbService.Post<Guid>(
                            $@"
                                INSERT INTO [LessonClassroomType] 
                                ([Id], [LessonId], [ClassroomTypeId])
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
                    INSERTED.[CurrentHours],
                    INSERTED.[AmountOfHours],
                    INSERTED.[LessonTypeId],
                    INSERTED.[TeacherId]
                    WHERE [Id] = '{request.Id}' ;",
                request.dto
            );

            lesson!.LessonType = lessonType.Value;
            lesson.Teacher = teacher.Value;

            AddToHistoryHandler addToHistoryHandler = new AddToHistoryHandler(_dbService);
            AddToHistoryDTO addToHistoryDTO = new AddToHistoryDTO
            {
                Id = Guid.NewGuid(),
                Timestamp = DateTime.Now,
                AffectedEntityId = request.Id,
                AffectedEntity = "Lesson",
                Command = "Patch",
                ReversaleDTO = JsonSerializer.Serialize(reversaleDTOLesson),
                CollisionChecked = false,
                ScheduleId = originalLesson.Value.LessonType.ScheduleId,
            };
            AddToHistoryCommand addToHistoryCommand = new AddToHistoryCommand(addToHistoryDTO);
            await addToHistoryHandler.Handle(addToHistoryCommand, cancellationToken);

            return lesson;
        }
    }
}
