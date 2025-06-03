using System.Text.Json;
using AlpimiAPI.Database;
using AlpimiAPI.Entities.EClassroomType.DTO;
using AlpimiAPI.Entities.EClassroomType.Queries;
using AlpimiAPI.Entities.EDayOff.Commands;
using AlpimiAPI.Entities.EHistory.DTO;
using AlpimiAPI.Locales;
using AlpimiAPI.Responses;
using MediatR;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Localization;

namespace AlpimiAPI.Entities.EClassroomType.Commands
{
    public record UpdateClassroomTypeCommand(
        Guid Id,
        UpdateClassroomTypeDTO dto,
        Guid FilteredId,
        string Role
    ) : IRequest<ClassroomType?>;

    public class UpdateClassroomTypeHandler
        : IRequestHandler<UpdateClassroomTypeCommand, ClassroomType?>
    {
        private readonly IDbService _dbService;
        private readonly IStringLocalizer<Errors> _str;

        public UpdateClassroomTypeHandler(IDbService dbService, IStringLocalizer<Errors> str)
        {
            _dbService = dbService;
            _str = str;
        }

        public async Task<ClassroomType?> Handle(
            UpdateClassroomTypeCommand request,
            CancellationToken cancellationToken
        )
        {
            GetClassroomTypeHandler getClassroomTypeHandler = new GetClassroomTypeHandler(
                _dbService
            );
            GetClassroomTypeQuery getClassroomTypeQuery = new GetClassroomTypeQuery(
                request.Id,
                request.FilteredId,
                request.Role
            );
            ActionResult<ClassroomType?> originalClassroomType =
                await getClassroomTypeHandler.Handle(getClassroomTypeQuery, cancellationToken);

            if (originalClassroomType.Value == null)
            {
                return null;
            }

            UpdateClassroomTypeDTO reversaleDTOClassroomType = new UpdateClassroomTypeDTO
            {
                Name = originalClassroomType.Value!.Name,
            };

            request.dto.Name = request.dto.Name ?? originalClassroomType.Value!.Name;

            var classroomTypeName = await _dbService.Get<ClassroomType>(
                $@"
                    SELECT 
                    [Id]
                    FROM [ClassroomType] 
                    WHERE [Name] = @Name AND [ScheduleId] = '{originalClassroomType .Value .ScheduleId}' AND [Id] != '{request.Id}';",
                request.dto
            );

            if (classroomTypeName != null)
            {
                throw new ApiErrorException(
                    [new ErrorObject(_str["alreadyExists", "ClassroomType", request.dto.Name])]
                );
            }

            var classroomType = await _dbService.Update<ClassroomType?>(
                $@"
                    UPDATE [ClassroomType] 
                    SET
                    [Name] = @Name
                    OUTPUT
                    INSERTED.[Id],
                    INSERTED.[Name],
                    INSERTED.[ScheduleId]
                    WHERE [Id] = '{request.Id}';",
                request.dto
            );

            classroomType!.Schedule = originalClassroomType.Value.Schedule!;

            AddToHistoryHandler addToHistoryHandler = new AddToHistoryHandler(_dbService);
            AddToHistoryDTO addToHistoryDTO = new AddToHistoryDTO
            {
                Id = Guid.NewGuid(),
                Timestamp = DateTime.Now,
                AffectedEntityId = request.Id,
                AffectedEntity = "ClassroomType",
                Command = "Patch",
                ReversaleDTO = JsonSerializer.Serialize(reversaleDTOClassroomType),
                CollisionChecked = false,
                ScheduleId = originalClassroomType.Value.ScheduleId,
            };
            AddToHistoryCommand addToHistoryCommand = new AddToHistoryCommand(addToHistoryDTO);
            await addToHistoryHandler.Handle(addToHistoryCommand, cancellationToken);

            return classroomType;
        }
    }
}
