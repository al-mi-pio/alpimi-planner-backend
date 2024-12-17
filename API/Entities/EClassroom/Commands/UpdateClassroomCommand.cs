using AlpimiAPI.Database;
using AlpimiAPI.Entities.EClassroom.DTO;
using AlpimiAPI.Entities.EClassroom.Queries;
using AlpimiAPI.Entities.EClassroomType;
using AlpimiAPI.Entities.EClassroomType.Queries;
using AlpimiAPI.Locales;
using AlpimiAPI.Responses;
using MediatR;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Localization;

namespace AlpimiAPI.Entities.EClassroom.Commands
{
    public record UpdateClassroomCommand(
        Guid Id,
        UpdateClassroomDTO dto,
        Guid FilteredId,
        string Role
    ) : IRequest<Classroom?>;

    public class UpdateClassroomHandler : IRequestHandler<UpdateClassroomCommand, Classroom?>
    {
        private readonly IDbService _dbService;
        private readonly IStringLocalizer<Errors> _str;

        public UpdateClassroomHandler(IDbService dbService, IStringLocalizer<Errors> str)
        {
            _dbService = dbService;
            _str = str;
        }

        public async Task<Classroom?> Handle(
            UpdateClassroomCommand request,
            CancellationToken cancellationToken
        )
        {
            if (request.dto.Capacity != null)
            {
                if (request.dto.Capacity < 1)
                {
                    throw new ApiErrorException(
                        [new ErrorObject(_str["badParameter", "Capacity"])]
                    );
                }
            }

            GetClassroomHandler getClassroomHandler = new GetClassroomHandler(_dbService);
            GetClassroomQuery getClassroomQuery = new GetClassroomQuery(
                request.Id,
                request.FilteredId,
                request.Role
            );
            ActionResult<Classroom?> originalClassroom = await getClassroomHandler.Handle(
                getClassroomQuery,
                cancellationToken
            );

            if (originalClassroom.Value == null)
            {
                return null;
            }

            request.dto.Name = request.dto.Name ?? originalClassroom.Value!.Name;

            var classroomName = await _dbService.GetAll<Classroom>(
                $@"
                    SELECT 
                    [Id]
                    FROM [Classroom] 
                    WHERE [Name] = @Name AND [ScheduleId] = '{originalClassroom .Value .ScheduleId}' AND [Id] != '{request.Id}';",
                request.dto
            );

            if (classroomName!.Any())
            {
                throw new ApiErrorException(
                    [new ErrorObject(_str["alreadyExists", "Classroom", request.dto.Name])]
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
                    else if (classroomType.Value.ScheduleId != originalClassroom.Value.ScheduleId)
                    {
                        errors.Add(
                            new ErrorObject(
                                _str["wrongSet", "ClassroomType", "Schedule", "Classroom"]
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
                        LEFT JOIN [ClassroomClassroomType] cct ON cct.[ClassroomTypeId] = ct.[Id]
                        LEFT JOIN [Classroom] c ON c.[Id] = cct.[ClassroomId]
                        WHERE c.[Id] = @Id;",
                    request
                );

                classroomTypes = classroomTypes ?? [];
                foreach (Guid classroomTypeId in request.dto.ClassroomTypeIds)
                {
                    if (!classroomTypes.Contains(classroomTypeId))
                    {
                        await _dbService.Post<Guid>(
                            $@"
                                INSERT INTO [ClassroomClassroomType] 
                                ([Id], [ClassroomId], [ClassroomTypeId])
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
                                DELETE [ClassroomClassroomType] 
                                WHERE [ClassroomId] = @Id AND [ClassroomTypeId] = '{classroomType}';",
                            request
                        );
                    }
                }
            }

            var classroom = await _dbService.Update<Classroom?>(
                $@"
                    UPDATE [Classroom] 
                    SET
                    [Name] = @Name, [Capacity] = @Capacity
                    OUTPUT
                    INSERTED.[Id],
                    INSERTED.[Name],
                    INSERTED.[Capacity],
                    INSERTED.[ScheduleId]
                    WHERE [Id] = '{request.Id}';",
                request.dto
            );

            classroom!.Schedule = originalClassroom.Value.Schedule!;

            return classroom;
        }
    }
}
