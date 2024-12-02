using AlpimiAPI.Database;
using AlpimiAPI.Entities.EGroup;
using AlpimiAPI.Entities.EGroup.Queries;
using AlpimiAPI.Entities.EStudent.DTO;
using AlpimiAPI.Entities.EStudent.Queries;
using AlpimiAPI.Entities.ESubgroup;
using AlpimiAPI.Entities.ESubgroup.Queries;
using AlpimiAPI.Locales;
using AlpimiAPI.Responses;
using MediatR;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Localization;

namespace AlpimiAPI.Entities.EStudent.Commands
{
    public record UpdateStudentCommand(Guid Id, UpdateStudentDTO dto, Guid FilteredId, string Role)
        : IRequest<Student?>;

    public class UpdateStudentHandler : IRequestHandler<UpdateStudentCommand, Student?>
    {
        private readonly IDbService _dbService;
        private readonly IStringLocalizer<Errors> _str;

        public UpdateStudentHandler(IDbService dbService, IStringLocalizer<Errors> str)
        {
            _dbService = dbService;
            _str = str;
        }

        public async Task<Student?> Handle(
            UpdateStudentCommand request,
            CancellationToken cancellationToken
        )
        {
            Group? group;
            switch (request.Role)
            {
                case "Admin":
                    group = await _dbService.Get<Group?>(
                        @"
                            SELECT
                            g.[Id], g.[Name],g.[StudentCount], g.[ScheduleId]
                            FROM [Group] g
                            INNER JOIN [Student] st ON st.[GroupId]=g.[Id]
                            WHERE st.[Id] = @Id;",
                        request
                    );
                    break;
                default:
                    group = await _dbService.Get<Group?>(
                        @"
                            SELECT
                            g.[Id], g.[Name],g.[StudentCount], g.[ScheduleId]
                            FROM [Group] g
                            INNER JOIN [Student] st ON st.[GroupId]=g.[Id]
                            INNER JOIN [Schedule] s ON s.[Id] = g.[ScheduleId]
                            WHERE st.[Id] = @Id AND s.[UserId] = @FilteredId;",
                        request
                    );
                    break;
            }

            if (group == null)
            {
                return null;
            }

            GetStudentHandler getStudentHandler = new GetStudentHandler(_dbService);
            GetStudentQuery getStudentQuery = new GetStudentQuery(
                request.Id,
                request.FilteredId,
                "User"
            );

            ActionResult<Student?> originalStudent = await getStudentHandler.Handle(
                getStudentQuery,
                cancellationToken
            );

            request.dto.AlbumNumber = request.dto.AlbumNumber ?? originalStudent.Value!.AlbumNumber;

            var studentAlbum = await _dbService.GetAll<Student>(
                $@"
                    SELECT 
                    st.[Id]
                    FROM [Student] st
                    INNER JOIN [Group] g on g.[Id] = st.[GroupId]
                    WHERE [AlbumNumber] = @AlbumNumber AND g.[ScheduleId] = '{group.ScheduleId}' AND st.[Id] != '{request.Id}';",
                request.dto
            );

            if (studentAlbum!.Any())
            {
                throw new ApiErrorException(
                    [new ErrorObject(_str["alreadyExists", "Student", request.dto.AlbumNumber])]
                );
            }

            if (request.dto.SubgroupIds != null)
            {
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

                List<ErrorObject> errors = new List<ErrorObject>();

                var subgroups = await _dbService.GetAll<Guid>(
                    $@"
                        SELECT
                        sg.[Id]
                        FROM [Subgroup] sg
                        LEFT JOIN [StudentSubgroup] ssg ON ssg.[SubgroupId] = sg.[Id]
                        LEFT JOIN [Student] st ON st.[Id] = ssg.[StudentId]
                        WHERE st.[Id] = @Id",
                    request
                );

                foreach (Guid subgroupId in request.dto.SubgroupIds)
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
                        errors.Add(
                            new ErrorObject(_str["notFound", $"Subgroup Id = {subgroupId}"])
                        );
                    }
                    else if (subgroup.Value.GroupId != group.Id)
                    {
                        errors.Add(
                            new ErrorObject(_str["notFound", $"Subgroup Id = {subgroupId}"])
                        );
                    }
                }

                if (errors.Any())
                {
                    throw new ApiErrorException(errors);
                }

                subgroups = subgroups ?? [];
                foreach (Guid subgroupId in request.dto.SubgroupIds)
                {
                    if (!subgroups.Contains(subgroupId))
                    {
                        await _dbService.Post<Guid>(
                            $@"
                                INSERT INTO [StudentSubgroup] 
                                ([Id],[StudentId],[SubgroupId])
                                OUTPUT 
                                INSERTED.Id                    
                                VALUES (
                                '{Guid.NewGuid()}',   
                                @Id,
                                '{subgroupId}');",
                            request
                        );
                    }
                }
                foreach (Guid subgroup in subgroups)
                {
                    if (!request.dto.SubgroupIds.Contains(subgroup))
                    {
                        await _dbService.Delete(
                            $@"
                                DELETE [StudentSubgroup] 
                                WHERE [StudentId] = @Id AND [SubgroupId] = '{subgroup}';",
                            request
                        );
                    }
                }
            }

            var student = await _dbService.Update<Student?>(
                $@"
                    UPDATE [Student] 
                    SET
                    [AlbumNumber]=CASE WHEN @AlbumNumber IS NOT NULL THEN @AlbumNumber ELSE [AlbumNumber] END
                    OUTPUT
                    INSERTED.[Id],
                    INSERTED.[AlbumNumber],
                    INSERTED.[GroupId]
                    WHERE [Id] = '{request.Id}';",
                request.dto
            );

            GetGroupHandler getGroupHandler = new GetGroupHandler(_dbService);
            GetGroupQuery getGroupQuery = new GetGroupQuery(student!.GroupId, new Guid(), "Admin");
            ActionResult<Group?> toInsertGroup = await getGroupHandler.Handle(
                getGroupQuery,
                cancellationToken
            );
            student.Group = toInsertGroup.Value!;

            return student;
        }
    }
}
