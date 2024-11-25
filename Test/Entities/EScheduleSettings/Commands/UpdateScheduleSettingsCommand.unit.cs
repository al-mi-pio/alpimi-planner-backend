using AlpimiAPI.Database;
using AlpimiAPI.Entities.EScheduleSettings;
using AlpimiAPI.Entities.EScheduleSettings.Commands;
using AlpimiAPI.Responses;
using AlpimiTest.TestSetup;
using AlpimiTest.TestUtilities;
using alpimi_planner_backend.API.Locales;
using Microsoft.Extensions.Localization;
using Moq;
using Newtonsoft.Json;
using Xunit;

namespace AlpimiTest.Entities.EScheduleSettings.Commands
{
    [Collection("Sequential Tests")]
    public class UpdateScheduleSettingsCommandUnit
    {
        private readonly Mock<IDbService> _dbService = new();
        private readonly Mock<IStringLocalizer<Errors>> _str;

        public UpdateScheduleSettingsCommandUnit()
        {
            _str = ResourceSetup.Setup();
        }

        [Fact]
        public async Task ThrowsErrorWhenDateStartIsAfterDateEnd()
        {
            var dto = MockData.GetUpdateScheduleSettingsDTO();
            dto.SchoolYearStart = new DateOnly(2020, 10, 10);
            dto.SchoolYearEnd = new DateOnly(2000, 10, 10);

            var scheduleSettings = MockData.GetScheduleSettingsDetails();

            var updateScheduleSettingsCommand = new UpdateScheduleSettingsCommand(
                scheduleSettings.Id,
                dto,
                new Guid(),
                "Admin"
            );

            var updateScheduleSettingsHandler = new UpdateScheduleSettingsHandler(
                _dbService.Object,
                _str.Object
            );

            var result = await Assert.ThrowsAsync<ApiErrorException>(
                async () =>
                    await updateScheduleSettingsHandler.Handle(
                        updateScheduleSettingsCommand,
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

        [Fact]
        public async Task ThrowsErrorDaysOffAreOutsideOfDateRange()
        {
            var dto = MockData.GetUpdateScheduleSettingsDTO();
            dto.SchoolYearStart = new DateOnly(2024, 10, 10);
            dto.SchoolYearEnd = new DateOnly(2024, 10, 10);

            _dbService
                .Setup(s => s.GetAll<Guid>(It.IsAny<string>(), It.IsAny<object>()))
                .ReturnsAsync(new List<Guid> { new Guid(), new Guid() });

            var updateScheduleSettingsCommand = new UpdateScheduleSettingsCommand(
                new Guid(),
                dto,
                new Guid(),
                "Admin"
            );

            var updateScheduleSettingsHandler = new UpdateScheduleSettingsHandler(
                _dbService.Object,
                _str.Object
            );

            var result = await Assert.ThrowsAsync<ApiErrorException>(
                async () =>
                    await updateScheduleSettingsHandler.Handle(
                        updateScheduleSettingsCommand,
                        new CancellationToken()
                    )
            );

            Assert.Equal(
                JsonConvert.SerializeObject(
                    new ErrorObject[]
                    {
                        new ErrorObject(
                            "There are days off outside of provided range. Please change them first"
                        )
                    }
                ),
                JsonConvert.SerializeObject(result.errors)
            );
        }
    }
}
