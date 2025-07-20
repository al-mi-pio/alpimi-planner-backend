using AlpimiAPI.Database;
using AlpimiAPI.Entities.EClassroom.DTO;
using AlpimiAPI.Entities.EClassroomType;
using AlpimiAPI.Entities.EClassroomType.Queries;
using AlpimiAPI.Entities.EDayOff.Commands;
using AlpimiAPI.Entities.EHistory.DTO;
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
        private readonly IStringLocalizer<Fields> _strFields;

        public CreateClassroomHandler(
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
            CreateClassroomCommand request,
            CancellationToken cancellationToken
        )
        {
            if (request.dto.Capacity < 1)
            {
                throw new ApiErrorException(
                    [new FieldErrorObject("capacity", _str["badParameter", _strFields["Capacity"]])]
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
                    [
                        new FieldErrorObject(
                            "name",
                            _str["alreadyExists", _strFields["Classroom"], request.dto.Name!]
                        )
                    ]
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
                            new FieldErrorObject(
                                "classroomType",
                                _str["duplicateData", _strFields["ClassroomType"], duplicate]
                            )
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
                    else if (classroomType.Value.ScheduleId != request.dto.ScheduleId)
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
            }

            var insertedId = await _dbService.Post<Guid>(
                $@"
                    INSERT INTO [Classroom] 
                    ([Id], [Name], [Capacity], [ScheduleId])
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
                            ([Id], [ClassroomId], [ClassroomTypeId])
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

            AddToHistoryHandler addToHistoryHandler = new AddToHistoryHandler(_dbService);
            AddToHistoryDTO addToHistoryDTO = new AddToHistoryDTO
            {
                Id = Guid.NewGuid(),
                Timestamp = DateTime.Now,
                AffectedEntityId = insertedId,
                AffectedEntity = "Classroom",
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
