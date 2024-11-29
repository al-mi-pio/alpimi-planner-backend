using AlpimiAPI.Database;
using AlpimiAPI.Entities.EGroup;
using AlpimiAPI.Entities.EGroup.Queries;
using AlpimiAPI.Entities.EStudent.DTO;
using AlpimiAPI.Entities.EStudent.Queries;
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
            var group = await _dbService.Get<Group?>(
                @"
                    SELECT
                    g.[Id], g.[Name],g.[StudentCount], [ScheduleId]
                    FROM [Group] g
                    INNER JOIN [Student] sg ON sg.[GroupId]=g.[Id]
                    WHERE sg.[Id]=@Id;",
                request
            );

            if (group == null)
            {
                return null;
            }

            GetStudentHandler getStudentHandler = new GetStudentHandler(_dbService);
            GetStudentQuery getStudentQuery = new GetStudentQuery(
                request.Id,
                request.FilteredId,
                "Admin"
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
                    WHERE [AlbumNumber] = @AlbumNumber AND g.[ScheduleId] = '{group.ScheduleId}';",
                request.dto
            );

            if (studentAlbum!.Any())
            {
                throw new ApiErrorException(
                    [new ErrorObject(_str["alreadyExists", "Student", request.dto.AlbumNumber])]
                );
            }

            Student? student;
            switch (request.Role)
            {
                case "Admin":
                    student = await _dbService.Update<Student?>(
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
                    break;
                default:
                    student = await _dbService.Update<Student?>(
                        $@"
                            UPDATE st
                            SET
                            st.[AlbumNumber]=CASE WHEN @AlbumNumber IS NOT NULL THEN @AlbumNumber ELSE st.[AlbumNumber] END
                            OUTPUT
                            INSERTED.[Id],
                            INSERTED.[AlbumNumber],
                            INSERTED.[GroupId]
                            FROM [Student] st
                            INNER JOIN [Group] g on g.[Id] = st.[GroupId]
                            INNER JOIN [Schedule] s ON s.[Id] = g.[ScheduleId]
                            WHERE s.[UserId] = '{request.FilteredId}' AND st.[Id] = '{request.Id}';",
                        request.dto
                    );
                    break;
            }

            if (student != null)
            {
                GetGroupHandler getGroupHandler = new GetGroupHandler(_dbService);
                GetGroupQuery getGroupQuery = new GetGroupQuery(
                    student.GroupId,
                    new Guid(),
                    "Admin"
                );
                ActionResult<Group?> toInsertGroup = await getGroupHandler.Handle(
                    getGroupQuery,
                    cancellationToken
                );
                student.Group = toInsertGroup.Value!;
            }

            return student;
        }
    }
}
