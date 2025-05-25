using AlpimiAPI.Database;
using AlpimiAPI.Entities.ECollisionType.DTO;
using AlpimiAPI.Entities.ECollisionType.Queries;
using AlpimiAPI.Locales;
using AlpimiAPI.Responses;
using MediatR;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Localization;

namespace AlpimiAPI.Entities.ECollisionType.Commands
{
    public record UpdateCollisionTypeCommand(
        Guid Id,
        UpdateCollisionTypeDTO dto,
        Guid FilteredId,
        string Role
    ) : IRequest<CollisionType?>;

    public class UpdateCollisionTypeHandler
        : IRequestHandler<UpdateCollisionTypeCommand, CollisionType?>
    {
        private readonly IDbService _dbService;
        private readonly IStringLocalizer<Errors> _str;

        public UpdateCollisionTypeHandler(IDbService dbService, IStringLocalizer<Errors> str)
        {
            _dbService = dbService;
            _str = str;
        }

        public async Task<CollisionType?> Handle(
            UpdateCollisionTypeCommand request,
            CancellationToken cancellationToken
        )
        {
            if (request.dto.Weight != null)
            {
                if (request.dto.Weight > 1 || request.dto.Weight < 0)
                {
                    throw new ApiErrorException([new ErrorObject(_str["badParameter", "Weight"])]);
                }
            }

            GetCollisionTypeHandler getCollisionTypeHandler = new GetCollisionTypeHandler(
                _dbService
            );
            GetCollisionTypeQuery getCollisionTypeQuery = new GetCollisionTypeQuery(
                request.Id,
                request.FilteredId,
                request.Role
            );
            ActionResult<CollisionType?> originalCollisionType =
                await getCollisionTypeHandler.Handle(getCollisionTypeQuery, cancellationToken);

            if (originalCollisionType.Value == null)
            {
                return null;
            }

            request.dto.Name = request.dto.Name ?? originalCollisionType.Value!.Name;
            request.dto.Description =
                request.dto.Description ?? originalCollisionType.Value!.Description;
            request.dto.Weight = request.dto.Weight ?? originalCollisionType.Value!.Weight;
            request.dto.Filter = request.dto.Filter ?? originalCollisionType.Value!.Filter;

            var collisionTypeName = await _dbService.GetAll<CollisionType>(
                $@"
                    SELECT 
                    [Id]
                    FROM [CollisionType] 
                    WHERE [Name] = @Name AND [ScheduleId] = '{originalCollisionType .Value .ScheduleId}' AND [Id] != '{request.Id}';",
                request.dto
            );

            if (collisionTypeName!.Any())
            {
                throw new ApiErrorException(
                    [new ErrorObject(_str["alreadyExists", "CollisionType", request.dto.Name])]
                );
            }

            var collisionType = await _dbService.Update<CollisionType?>(
                $@"
                    UPDATE [CollisionType] 
                    SET
                    [Name] = @Name, [Description] = @Description, [Weight] = @Weight, [Filter] = @Filter 
                    OUTPUT
                    INSERTED.[Id],
                    INSERTED.[Name],
                    INSERTED.[Description],
                    INSERTED.[Weight],
                    INSERTED.[Filter],
                    INSERTED.[ScheduleId]
                    WHERE [Id] = '{request.Id}';",
                request.dto
            );

            collisionType!.Schedule = originalCollisionType.Value.Schedule;

            return collisionType;
        }
    }
}
