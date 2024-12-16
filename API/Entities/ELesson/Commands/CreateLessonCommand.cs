using AlpimiAPI.Database;
using AlpimiAPI.Entities.EClassroomType;
using AlpimiAPI.Entities.EClassroomType.Queries;
using AlpimiAPI.Entities.ELesson.DTO;
using AlpimiAPI.Entities.ELessonType;
using AlpimiAPI.Entities.ELessonType.Queries;
using AlpimiAPI.Entities.ESubgroup;
using AlpimiAPI.Entities.ESubgroup.Queries;
using AlpimiAPI.Locales;
using AlpimiAPI.Responses;
using MediatR;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Localization;

namespace AlpimiAPI.Entities.ELesson.Commands
{
    public record CreateLessonCommand(Guid Id, CreateLessonDTO dto, Guid FilteredId, string Role)
        : IRequest<Guid>;

    public class CreateLessonHandler : IRequestHandler<CreateLessonCommand, Guid>
    {
        private readonly IDbService _dbService;
        private readonly IStringLocalizer<Errors> _str;

        public CreateLessonHandler(IDbService dbService, IStringLocalizer<Errors> str)
        {
            _dbService = dbService;
            _str = str;
        }

        public async Task<Guid> Handle(
            CreateLessonCommand request,
            CancellationToken cancellationToken
        )
        {
            List<ErrorObject> errors = new List<ErrorObject>();
            if (request.dto.AmountOfHours < 1)
            {
                errors.Add(new ErrorObject(_str["badParameter", "AmountOfHours"]));
            }

            GetLessonTypeHandler getLessonTypeHandler = new GetLessonTypeHandler(_dbService);
            GetLessonTypeQuery getLessonTypeQuery = new GetLessonTypeQuery(
                request.dto.LessonTypeId,
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

            GetSubgroupHandler getSubgroupHandler = new GetSubgroupHandler(_dbService);
            GetSubgroupQuery getSubgroupQuery = new GetSubgroupQuery(
                request.dto.SubgroupId,
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
                    new ErrorObject(_str["resourceNotFound", "Subgroup", request.dto.SubgroupId])
                );
            }

            if (errors.Count != 0)
            {
                throw new ApiErrorException(errors);
            }

            if (subgroup.Value!.Group.ScheduleId != lessonType.Value!.ScheduleId)
            {
                throw new ApiErrorException(
                    [new ErrorObject(_str["wrongSet", "Subgroup", "Schedule", "LessonType"])]
                );
            }

            var lessonName = await _dbService.GetAll<Lesson>(
                @"
                    SELECT 
                    [Id]
                    FROM [Lesson] 
                    WHERE [Name] = @Name AND [SubgroupId] = @SubgroupId; ",
                request.dto
            );

            if (lessonName!.Any())
            {
                throw new ApiErrorException(
                    [new ErrorObject(_str["alreadyExists", "Lesson", request.dto.Name])]
                );
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
                    else if (classroomType.Value.ScheduleId != lessonType.Value.ScheduleId)
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
            }

            var insertedId = await _dbService.Post<Guid>(
                $@"
                    INSERT INTO [Lesson] 
                    ([Id], [Name], [CurrentHours], [AmountOfHours], [LessonTypeId], [SubgroupId])
                    OUTPUT 
                    INSERTED.Id                    
                    VALUES (
                    '{request.Id}',   
                    @Name,
                    0,
                    @AmountOfHours,
                    @LessonTypeId,
                    @SubgroupId); ",
                request.dto
            );

            if (request.dto.ClassroomTypeIds != null)
            {
                foreach (Guid classroomTypeId in request.dto.ClassroomTypeIds)
                {
                    await _dbService.Post<Guid>(
                        $@"
                            INSERT INTO [LessonClassroomType] 
                            ([Id], [LessonId], [ClassroomTypeId])
                            OUTPUT 
                            INSERTED.Id                    
                            VALUES (
                            '{Guid.NewGuid()}',   
                            '{insertedId}',
                            '{classroomTypeId}'); ",
                        ""
                    );
                }
            }

            return insertedId;
        }
    }
}
