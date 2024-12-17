using AlpimiAPI.Database;
using AlpimiAPI.Entities.EGroup.DTO;
using AlpimiAPI.Entities.EGroup.Queries;
using AlpimiAPI.Entities.ESubgroup;
using AlpimiAPI.Locales;
using AlpimiAPI.Responses;
using MediatR;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Localization;

namespace AlpimiAPI.Entities.EGroup.Commands
{
    public record UpdateGroupCommand(Guid Id, UpdateGroupDTO dto, Guid FilteredId, string Role)
        : IRequest<Group?>;

    public class UpdateGroupHandler : IRequestHandler<UpdateGroupCommand, Group?>
    {
        private readonly IDbService _dbService;
        private readonly IStringLocalizer<Errors> _str;

        public UpdateGroupHandler(IDbService dbService, IStringLocalizer<Errors> str)
        {
            _dbService = dbService;
            _str = str;
        }

        public async Task<Group?> Handle(
            UpdateGroupCommand request,
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

            GetGroupHandler getGroupHandler = new GetGroupHandler(_dbService);
            GetGroupQuery getGroupQuery = new GetGroupQuery(
                request.Id,
                request.FilteredId,
                request.Role
            );
            ActionResult<Group?> originalGroup = await getGroupHandler.Handle(
                getGroupQuery,
                cancellationToken
            );

            if (originalGroup.Value == null)
            {
                return null;
            }

            request.dto.Name = request.dto.Name ?? originalGroup.Value!.Name;
            request.dto.StudentCount =
                request.dto.StudentCount ?? originalGroup.Value!.StudentCount;

            var groupName = await _dbService.GetAll<Group>(
                $@"
                    SELECT 
                    [Id]
                    FROM [Group] 
                    WHERE [Name] = @Name AND [ScheduleId] = '{originalGroup .Value .ScheduleId}' AND [Id] != '{request.Id}';",
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
                    sg.[Id]
                    FROM [Subgroup] sg
                    INNER JOIN [Group] g ON g.[Id] = sg.[GroupId]
                    WHERE sg.[Name] = @Name AND g.[ScheduleId] = '{originalGroup .Value .ScheduleId}';",
                request.dto
            );

            if (subgroupName!.Any())
            {
                throw new ApiErrorException(
                    [new ErrorObject(_str["alreadyExists", "Subgroup", request.dto.Name])]
                );
            }

            var studentCount = await _dbService.GetAll<Guid>(
                $@"
                    SELECT 
                    [Id]
                    FROM [Subgroup] 
                    WHERE [StudentCount]>@StudentCount AND [GroupId] = '{originalGroup .Value! .Id}';",
                request.dto
            );

            if (studentCount!.Any())
            {
                throw new ApiErrorException([new ErrorObject(_str["tooManyStudents"])]);
            }

            var group = await _dbService.Update<Group?>(
                $@"
                    UPDATE [Group] 
                    SET
                    [Name] = @Name, [StudentCount] = @StudentCount 
                    OUTPUT
                    INSERTED.[Id],
                    INSERTED.[Name],
                    INSERTED.[StudentCount],
                    INSERTED.[ScheduleId]
                    WHERE [Id] = '{request.Id}';",
                request.dto
            );

            group!.Schedule = originalGroup.Value.Schedule;

            return group;
        }
    }
}
