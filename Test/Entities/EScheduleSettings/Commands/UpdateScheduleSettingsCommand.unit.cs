using AlpimiAPI.Database;
using AlpimiAPI.Entities.EDayOff;
using AlpimiAPI.Entities.ELessonBlock;
using AlpimiAPI.Entities.ELessonPeriod;
using AlpimiAPI.Entities.EScheduleSettings;
using AlpimiAPI.Entities.EScheduleSettings.Commands;
using AlpimiAPI.Locales;
using AlpimiAPI.Responses;
using AlpimiTest.TestSetup;
using AlpimiTest.TestUtilities;
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
            _dbService
                .Setup(s => s.Get<ScheduleSettings>(It.IsAny<string>(), It.IsAny<object>()))
                .ReturnsAsync(MockData.GetScheduleSettingsDetails());
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
                .Setup(s => s.Get<ScheduleSettings>(It.IsAny<string>(), It.IsAny<object>()))
                .ReturnsAsync(MockData.GetScheduleSettingsDetails());
            _dbService
                .Setup(s => s.GetAll<DayOff>(It.IsAny<string>(), It.IsAny<object>()))
                .ReturnsAsync(new List<DayOff> { MockData.GetDayOffDetails() });

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
                            "There are DayOff outside of provided range. Please change them first"
                        )
                    }
                ),
                JsonConvert.SerializeObject(result.errors)
            );
        }

        [Fact]
        public async Task ThrowsErrorLessonBlocksAreOutsideOfDateRange()
        {
            var dto = MockData.GetUpdateScheduleSettingsDTO();
            dto.SchoolYearStart = new DateOnly(2024, 10, 10);
            dto.SchoolYearEnd = new DateOnly(2024, 10, 10);
            _dbService
                .Setup(s => s.Get<ScheduleSettings>(It.IsAny<string>(), It.IsAny<object>()))
                .ReturnsAsync(MockData.GetScheduleSettingsDetails());
            _dbService
                .Setup(s => s.GetAll<LessonBlock>(It.IsAny<string>(), It.IsAny<object>()))
                .ReturnsAsync(new List<LessonBlock> { MockData.GetLessonBlockDetails() });

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
                            "There are LessonBlock outside of provided range. Please change them first"
                        )
                    }
                ),
                JsonConvert.SerializeObject(result.errors)
            );
        }

        [Fact]
        public async Task ThrowsErrorWhenSchoolHourIsLessThan1()
        {
            var dto = MockData.GetUpdateScheduleSettingsDTO();
            dto.SchoolHour = 0;

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
                    new ErrorObject[] { new ErrorObject("SchoolHour parameter is invalid") }
                ),
                JsonConvert.SerializeObject(result.errors)
            );
        }

        [Fact]
        public async Task ThrowsErrorWhenSchoolHourIsMoreThan1440()
        {
            var dto = MockData.GetUpdateScheduleSettingsDTO();
            dto.SchoolHour = 1441;

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
                    new ErrorObject[] { new ErrorObject("SchoolHour parameter is invalid") }
                ),
                JsonConvert.SerializeObject(result.errors)
            );
        }

        [Fact]
        public async Task ThrowsErrorWhenSchoolDaysLengthIsOtherThan7()
        {
            var dto = MockData.GetUpdateScheduleSettingsDTO();
            dto.SchoolDays = "11001";

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
                    new ErrorObject[] { new ErrorObject("SchoolDays parameter is invalid") }
                ),
                JsonConvert.SerializeObject(result.errors)
            );
        }

        [Fact]
        public async Task ThrowsErrorWhenSchoolDaysContainsSomethingOtherThan1Or0()
        {
            var dto = MockData.GetUpdateScheduleSettingsDTO();
            dto.SchoolDays = "1100115";

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
                    new ErrorObject[] { new ErrorObject("SchoolDays parameter is invalid") }
                ),
                JsonConvert.SerializeObject(result.errors)
            );
        }

        [Fact]
        public async Task ThrowsErrorWhenLessonPeriodsOverlapAfterUpdatingSchoolHour()
        {
            _dbService
                .Setup(s => s.Get<ScheduleSettings>(It.IsAny<string>(), It.IsAny<object>()))
                .ReturnsAsync(MockData.GetScheduleSettingsDetails());
            _dbService
                .Setup(s => s.GetAll<LessonPeriod>(It.IsAny<string>(), It.IsAny<object>()))
                .ReturnsAsync(
                    new List<LessonPeriod>
                    {
                        MockData.GetLessonPeriodDetails(),
                        MockData.GetLessonPeriodDetails()
                    }
                );

            var updateScheduleSettingsCommand = new UpdateScheduleSettingsCommand(
                new Guid(),
                MockData.GetUpdateScheduleSettingsDTO(),
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
                    new ErrorObject[] { new ErrorObject("LessonPeriods cannot overlap") }
                ),
                JsonConvert.SerializeObject(result.errors)
            );
        }
    }
}
