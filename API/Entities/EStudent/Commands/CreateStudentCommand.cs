using AlpimiAPI.Database;
using AlpimiAPI.Entities.EGroup;
using AlpimiAPI.Entities.EGroup.Queries;
using AlpimiAPI.Entities.EStudent.DTO;
using AlpimiAPI.Responses;
using alpimi_planner_backend.API.Locales;
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

            return insertedId;
        }
    }
}
