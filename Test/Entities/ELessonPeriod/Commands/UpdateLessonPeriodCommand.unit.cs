using AlpimiAPI.Database;
using AlpimiAPI.Entities.ELessonPeriod;
using AlpimiAPI.Entities.ELessonPeriod.Commands;
using AlpimiAPI.Entities.ELessonPerioid;
using AlpimiAPI.Entities.EScheduleSettings;
using AlpimiAPI.Responses;
using AlpimiTest.TestSetup;
using AlpimiTest.TestUtilities;
using alpimi_planner_backend.API.Locales;
using Microsoft.Extensions.Localization;
using Moq;
using Newtonsoft.Json;
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
            dto.Finish = new TimeOnly(10, 0, 0);

            var scheduleSettings = MockData.GetScheduleSettingsDetails();
            var lessonPeriod = MockData.GetLessonPeriodDetails();

            _dbService
                .Setup(s => s.Get<ScheduleSettings>(It.IsAny<string>(), It.IsAny<object>()))
                .ReturnsAsync(scheduleSettings);
            _dbService
                .Setup(s => s.GetAll<LessonPeriod>(It.IsAny<string>(), It.IsAny<object>()))
                .ReturnsAsync(new List<LessonPeriod> { MockData.GetLessonPeriodDetails() });
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

            Assert.Equal("Start time and end time cannot overlap", result.errors.First().message);
        }

        [Fact]
        public async Task ThrowsErrorWhenTimeStartIsAfterTimeEnd()
        {
            var dto = MockData.GetUpdateLessonPeriodDTODetails();
            dto.Finish = new TimeOnly(10, 00, 00);
            dto.Start = new TimeOnly(11, 00, 00);

            var scheduleSettings = MockData.GetScheduleSettingsDetails();
            _dbService
                .Setup(s => s.Get<ScheduleSettings>(It.IsAny<string>(), It.IsAny<object>()))
                .ReturnsAsync(scheduleSettings);

            var updateLessonPeriodCommand = new UpdateLessonPeriodCommand(
                new Guid(),
                dto,
                new Guid(),
                "User"
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

            Assert.Equal(
                JsonConvert.SerializeObject(
                    new ErrorObject[]
                    {
                        new ErrorObject("The end date cannot happen before the start date")
                    }
                ),
                JsonConvert.SerializeObject(result.errors)
            );
        }
    }
}
