using AlpimiAPI.Database;
using AlpimiAPI.Entities.EDayOff.Commands;
using AlpimiAPI.Entities.EScheduleSettings;
using AlpimiAPI.Responses;
using AlpimiTest.TestSetup;
using AlpimiTest.TestUtilities;
using alpimi_planner_backend.API.Locales;
using Microsoft.Extensions.Localization;
using Moq;
using Newtonsoft.Json;
using Xunit;

namespace AlpimiTest.Entities.EDayOff.Commands
{
    [Collection("Sequential Tests")]
    public class CreateMultipleDayOffCommandUnit
    {
        private readonly Mock<IDbService> _dbService = new();
        private readonly Mock<IStringLocalizer<Errors>> _str;

        public CreateMultipleDayOffCommandUnit()
        {
            _str = ResourceSetup.Setup();
        }

        [Fact]
        public async Task ThrowsErrorWhenWrongScheduleIdIsGiven()
        {
            var createMultipleDayOffCommand = new CreateMultipleDayOffCommand(
                new Guid(),
                "Bob",
                new DateTime(2000, 1, 1),
                new DateTime(2000, 1, 2),
                new Guid(),
                new Guid(),
                "User"
            );

            var createMultipleDayOffHandler = new CreateMultipleDayOffHandler(
                _dbService.Object,
                _str.Object
            );

            var result = await Assert.ThrowsAsync<ApiErrorException>(
                async () =>
                    await createMultipleDayOffHandler.Handle(
                        createMultipleDayOffCommand,
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
        public async Task ThrowsErrorWhenWrongDateIsProvided()
        {
            var scheduleSettings = MockData.GetScheduleSettingsDetails();
            _dbService
                .Setup(s => s.Get<ScheduleSettings>(It.IsAny<string>(), It.IsAny<object>()))
                .ReturnsAsync(scheduleSettings);

            var createMultipleDayOffCommand = new CreateMultipleDayOffCommand(
                new Guid(),
                "Bob",
                new DateTime(2000, 1, 1),
                new DateTime(2000, 1, 2),
                new Guid(),
                new Guid(),
                "User"
            );

            var createMultipleDayOffHandler = new CreateMultipleDayOffHandler(
                _dbService.Object,
                _str.Object
            );

            var result = await Assert.ThrowsAsync<ApiErrorException>(
                async () =>
                    await createMultipleDayOffHandler.Handle(
                        createMultipleDayOffCommand,
                        new CancellationToken()
                    )
            );

            Assert.Contains("Date must be in between", result.errors.First().message);
        }

        [Fact]
        public async Task ThrowsErrorWhenDateIsIncorrect()
        {
            var scheduleSettings = MockData.GetScheduleSettingsDetails();
            _dbService
                .Setup(s => s.Get<ScheduleSettings>(It.IsAny<string>(), It.IsAny<object>()))
                .ReturnsAsync(scheduleSettings);

            var createMultipleDayOffCommand = new CreateMultipleDayOffCommand(
                new Guid(),
                "Bob",
                new DateTime(2000, 1, 1),
                new DateTime(1999, 1, 1),
                new Guid(),
                new Guid(),
                "User"
            );

            var createMultipleDayOffHandler = new CreateMultipleDayOffHandler(
                _dbService.Object,
                _str.Object
            );

            var result = await Assert.ThrowsAsync<ApiErrorException>(
                async () =>
                    await createMultipleDayOffHandler.Handle(
                        createMultipleDayOffCommand,
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
