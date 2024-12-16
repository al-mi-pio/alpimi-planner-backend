using AlpimiAPI.Database;
using AlpimiAPI.Entities.EGroup;
using AlpimiAPI.Entities.EGroup.Queries;
using AlpimiAPI.Entities.ESubgroup.DTO;
using AlpimiAPI.Entities.ESubgroup.Queries;
using AlpimiAPI.Locales;
using AlpimiAPI.Responses;
using MediatR;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Localization;

namespace AlpimiAPI.Entities.ESubgroup.Commands
{
    public record UpdateSubgroupCommand(
        Guid Id,
        UpdateSubgroupDTO dto,
        Guid FilteredId,
        string Role
    ) : IRequest<Subgroup?>;

    public class UpdateSubgroupHandler : IRequestHandler<UpdateSubgroupCommand, Subgroup?>
    {
        private readonly IDbService _dbService;
        private readonly IStringLocalizer<Errors> _str;

        public UpdateSubgroupHandler(IDbService dbService, IStringLocalizer<Errors> str)
        {
            _dbService = dbService;
            _str = str;
        }

        public async Task<Subgroup?> Handle(
            UpdateSubgroupCommand request,
            CancellationToken cancellationToken
        )
        {
            if (request.dto.StudentCount != null)
            {
                if (request.dto.StudentCount < 1)
                {
                    throw new ApiErrorException(
                        [new ErrorObject(_str["badParameter", "StudentCount"])]
                    );
                }
            }

            GetSubgroupHandler getSubgroupHandler = new GetSubgroupHandler(_dbService);
            GetSubgroupQuery getSubgroupQuery = new GetSubgroupQuery(
                request.Id,
                request.FilteredId,
                request.Role
            );

            ActionResult<Subgroup?> originalSubgroup = await getSubgroupHandler.Handle(
                getSubgroupQuery,
                cancellationToken
            );

            if (originalSubgroup.Value == null)
            {
                return null;
            }

            request.dto.Name = request.dto.Name ?? originalSubgroup.Value!.Name;
            request.dto.StudentCount =
                request.dto.StudentCount ?? originalSubgroup.Value!.StudentCount;

            GetGroupHandler getGroupHandler = new GetGroupHandler(_dbService);
            GetGroupQuery getGroupQuery = new GetGroupQuery(
                originalSubgroup.Value.GroupId,
                request.FilteredId,
                request.Role
            );

            ActionResult<Group?> group = await getGroupHandler.Handle(
                getGroupQuery,
                cancellationToken
            );

            if (group.Value!.StudentCount < request.dto.StudentCount)
            {
                throw new ApiErrorException([new ErrorObject(_str["tooManyStudents"])]);
            }

            var groupName = await _dbService.GetAll<Group>(
                $@"
                    SELECT 
                    [Id]
                    FROM [Group] 
                    WHERE [Name] = @Name AND [ScheduleId]='{group.Value.ScheduleId}';",
                request.dto
            );

            if (groupName!.Any())
            {
                throw new ApiErrorException(
                    [new ErrorObject(_str["alreadyExists", "Group", request.dto.Name])]
                );
            }

            var subgroupName = await _dbService.GetAll<Subgroup>(
                $@"
                    SELECT 
                    [Id]
                    FROM [Subgroup] 
                    WHERE [Name] = @Name AND [GroupId] = '{originalSubgroup .Value! .GroupId}' AND [Id] != '{request.Id}';",
                request.dto
            );

            if (subgroupName!.Any())
            {
                throw new ApiErrorException(
                    [new ErrorObject(_str["alreadyExists", "Subgroup", request.dto.Name])]
                );
            }

            var subgroup = await _dbService.Update<Subgroup?>(
                $@"
                    UPDATE [Subgroup] 
                    SET
                    [Name] = @Name, [StudentCount] = @StudentCount 
                    OUTPUT
                    INSERTED.[Id],
                    INSERTED.[Name],
                    INSERTED.[StudentCount],
                    INSERTED.[GroupId]
                    WHERE [Id] = '{request.Id}';",
                request.dto
            );

            subgroup!.Group = group.Value!;

            return subgroup;
        }
    }
}
