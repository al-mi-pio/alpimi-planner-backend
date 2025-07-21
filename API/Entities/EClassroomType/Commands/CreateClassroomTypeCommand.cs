using AlpimiAPI.Database;
using AlpimiAPI.Entities.EClassroomType.DTO;
using AlpimiAPI.Entities.EDayOff.Commands;
using AlpimiAPI.Entities.EHistory.DTO;
using AlpimiAPI.Entities.ESchedule;
using AlpimiAPI.Entities.ESchedule.Queries;
using AlpimiAPI.Locales;
using AlpimiAPI.Responses;
using MediatR;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Localization;

namespace AlpimiAPI.Entities.EClassroomType.Commands
{
    public record CreateClassroomTypeCommand(
        Guid Id,
        CreateClassroomTypeDTO dto,
        Guid FilteredId,
        string Role
    ) : IRequest<Guid>;

    public class CreateClassroomTypeHandler : IRequestHandler<CreateClassroomTypeCommand, Guid>
    {
        private readonly IDbService _dbService;
        private readonly IStringLocalizer<Errors> _str;
        private readonly IStringLocalizer<Fields> _strFields;

        public CreateClassroomTypeHandler(
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
            CreateClassroomTypeCommand request,
            CancellationToken cancellationToken
        )
        {
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
                    [
                        new ErrorObject(
                            _str["resourceNotFound", _strFields["Schedule"], request.dto.ScheduleId]
                        )
                    ]
                );
            }

            var classroomTypeName = await _dbService.Get<ClassroomType>(
                @"
                    SELECT 
                    [Id]
                    FROM [ClassroomType] 
                    WHERE [Name] = @Name AND [ScheduleId] = @ScheduleId;",
                request.dto
            );

            if (classroomTypeName != null)
            {
                throw new ApiErrorException(
                    [
                        new FieldErrorObject(
                            "name",
                            _str["alreadyExists", _strFields["ClassroomType"], request.dto.Name!]
                        )
                    ]
                );
            }

            var insertedId = await _dbService.Post<Guid>(
                $@"
                    INSERT INTO [ClassroomType] 
                    ([Id], [Name], [ScheduleId])
                    OUTPUT 
                    INSERTED.Id                    
                    VALUES (
                    '{request.Id}',   
                    @Name,
                    @ScheduleId);",
                request.dto
            );

            AddToHistoryHandler addToHistoryHandler = new AddToHistoryHandler(_dbService);
            AddToHistoryDTO addToHistoryDTO = new AddToHistoryDTO
            {
                Id = Guid.NewGuid(),
                Timestamp = DateTime.Now,
                AffectedEntityId = insertedId,
                AffectedEntity = "ClassroomType",
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
