using AlpimiAPI.Database;
using AlpimiAPI.Entities.ELessonPeriod.DTO;
using AlpimiAPI.Entities.EScheduleSettings;
using AlpimiAPI.Entities.EScheduleSettings.Queries;
using AlpimiAPI.Responses;
using alpimi_planner_backend.API.Locales;
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
            if (request.dto.Finish < request.dto.Start)
            {
                throw new ApiErrorException([new ErrorObject(_str["scheduleTime"])]);
            }

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
                    [new ErrorObject(_str["notFound", "ScheduleSettings"])]
                );
            }

            var lessonPeriodOverlap = await _dbService.GetAll<Guid>(
                $@"
                    SELECT 
                    lp.[Id]
                    FROM [LessonPeriod] lp
                    INNER JOIN [ScheduleSettings] ss ON ss.[Id] = lp.[ScheduleSettingsId]
                    INNER JOIN [Schedule] s ON s.[Id]=ss.[ScheduleId]
                    WHERE s.[UserId] = '{request.FilteredId}' AND ss.[ScheduleId] = @ScheduleId
                    AND ((([Start] > @Start AND [Start] < @Finish) 
                    OR ([Finish] > @Start AND [Finish] < @Finish))
                    OR ([Start] = @Start AND [Finish] = @Finish));",
                request.dto
            );
            if (lessonPeriodOverlap!.Any())
            {
                throw new ApiErrorException([new ErrorObject(_str["timeOverlap"])]);
            }

            var insertedId = await _dbService.Post<Guid>(
                $@"
                    INSERT INTO [LessonPeriod] 
                    ([Id],[Start],[Finish],[ScheduleSettingsId])
                    OUTPUT 
                    INSERTED.Id                    
                    VALUES (
                    '{request.Id}',
                    @Start,
                    @Finish,
                    '{scheduleSettings.Value.Id}');",
                request.dto
            );

            return insertedId;
        }
    }
}
