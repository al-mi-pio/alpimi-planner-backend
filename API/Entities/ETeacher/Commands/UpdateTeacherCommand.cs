using AlpimiAPI.Database;
using AlpimiAPI.Entities.ESchedule;
using AlpimiAPI.Entities.ESchedule.Queries;
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
            var schedule = await _dbService.Get<Schedule?>(
                @"
                    SELECT
                    s.[Id], s.[Name], [UserId]
                    FROM [Schedule] s
                    INNER JOIN [Teacher] t ON t.[ScheduleId]=s.[Id]
                    WHERE t.[Id]=@Id;",
                request
            );

            if (schedule == null)
            {
                return null;
            }

            GetTeacherHandler getTeacherHandler = new GetTeacherHandler(_dbService);
            GetTeacherQuery getTeacherQuery = new GetTeacherQuery(
                request.Id,
                request.FilteredId,
                "Admin"
            );

            ActionResult<Teacher?> originalTeacher = await getTeacherHandler.Handle(
                getTeacherQuery,
                cancellationToken
            );

            request.dto.Name = request.dto.Name ?? originalTeacher.Value!.Name;
            request.dto.Surname = request.dto.Surname ?? originalTeacher.Value!.Surname;

            var teacherName = await _dbService.GetAll<Teacher>(
                $@"
                    SELECT 
                    [Id]
                    FROM [Teacher] 
                    WHERE [Name] = @Name AND [Surname] = @Surname  AND [ScheduleId] = '{schedule.Id}';",
                request.dto
            );

            if (teacherName!.Any())
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

            Teacher? teacher;
            switch (request.Role)
            {
                case "Admin":
                    teacher = await _dbService.Update<Teacher?>(
                        $@"
                            UPDATE [Teacher] 
                            SET
                            [Name]=CASE WHEN @Name IS NOT NULL THEN @Name ELSE [Name] END,
                            [Surname]=CASE WHEN @Surname IS NOT NULL THEN @Surname ELSE [Surname] END
                            OUTPUT
                            INSERTED.[Id],
                            INSERTED.[Name],
                            INSERTED.[Surname],
                            INSERTED.[ScheduleId]
                            WHERE [Id] = '{request.Id}';",
                        request.dto
                    );
                    break;
                default:
                    teacher = await _dbService.Update<Teacher?>(
                        $@"
                            UPDATE t
                            SET
                            t.[Name]=CASE WHEN @Name IS NOT NULL THEN @Name ELSE t.[Name] END,
                            [Surname]=CASE WHEN @Surname IS NOT NULL THEN @Surname ELSE [Surname] END
                            OUTPUT
                            INSERTED.[Id],
                            INSERTED.[Name],
                            INSERTED.[Surname],
                            INSERTED.[ScheduleId]
                            FROM [Teacher] t
                            INNER JOIN [Schedule] s ON s.[Id] = t.[ScheduleId]
                            WHERE s.[UserId] = '{request.FilteredId}' AND t.[Id] = '{request.Id}';",
                        request.dto
                    );
                    break;
            }

            if (teacher != null)
            {
                GetScheduleHandler getScheduleHandler = new GetScheduleHandler(_dbService);
                GetScheduleQuery getScheduleQuery = new GetScheduleQuery(
                    teacher.ScheduleId,
                    new Guid(),
                    "Admin"
                );
                ActionResult<Schedule?> toInsertSchedule = await getScheduleHandler.Handle(
                    getScheduleQuery,
                    cancellationToken
                );
                teacher.Schedule = toInsertSchedule.Value!;
            }

            return teacher;
        }
    }
}
