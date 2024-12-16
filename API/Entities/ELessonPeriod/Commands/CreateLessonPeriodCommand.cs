using AlpimiAPI.Database;
using AlpimiAPI.Entities.ELessonPeriod;
using AlpimiAPI.Entities.ELessonPeriod.DTO;
using AlpimiAPI.Entities.ELessonPeriod.Queries;
using AlpimiAPI.Entities.EScheduleSettings;
using AlpimiAPI.Entities.EScheduleSettings.Queries;
using AlpimiAPI.Locales;
using AlpimiAPI.Responses;
using MediatR;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Localization;

namespace AlpimiAPI.Entities.ELessonPeriod.Commands
{
    public record CreateLessonPeriodCommand(
        Guid Id,
        CreateLessonPeriodDTO dto,
        Guid FilteredId,
        string Role
    ) : IRequest<Guid>;

    public class CreateLessonPeriodHandler : IRequestHandler<CreateLessonPeriodCommand, Guid>
    {
        private readonly IDbService _dbService;
        private readonly IStringLocalizer<Errors> _str;

        public CreateLessonPeriodHandler(IDbService dbService, IStringLocalizer<Errors> str)
        {
            _dbService = dbService;
            _str = str;
        }

        public async Task<Guid> Handle(
            CreateLessonPeriodCommand request,
            CancellationToken cancellationToken
        )
        {
            GetScheduleSettingsByScheduleIdHandler getScheduleSettingsByScheduleIdHandler =
                new GetScheduleSettingsByScheduleIdHandler(_dbService);
            GetScheduleSettingsByScheduleIdQuery getScheduleSettingsByScheduleIdQuery =
                new GetScheduleSettingsByScheduleIdQuery(
                    request.dto.ScheduleId,
                    request.FilteredId,
                    request.Role
                );

            ActionResult<ScheduleSettings?> scheduleSettings =
                await getScheduleSettingsByScheduleIdHandler.Handle(
                    getScheduleSettingsByScheduleIdQuery,
                    cancellationToken
                );
            if (scheduleSettings.Value == null)
            {
                throw new ApiErrorException(
                    [
                        new ErrorObject(
                            _str["resourceNotFound", "ScheduleSettings", request.dto.ScheduleId]
                        )
                    ]
                );
            }

            GetAllLessonPeriodByScheduleHandler getAllLessonPeriodByScheduleHandler =
                new GetAllLessonPeriodByScheduleHandler(_dbService, _str);
            GetAllLessonPeriodByScheduleQuery getAllLessonPeriodByScheduleQuery =
                new GetAllLessonPeriodByScheduleQuery(
                    request.dto.ScheduleId,
                    request.FilteredId,
                    request.Role,
                    new PaginationParams(1440, 0, "Start", "ASC")
                );
            ActionResult<(IEnumerable<LessonPeriod>?, int)> allLessonsPeriods =
                await getAllLessonPeriodByScheduleHandler.Handle(
                    getAllLessonPeriodByScheduleQuery,
                    cancellationToken
                );

            if (allLessonsPeriods.Value.Item1 != null)
            {
                foreach (var lessonPeriod in allLessonsPeriods.Value.Item1)
                {
                    if (
                        request.dto.Start
                            > lessonPeriod.Start.AddMinutes(-scheduleSettings.Value.SchoolHour)
                        && request.dto.Start
                            < lessonPeriod.Start.AddMinutes(scheduleSettings.Value.SchoolHour)
                    )
                    {
                        throw new ApiErrorException(
                            [new ErrorObject(_str["timeOverlap", "LessonPeriod"])]
                        );
                    }
                }
            }

            var insertedId = await _dbService.Post<Guid>(
                $@"
                    INSERT INTO [LessonPeriod] 
                    ([Id],[Start],[ScheduleSettingsId])
                    OUTPUT 
                    INSERTED.Id                    
                    VALUES (
                    '{request.Id}',
                    @Start,
                    '{scheduleSettings.Value.Id}');",
                request.dto
            );

            return insertedId;
        }
    }
}
