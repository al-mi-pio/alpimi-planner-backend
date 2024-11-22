using AlpimiAPI.Database;
using AlpimiAPI.Entities.EDayOff;
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
    public class UpdateDayOffCommandUnit
    {
        private readonly Mock<IDbService> _dbService = new();
        private readonly Mock<IStringLocalizer<Errors>> _str;

        public UpdateDayOffCommandUnit()
        {
            _str = ResourceSetup.Setup();
        }

        [Fact]
        public async Task ThrowsErrorWhenOutOfRangeDateIsProvided()
        {
            var dto = MockData.GetUpdateDayOffDTODetails();
            dto.From = new DateTime(1000, 1, 1);

            var scheduleSettings = MockData.GetScheduleSettingsDetails();
            var dayOff = MockData.GetDayOffDetails();

            _dbService
                .Setup(s => s.Get<ScheduleSettings>(It.IsAny<string>(), It.IsAny<object>()))
                .ReturnsAsync(scheduleSettings);
            _dbService
                .Setup(s => s.Get<DayOff>(It.IsAny<string>(), It.IsAny<object>()))
                .ReturnsAsync(dayOff);

            var updateDayOffCommand = new UpdateDayOffCommand(new Guid(), dto, new Guid(), "Admin");

            var updateDayOffHandler = new UpdateDayOffHandler(_dbService.Object, _str.Object);

            var result = await Assert.ThrowsAsync<ApiErrorException>(
                async () =>
                    await updateDayOffHandler.Handle(updateDayOffCommand, new CancellationToken())
            );

            Assert.Contains("Date must be in between", result.errors.First().message);
        }

        [Fact]
        public async Task ThrowsErrorWhenDateStartIsAfterDateEnd()
        {
            var dto = MockData.GetUpdateDayOffDTODetails();
            dto.From = new DateTime(2020, 1, 1);
            dto.To = new DateTime(2019, 1, 1);

            var scheduleSettings = MockData.GetScheduleSettingsDetails();
            _dbService
                .Setup(s => s.Get<ScheduleSettings>(It.IsAny<string>(), It.IsAny<object>()))
                .ReturnsAsync(scheduleSettings);

            var updateDayOffCommand = new UpdateDayOffCommand(new Guid(), dto, new Guid(), "User");

            var updateDayOffHandler = new UpdateDayOffHandler(_dbService.Object, _str.Object);

            var result = await Assert.ThrowsAsync<ApiErrorException>(
                async () =>
                    await updateDayOffHandler.Handle(updateDayOffCommand, new CancellationToken())
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
