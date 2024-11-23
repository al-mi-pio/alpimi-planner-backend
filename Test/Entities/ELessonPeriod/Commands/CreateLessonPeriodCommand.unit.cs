using AlpimiAPI.Database;
using AlpimiAPI.Entities.ELessonPeriod.Commands;
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
    public class CreateLessonPeriodCommandUnit
    {
        private readonly Mock<IDbService> _dbService = new();
        private readonly Mock<IStringLocalizer<Errors>> _str;

        public CreateLessonPeriodCommandUnit()
        {
            _str = ResourceSetup.Setup();
        }

        [Fact]
        public async Task ThrowsErrorWhenWrongScheduleIdIsGiven()
        {
            var createLessonPeriodCommand = new CreateLessonPeriodCommand(
                new Guid(),
                MockData.GetCreateLessonPeriodDTODetails(new Guid()),
                new Guid(),
                "User"
            );

            var createLessonPeriodHandler = new CreateLessonPeriodHandler(
                _dbService.Object,
                _str.Object
            );

            var result = await Assert.ThrowsAsync<ApiErrorException>(
                async () =>
                    await createLessonPeriodHandler.Handle(
                        createLessonPeriodCommand,
                        new CancellationToken()
                    )
            );

            Assert.Equal(
                JsonConvert.SerializeObject(
                    new ErrorObject[] { new ErrorObject("ScheduleSettings was not found") }
                ),
                JsonConvert.SerializeObject(result.errors)
            );
        }

        [Fact]
        public async Task ThrowsErrorWhenLessonPeriodsOverlap()
        {
            var dto = MockData.GetCreateLessonPeriodDTODetails(new Guid());
            dto.Finish = new TimeOnly(10, 00, 00);
            var scheduleSettings = MockData.GetScheduleSettingsDetails();

            _dbService
                .Setup(s => s.GetAll<Guid>(It.IsAny<string>(), It.IsAny<object>()))
                .ReturnsAsync(new List<Guid> { new Guid(), new Guid() });
            _dbService
                .Setup(s => s.Get<ScheduleSettings>(It.IsAny<string>(), It.IsAny<object>()))
                .ReturnsAsync(scheduleSettings);

            var createLessonPeriodCommand = new CreateLessonPeriodCommand(
                new Guid(),
                dto,
                new Guid(),
                "User"
            );

            var createLessonPeriodHandler = new CreateLessonPeriodHandler(
                _dbService.Object,
                _str.Object
            );

            var result = await Assert.ThrowsAsync<ApiErrorException>(
                async () =>
                    await createLessonPeriodHandler.Handle(
                        createLessonPeriodCommand,
                        new CancellationToken()
                    )
            );

            Assert.Contains("Overlapping time", result.errors.First().message);
        }

        [Fact]
        public async Task ThrowsErrorWhenTimeStartIsAfterTimeEnd()
        {
            var dto = MockData.GetCreateLessonPeriodDTODetails(new Guid());
            dto.Start = new TimeOnly(23, 00, 00);

            var scheduleSettings = MockData.GetScheduleSettingsDetails();
            _dbService
                .Setup(s => s.Get<ScheduleSettings>(It.IsAny<string>(), It.IsAny<object>()))
                .ReturnsAsync(scheduleSettings);

            var createLessonPeriodCommand = new CreateLessonPeriodCommand(
                new Guid(),
                dto,
                new Guid(),
                "User"
            );

            var createLessonPeriodHandler = new CreateLessonPeriodHandler(
                _dbService.Object,
                _str.Object
            );

            var result = await Assert.ThrowsAsync<ApiErrorException>(
                async () =>
                    await createLessonPeriodHandler.Handle(
                        createLessonPeriodCommand,
                        new CancellationToken()
                    )
            );

            Assert.Equal(
                JsonConvert.SerializeObject(
                    new ErrorObject[]
                    {
                        new ErrorObject("The end time cannot happen before the start time")
                    }
                ),
                JsonConvert.SerializeObject(result.errors)
            );
        }
    }
}
