using AlpimiAPI.Database;
using AlpimiAPI.Entities.EClassroomType;
using AlpimiAPI.Entities.EClassroomType.Queries;
using AlpimiAPI.Entities.ELesson.DTO;
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

            GetTeacherHandler getTeacherHandler = new GetTeacherHandler(_dbService);
            GetTeacherQuery getTeacherQuery = new GetTeacherQuery(
                request.dto.TeacherId,
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
                    errors.Add(new ErrorObject(_str["resourceNotFound", "Subgroup", subgroupId]));
                }
                else if (
                    lessonType.Value != null
                    && subgroup.Value.Group.ScheduleId != lessonType.Value.ScheduleId
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

            if (teacher.Value!.ScheduleId != lessonType.Value!.ScheduleId)
            {
                throw new ApiErrorException(
                    [new ErrorObject(_str["wrongSet", "Teacher", "Schedule", "LessonType"])]
                );
            }

            var lessonName = await _dbService.GetAll<Lesson>(
                @"
                    SELECT 
                    [Id]
                    FROM [Lesson] 
                    WHERE [Name] = @Name AND [TeacherId] = @TeacherId;",
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
                duplicates = request
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
                    ([Id], [Name], [CurrentHours], [AmountOfHours], [LessonTypeId], [TeacherId])
                    OUTPUT 
                    INSERTED.Id                    
                    VALUES (
                    '{request.Id}',   
                    @Name,
                    0,
                    @AmountOfHours,
                    @LessonTypeId,
                    @TeacherId);",
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
                            '{classroomTypeId}');",
                        ""
                    );
                }
            }
            foreach (Guid subgroupId in request.dto.SubgroupIds)
            {
                await _dbService.Post<Guid>(
                    $@"
                        INSERT INTO [LessonSubgroup] 
                        ([Id], [LessonId], [SubgroupId])
                        OUTPUT 
                        INSERTED.Id                    
                        VALUES (
                        '{Guid.NewGuid()}',   
                        '{insertedId}',
                        '{subgroupId}');",
                    ""
                );
            }

            return insertedId;
        }
    }
}
