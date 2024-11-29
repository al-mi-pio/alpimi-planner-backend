using System;
using AlpimiAPI.Database;
using AlpimiAPI.Entities.EGroup;
using AlpimiAPI.Entities.EGroup.Queries;
using AlpimiAPI.Entities.EStudent.DTO;
using AlpimiAPI.Entities.EStudentSubgroup.Commands;
using AlpimiAPI.Entities.EStudentSubgroup.DTO;
using AlpimiAPI.Entities.ESubgroup;
using AlpimiAPI.Entities.ESubgroup.Queries;
using AlpimiAPI.Locales;
using AlpimiAPI.Responses;
using MediatR;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Localization;

namespace AlpimiAPI.Entities.EStudent.Commands
{
    public record CreateStudentCommand(Guid Id, CreateStudentDTO dto, Guid FilteredId, string Role)
        : IRequest<Guid>;

    public class CreateStudentHandler : IRequestHandler<CreateStudentCommand, Guid>
    {
        private readonly IDbService _dbService;
        private readonly IStringLocalizer<Errors> _str;

        public CreateStudentHandler(IDbService dbService, IStringLocalizer<Errors> str)
        {
            _dbService = dbService;
            _str = str;
        }

        public async Task<Guid> Handle(
            CreateStudentCommand request,
            CancellationToken cancellationToken
        )
        {
            GetGroupHandler getGroupHandler = new GetGroupHandler(_dbService);
            GetGroupQuery getGroupQuery = new GetGroupQuery(
                request.dto.GroupId,
                request.FilteredId,
                request.Role
            );
            ActionResult<Group?> group = await getGroupHandler.Handle(
                getGroupQuery,
                cancellationToken
            );

            if (group.Value == null)
            {
                throw new ApiErrorException([new ErrorObject(_str["notFound", "Group"])]);
            }

            var studentAlbum = await _dbService.GetAll<Student>(
                $@"
                    SELECT 
                    st.[Id]
                    FROM [Student] st
                    INNER JOIN [Group] g on g.[Id] = st.[GroupId]
                    WHERE [AlbumNumber] = @AlbumNumber AND g.[ScheduleId] = '{group .Value .ScheduleId}';",
                request.dto
            );

            if (studentAlbum!.Any())
            {
                throw new ApiErrorException(
                    [new ErrorObject(_str["alreadyExists", "Student", request.dto.AlbumNumber])]
                );
            }

            List<ErrorObject> errors = new List<ErrorObject>();
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

                foreach (var subgroupId in request.dto.SubgroupIds)
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
                    else if (subgroup.Value.GroupId != request.dto.GroupId)
                    {
                        errors.Add(
                            new ErrorObject(_str["notFound", $"Subgroup Id = {subgroupId}"])
                        );
                    }
                }
            }
            if (errors.Count != 0)
            {
                throw new ApiErrorException(errors);
            }

            var insertedId = await _dbService.Post<Guid>(
                $@"
                    INSERT INTO [Student] 
                    ([Id],[AlbumNumber],[GroupId])
                    OUTPUT 
                    INSERTED.Id                    
                    VALUES (
                    '{request.Id}',   
                    @AlbumNumber,
                    @GroupId);",
                request.dto
            );

            if (request.dto.SubgroupIds != null)
            {
                CreateStudentSubgroupDTO studentSubgroupDTO = new CreateStudentSubgroupDTO()
                {
                    StudentId = insertedId,
                    SubgroupIds = request.dto.SubgroupIds
                };
                CreateStudentSubgroupHandler createStudentSubgroupHandler =
                    new CreateStudentSubgroupHandler(_dbService, _str);
                CreateStudentSubgroupCommand createStudentSubgroupCommand =
                    new CreateStudentSubgroupCommand(
                        studentSubgroupDTO,
                        request.FilteredId,
                        request.Role
                    );
                await createStudentSubgroupHandler.Handle(
                    createStudentSubgroupCommand,
                    cancellationToken
                );
            }

            return insertedId;
        }
    }
}
