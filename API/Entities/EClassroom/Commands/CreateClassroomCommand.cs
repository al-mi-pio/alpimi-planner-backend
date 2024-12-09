using AlpimiAPI.Database;
using AlpimiAPI.Entities.EClassroom.DTO;
using AlpimiAPI.Entities.EClassroomType;
using AlpimiAPI.Entities.EClassroomType.Queries;
using AlpimiAPI.Entities.ESchedule;
using AlpimiAPI.Entities.ESchedule.Queries;
using AlpimiAPI.Locales;
using AlpimiAPI.Responses;
using MediatR;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Localization;

namespace AlpimiAPI.Entities.EClassroom.Commands
{
    public record CreateClassroomCommand(
        Guid Id,
        CreateClassroomDTO dto,
        Guid FilteredId,
        string Role
    ) : IRequest<Guid>;

    public class CreateClassroomHandler : IRequestHandler<CreateClassroomCommand, Guid>
    {
        private readonly IDbService _dbService;
        private readonly IStringLocalizer<Errors> _str;

        public CreateClassroomHandler(IDbService dbService, IStringLocalizer<Errors> str)
        {
            _dbService = dbService;
            _str = str;
        }

        public async Task<Guid> Handle(
            CreateClassroomCommand request,
            CancellationToken cancellationToken
        )
        {
            if (request.dto.Capacity < 1)
            {
                throw new ApiErrorException([new ErrorObject(_str["badParameter", "Capacity"])]);
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

            var classroomName = await _dbService.Get<Classroom>(
                @"
                    SELECT 
                    [Id]
                    FROM [Classroom] 
                    WHERE [Name] = @Name AND [ScheduleId] = @ScheduleId;",
                request.dto
            );

            if (classroomName != null)
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
                }
                if (errors.Count != 0)
                {
                    throw new ApiErrorException(errors);
                }
            }

            var insertedId = await _dbService.Post<Guid>(
                $@"
                    INSERT INTO [Classroom] 
                    ([Id],[Name],[Capacity],[ScheduleId])
                    OUTPUT 
                    INSERTED.Id                    
                    VALUES (
                    '{request.Id}',   
                    @Name,
                    @Capacity,
                    @ScheduleId);",
                request.dto
            );

            if (request.dto.ClassroomTypeIds != null)
            {
                foreach (Guid classroomTypeId in request.dto.ClassroomTypeIds)
                {
                    await _dbService.Post<Guid>(
                        $@"
                            INSERT INTO [ClassroomClassroomType] 
                            ([Id],[ClassroomId],[ClassroomTypeId])
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

            return insertedId;
        }
    }
}
