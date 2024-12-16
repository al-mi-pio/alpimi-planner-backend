using AlpimiAPI.Database;
using AlpimiAPI.Entities.ETeacher.DTO;
using AlpimiAPI.Entities.ETeacher.Queries;
using AlpimiAPI.Locales;
using AlpimiAPI.Responses;
using MediatR;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Localization;

namespace AlpimiAPI.Entities.ETeacher.Commands
{
    public record UpdateTeacherCommand(Guid Id, UpdateTeacherDTO dto, Guid FilteredId, string Role)
        : IRequest<Teacher?>;

    public class UpdateTeacherHandler : IRequestHandler<UpdateTeacherCommand, Teacher?>
    {
        private readonly IDbService _dbService;
        private readonly IStringLocalizer<Errors> _str;

        public UpdateTeacherHandler(IDbService dbService, IStringLocalizer<Errors> str)
        {
            _dbService = dbService;
            _str = str;
        }

        public async Task<Teacher?> Handle(
            UpdateTeacherCommand request,
            CancellationToken cancellationToken
        )
        {
            GetTeacherHandler getTeacherHandler = new GetTeacherHandler(_dbService);
            GetTeacherQuery getTeacherQuery = new GetTeacherQuery(
                request.Id,
                request.FilteredId,
                request.Role
            );
            ActionResult<Teacher?> originalTeacher = await getTeacherHandler.Handle(
                getTeacherQuery,
                cancellationToken
            );

            if (originalTeacher.Value == null)
            {
                return null;
            }

            request.dto.Name = request.dto.Name ?? originalTeacher.Value!.Name;
            request.dto.Surname = request.dto.Surname ?? originalTeacher.Value!.Surname;

            var teacherName = await _dbService.Get<Teacher>(
                $@"
                    SELECT 
                    [Id]
                    FROM [Teacher] 
                    WHERE [Name] = @Name AND [Surname] = @Surname  AND [ScheduleId] = '{originalTeacher .Value .ScheduleId}' AND [Id] != '{request.Id}';",
                request.dto
            );

            if (teacherName != null)
            {
                throw new ApiErrorException(
                    [
                        new ErrorObject(
                            _str[
                                "alreadyExists",
                                "Teacher",
                                request.dto.Name + " " + request.dto.Surname
                            ]
                        )
                    ]
                );
            }

            var teacher = await _dbService.Update<Teacher?>(
                $@"
                    UPDATE [Teacher] 
                    SET
                    [Name] = @Name, [Surname] = @Surname 
                    OUTPUT
                    INSERTED.[Id],
                    INSERTED.[Name],
                    INSERTED.[Surname],
                    INSERTED.[ScheduleId]
                    WHERE [Id] = '{request.Id}';",
                request.dto
            );

            teacher!.Schedule = originalTeacher.Value.Schedule;

            return teacher;
        }
    }
}
