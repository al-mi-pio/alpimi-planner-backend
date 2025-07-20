using AlpimiAPI.Database;

namespace AlpimiAPI.Utilities
{
    public static class LessonPeriodCount
    {
        public static async Task<int> Get(
            IDbService _dbService,
            Guid scheduleSettingsId,
            CancellationToken cancellationToken
        )
        {
            var lessonPeriodCount = await _dbService.Get<int>(
                $@"
                    SELECT 
                    count(*)
                    FROM [LessonPeriod] 
                    WHERE [ScheduleSettingsId] = '{scheduleSettingsId}';",
                ""
            );
            return lessonPeriodCount;
        }
    }
}
