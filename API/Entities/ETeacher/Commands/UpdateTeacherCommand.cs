using System.Text.Json;
using System.Text.RegularExpressions;
using AlpimiAPI.Database;
using AlpimiAPI.Entities.EDayOff.Commands;
using AlpimiAPI.Entities.EHistory.DTO;
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
            if (request.dto.Email != null)
            {
                if (!Regex.IsMatch(request.dto.Email, @"^[^@\s]+@[^@\s]+\.[^@\s]+$"))
                {
                    throw new ApiErrorException(
                        [new FieldErrorObject("email", _str["badParameter", "Email"])]
                    );
                }
            }

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

            UpdateTeacherDTO reversaleDTOTeacher = new UpdateTeacherDTO
            {
                Name = originalTeacher.Value!.Name,
                Surname = originalTeacher.Value!.Surname,
                Email = originalTeacher.Value!.Email,
            };

            request.dto.Name = request.dto.Name ?? originalTeacher.Value!.Name;
            request.dto.Surname = request.dto.Surname ?? originalTeacher.Value!.Surname;
            request.dto.Email = request.dto.Email ?? originalTeacher.Value!.Email;

            var teacherEmail = await _dbService.Get<Teacher>(
                $@"
                    SELECT 
                    [Id]
                    FROM [Teacher] 
                    WHERE [Email] = @Email  AND [ScheduleId] = '{originalTeacher .Value .ScheduleId}' AND [Id] != '{request.Id}';",
                request.dto
            );

            if (teacherEmail != null)
            {
                throw new ApiErrorException(
                    [
                        new FieldErrorObject(
                            "email",
                            _str["alreadyExists", "Teacher", request.dto.Email]
                        )
                    ]
                );
            }

            var teacher = await _dbService.Update<Teacher?>(
                $@"
                    UPDATE [Teacher] 
                    SET
                    [Name] = @Name, [Surname] = @Surname , [Email] = @Email
                    OUTPUT
                    INSERTED.[Id],
                    INSERTED.[Name],
                    INSERTED.[Email],
                    INSERTED.[Surname],
                    INSERTED.[ScheduleId]
                    WHERE [Id] = '{request.Id}';",
                request.dto
            );

            teacher!.Schedule = originalTeacher.Value.Schedule;

            AddToHistoryHandler addToHistoryHandler = new AddToHistoryHandler(_dbService);
            AddToHistoryDTO addToHistoryDTO = new AddToHistoryDTO
            {
                Id = Guid.NewGuid(),
                Timestamp = DateTime.Now,
                AffectedEntityId = request.Id,
                AffectedEntity = "Teacher",
                Command = "Patch",
                ReversaleDTO = JsonSerializer.Serialize(reversaleDTOTeacher),
                CollisionChecked = false,
                ScheduleId = originalTeacher.Value.ScheduleId,
            };
            AddToHistoryCommand addToHistoryCommand = new AddToHistoryCommand(addToHistoryDTO);
            await addToHistoryHandler.Handle(addToHistoryCommand, cancellationToken);

            return teacher;
        }
    }
}
