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
    public class CreateDayOffCommandUnit
    {
        private readonly Mock<IDbService> _dbService = new();
        private readonly Mock<IStringLocalizer<Errors>> _str;

        public CreateDayOffCommandUnit()
        {
            _str = ResourceSetup.Setup();
        }

        [Fact]
        public async Task ThrowsErrorWhenWrongScheduleIdIsGiven()
        {
            var createDayOffCommand = new CreateDayOffCommand(
                new Guid(),
                "Bob",
                new DateTime(2000, 1, 1),
                new Guid(),
                new Guid(),
                "User"
            );

            var createDayOffHandler = new CreateDayOffHandler(_dbService.Object, _str.Object);

            var result = await Assert.ThrowsAsync<ApiErrorException>(
                async () =>
                    await createDayOffHandler.Handle(createDayOffCommand, new CancellationToken())
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

            var createDayOffCommand = new CreateDayOffCommand(
                new Guid(),
                "Bob",
                new DateTime(2000, 1, 1),
                new Guid(),
                new Guid(),
                "User"
            );

            var createDayOffHandler = new CreateDayOffHandler(_dbService.Object, _str.Object);

            var result = await Assert.ThrowsAsync<ApiErrorException>(
                async () =>
                    await createDayOffHandler.Handle(createDayOffCommand, new CancellationToken())
            );

            Assert.Equal(
                JsonConvert.SerializeObject(
                    new ErrorObject[]
                    {
                        new ErrorObject("Date must be in between 19.11.2020 and 19.11.2025")
                    }
                ),
                JsonConvert.SerializeObject(result.errors)
            );
        }
    }
}
