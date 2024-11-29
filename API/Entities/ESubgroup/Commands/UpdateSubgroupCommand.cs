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
            if (request.dto.StudentCount < 1)
            {
                throw new ApiErrorException(
                    [new ErrorObject(_str["badParameter", "StudentCount"])]
                );
            }

            var group = await _dbService.Get<Group?>(
                @"
                    SELECT
                    g.[Id], g.[Name],g.[StudentCount], [ScheduleId]
                    FROM [Group] g
                    INNER JOIN [Subgroup] sg ON sg.[GroupId]=g.[Id]
                    WHERE sg.[Id]=@Id;",
                request
            );

            if (group == null)
            {
                return null;
            }

            GetSubgroupHandler getSubgroupHandler = new GetSubgroupHandler(_dbService);
            GetSubgroupQuery getSubgroupQuery = new GetSubgroupQuery(
                request.Id,
                request.FilteredId,
                "Admin"
            );

            ActionResult<Subgroup?> originalSubgroup = await getSubgroupHandler.Handle(
                getSubgroupQuery,
                cancellationToken
            );

            request.dto.Name = request.dto.Name ?? originalSubgroup.Value!.Name;
            request.dto.StudentCount =
                request.dto.StudentCount ?? originalSubgroup.Value!.StudentCount;

            if (group.StudentCount < request.dto.StudentCount)
            {
                throw new ApiErrorException([new ErrorObject(_str["tooManyStudents"])]);
            }

            var groupName = await _dbService.GetAll<Group>(
                $@"
                    SELECT 
                    [Id]
                    FROM [Group] 
                    WHERE [Name] = @Name AND [ScheduleId]='{group.ScheduleId}';",
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
                    WHERE [Name] = @Name AND [GroupId] = '{originalSubgroup.Value!.GroupId}';",
                request.dto
            );

            if (subgroupName!.Any())
            {
                throw new ApiErrorException(
                    [new ErrorObject(_str["alreadyExists", "Subgroup", request.dto.Name])]
                );
            }

            Subgroup? subgroup;
            switch (request.Role)
            {
                case "Admin":
                    subgroup = await _dbService.Update<Subgroup?>(
                        $@"
                            UPDATE [Subgroup] 
                            SET
                            [Name]=CASE WHEN @Name IS NOT NULL THEN @Name ELSE [Name] END,
                            [StudentCount]=CASE WHEN @StudentCount IS NOT NULL THEN @StudentCount ELSE [StudentCount] END
                            OUTPUT
                            INSERTED.[Id],
                            INSERTED.[Name],
                            INSERTED.[StudentCount],
                            INSERTED.[GroupId]
                            WHERE [Id] = '{request.Id}';",
                        request.dto
                    );
                    break;
                default:
                    subgroup = await _dbService.Update<Subgroup?>(
                        $@"
                            UPDATE sg
                            SET
                            sg.[Name]=CASE WHEN @Name IS NOT NULL THEN @Name ELSE sg.[Name] END,
                            sg.[StudentCount]=CASE WHEN @StudentCount IS NOT NULL THEN @StudentCount ELSE sg.[StudentCount] END
                            OUTPUT
                            INSERTED.[Id],
                            INSERTED.[Name],
                            INSERTED.[StudentCount],
                            INSERTED.[GroupId]
                            FROM [Subgroup] sg
                            INNER JOIN [Group] g on g.[Id] = sg.[GroupId]
                            INNER JOIN [Schedule] s ON s.[Id] = g.[ScheduleId]
                            WHERE s.[UserId] = '{request.FilteredId}' AND sg.[Id] = '{request.Id}';",
                        request.dto
                    );
                    break;
            }

            if (subgroup != null)
            {
                GetGroupHandler getGroupHandler = new GetGroupHandler(_dbService);
                GetGroupQuery getGroupQuery = new GetGroupQuery(
                    subgroup.GroupId,
                    new Guid(),
                    "Admin"
                );
                ActionResult<Group?> toInsertGroup = await getGroupHandler.Handle(
                    getGroupQuery,
                    cancellationToken
                );
                subgroup.Group = toInsertGroup.Value!;
            }

            return subgroup;
        }
    }
}
