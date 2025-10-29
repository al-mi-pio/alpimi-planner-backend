using AlpimiAPI.Database;
using AlpimiAPI.Entities.EDayOff;
using AlpimiAPI.Entities.EDayOff.Commands;
using AlpimiAPI.Entities.EScheduleSettings;
using AlpimiAPI.Locales;
using AlpimiAPI.Responses;
using AlpimiTest.TestSetup;
using AlpimiTest.TestUtilities;
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
        private readonly Mock<IStringLocalizer<Fields>> _strFields;

        public UpdateDayOffCommandUnit()
        {
            _str = ResourceSetup.Setup();
            _strFields = ResourceSetup.FieldSetup();
        }

        [Fact]
        public async Task ThrowsErrorWhenNameIsAlreadyTakenByDayOff()
        {
            var dto = MockData.GetUpdateDayOffDTODetails();
            _dbService
                .Setup(s => s.Get<ScheduleSettings>(It.IsAny<string>(), It.IsAny<object>()))
                .ReturnsAsync(MockData.GetScheduleSettingsDetails());
            _dbService
                .Setup(s => s.Get<DayOff>(It.IsAny<string>(), It.IsAny<object>()))
                .ReturnsAsync(MockData.GetDayOffDetails());
            _dbService
                .Setup(s => s.GetAll<DayOff>(It.IsAny<string>(), It.IsAny<object>()))
                .ReturnsAsync([MockData.GetDayOffDetails()]);

            var updateDayOffCommand = new UpdateDayOffCommand(new Guid(), dto, new Guid(), "Admin");
            var updateDayOffHandler = new UpdateDayOffHandler(
                _dbService.Object,
                _str.Object,
                _strFields.Object
            );
            var result = await Assert.ThrowsAsync<ApiErrorException>(
                async () =>
                    await updateDayOffHandler.Handle(updateDayOffCommand, new CancellationToken())
            );

            Assert.Equal(
                "There is already a Day Off with the name Inny_fest",
                result.errors.First().message
            );
        }

        [Fact]
        public async Task ThrowsErrorWhenOutOfRangeDateIsProvided()
        {
            var dto = MockData.GetUpdateDayOffDTODetails();
            dto.From = new DateOnly(1000, 1, 1);
            var scheduleSettings = MockData.GetScheduleSettingsDetails();
            var dayOff = MockData.GetDayOffDetails();
            _dbService
                .Setup(s => s.Get<ScheduleSettings>(It.IsAny<string>(), It.IsAny<object>()))
                .ReturnsAsync(scheduleSettings);
            _dbService
                .Setup(s => s.Get<DayOff>(It.IsAny<string>(), It.IsAny<object>()))
                .ReturnsAsync(dayOff);

            var updateDayOffCommand = new UpdateDayOffCommand(new Guid(), dto, new Guid(), "Admin");
            var updateDayOffHandler = new UpdateDayOffHandler(
                _dbService.Object,
                _str.Object,
                _strFields.Object
            );
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
            dto.From = new DateOnly(2020, 1, 1);
            dto.To = new DateOnly(2019, 1, 1);
            _dbService
                .Setup(s => s.Get<DayOff>(It.IsAny<string>(), It.IsAny<object>()))
                .ReturnsAsync(MockData.GetDayOffDetails());

            var updateDayOffCommand = new UpdateDayOffCommand(new Guid(), dto, new Guid(), "User");
            var updateDayOffHandler = new UpdateDayOffHandler(
                _dbService.Object,
                _str.Object,
                _strFields.Object
            );
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
