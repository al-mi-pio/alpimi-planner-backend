using AlpimiAPI.Database;
using AlpimiAPI.Entities.ELessonPeriod;
using AlpimiAPI.Entities.ELessonPeriod.Commands;
using AlpimiAPI.Entities.EScheduleSettings;
using AlpimiAPI.Locales;
using AlpimiAPI.Responses;
using AlpimiTest.TestSetup;
using AlpimiTest.TestUtilities;
using Microsoft.Extensions.Localization;
using Moq;
using Xunit;

namespace AlpimiTest.Entities.ELessonPeriod.Commands
{
    [Collection("Sequential Tests")]
    public class UpdateLessonPeriodCommandUnit
    {
        private readonly Mock<IDbService> _dbService = new();
        private readonly Mock<IStringLocalizer<Errors>> _str;

        public UpdateLessonPeriodCommandUnit()
        {
            _str = ResourceSetup.Setup();
        }

        [Fact]
        public async Task ThrowsErrorWhenLessonPeriodsOverlap()
        {
            var dto = MockData.GetUpdateLessonPeriodDTODetails();
            var scheduleSettings = MockData.GetScheduleSettingsDetails();
            var lessonPeriod = MockData.GetLessonPeriodDetails();
            _dbService
                .Setup(s => s.Get<ScheduleSettings>(It.IsAny<string>(), It.IsAny<object>()))
                .ReturnsAsync(scheduleSettings);
            _dbService
                .Setup(s => s.GetAll<LessonPeriod>(It.IsAny<string>(), It.IsAny<object>()))
                .ReturnsAsync(
                    new List<LessonPeriod>
                    {
                        MockData.GetLessonPeriodDetails(),
                        MockData.GetLessonPeriodDetails()
                    }
                );
            _dbService
                .Setup(s => s.Get<LessonPeriod>(It.IsAny<string>(), It.IsAny<object>()))
                .ReturnsAsync(MockData.GetLessonPeriodDetails());

            var updateLessonPeriodCommand = new UpdateLessonPeriodCommand(
                new Guid(),
                dto,
                new Guid(),
                "Admin"
            );
            var updateLessonPeriodHandler = new UpdateLessonPeriodHandler(
                _dbService.Object,
                _str.Object
            );
            var result = await Assert.ThrowsAsync<ApiErrorException>(
                async () =>
                    await updateLessonPeriodHandler.Handle(
                        updateLessonPeriodCommand,
                        new CancellationToken()
                    )
            );

            Assert.Equal("LessonPeriods cannot overlap", result.errors.First().message);
        }
    }
}
