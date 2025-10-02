using AlpimiAPI.Database;
using AlpimiAPI.Entities.EAvailability;
using AlpimiAPI.Entities.EAvailability.Commands;
using AlpimiAPI.Entities.EScheduleSettings;
using AlpimiAPI.Entities.ETeacher;
using AlpimiAPI.Locales;
using AlpimiAPI.Responses;
using AlpimiTest.TestSetup;
using AlpimiTest.TestUtilities;
using Microsoft.Extensions.Localization;
using Moq;
using Newtonsoft.Json;
using Xunit;

namespace AlpimiTest.Entities.EAvailability.Commands
{
    [Collection("Sequential Tests")]
    public class UpdateAvailabilityCommandUnit
    {
        private readonly Mock<IDbService> _dbService = new();
        private readonly Mock<IStringLocalizer<Errors>> _str;
        private readonly Mock<IStringLocalizer<Fields>> _strFields;

        public UpdateAvailabilityCommandUnit()
        {
            _str = ResourceSetup.Setup();
            _strFields = ResourceSetup.FieldSetup();
        }

        [Fact]
        public async Task ThrowsErrorWhenWrongTeacherIdIsGiven()
        {
            var dto = MockData.GetUpdateAvailabilityDTODetails();
            dto.TeacherId = new Guid();
            _dbService
                .Setup(s => s.Get<Availability>(It.IsAny<string>(), It.IsAny<object>()))
                .ReturnsAsync(MockData.GetAvailabilityDetails());

            var updateAvailabilityCommand = new UpdateAvailabilityCommand(
                new Guid(),
                dto,
                new Guid(),
                "User"
            );
            var updateAvailabilityHandler = new UpdateAvailabilityHandler(
                _dbService.Object,
                _str.Object,
                _strFields.Object
            );
            var result = await Assert.ThrowsAsync<ApiErrorException>(
                async () =>
                    await updateAvailabilityHandler.Handle(
                        updateAvailabilityCommand,
                        new CancellationToken()
                    )
            );

            Assert.Equal(
                JsonConvert.SerializeObject(
                    new ErrorObject[]
                    {
                        new ErrorObject(
                            "Teacher with id 00000000-0000-0000-0000-000000000000 was not found"
                        )
                    }
                ),
                JsonConvert.SerializeObject(result.errors)
            );
        }

        [Fact]
        public async Task ThrowsErrorWhenStartIsAfterEnd()
        {
            _dbService
                .Setup(s => s.Get<Availability>(It.IsAny<string>(), It.IsAny<object>()))
                .ReturnsAsync(MockData.GetAvailabilityDetails());
            _dbService
                .Setup(s => s.Get<Teacher>(It.IsAny<string>(), It.IsAny<object>()))
                .ReturnsAsync(MockData.GetTeacherDetails());
            _dbService
                .Setup(s => s.Get<ScheduleSettings>(It.IsAny<string>(), It.IsAny<object>()))
                .ReturnsAsync(MockData.GetScheduleSettingsDetails());
            _dbService
                .Setup(s => s.Get<int>(It.IsAny<string>(), It.IsAny<object>()))
                .ReturnsAsync(5);

            var updateRequest = MockData.GetUpdateAvailabilityDTODetails();
            updateRequest.End = 3;
            updateRequest.Start = 4;
            var updateAvailabilityCommand = new UpdateAvailabilityCommand(
                new Guid(),
                updateRequest,
                new Guid(),
                "User"
            );
            var updateAvailabilityHandler = new UpdateAvailabilityHandler(
                _dbService.Object,
                _str.Object,
                _strFields.Object
            );
            var result = await Assert.ThrowsAsync<ApiErrorException>(
                async () =>
                    await updateAvailabilityHandler.Handle(
                        updateAvailabilityCommand,
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

        [Fact]
        public async Task ThrowsErrorWhenStartIsLessThan0()
        {
            _dbService
                .Setup(s => s.Get<Availability>(It.IsAny<string>(), It.IsAny<object>()))
                .ReturnsAsync(MockData.GetAvailabilityDetails());
            _dbService
                .Setup(s => s.Get<Teacher>(It.IsAny<string>(), It.IsAny<object>()))
                .ReturnsAsync(MockData.GetTeacherDetails());
            _dbService
                .Setup(s => s.Get<ScheduleSettings>(It.IsAny<string>(), It.IsAny<object>()))
                .ReturnsAsync(MockData.GetScheduleSettingsDetails());
            _dbService
                .Setup(s => s.Get<int>(It.IsAny<string>(), It.IsAny<object>()))
                .ReturnsAsync(5);

            var updateRequest = MockData.GetUpdateAvailabilityDTODetails();
            updateRequest.Start = -1;
            var updateAvailabilityCommand = new UpdateAvailabilityCommand(
                new Guid(),
                updateRequest,
                new Guid(),
                "User"
            );
            var updateAvailabilityHandler = new UpdateAvailabilityHandler(
                _dbService.Object,
                _str.Object,
                _strFields.Object
            );
            var result = await Assert.ThrowsAsync<ApiErrorException>(
                async () =>
                    await updateAvailabilityHandler.Handle(
                        updateAvailabilityCommand,
                        new CancellationToken()
                    )
            );

            Assert.Equal(
                JsonConvert.SerializeObject(
                    new ErrorObject[]
                    {
                        new FieldErrorObject("start", "Start parameter is invalid")
                    }
                ),
                JsonConvert.SerializeObject(result.errors)
            );
        }

        [Fact]
        public async Task ThrowsErrorWhenEndIsMoreThanTheAmountOfPeriods()
        {
            _dbService
                .Setup(s => s.Get<Availability>(It.IsAny<string>(), It.IsAny<object>()))
                .ReturnsAsync(MockData.GetAvailabilityDetails());
            _dbService
                .Setup(s => s.Get<Teacher>(It.IsAny<string>(), It.IsAny<object>()))
                .ReturnsAsync(MockData.GetTeacherDetails());
            _dbService
                .Setup(s => s.Get<ScheduleSettings>(It.IsAny<string>(), It.IsAny<object>()))
                .ReturnsAsync(MockData.GetScheduleSettingsDetails());
            _dbService
                .Setup(s => s.Get<int>(It.IsAny<string>(), It.IsAny<object>()))
                .ReturnsAsync(5);

            var updateRequest = MockData.GetUpdateAvailabilityDTODetails();
            updateRequest.End = 6;
            var updateAvailabilityCommand = new UpdateAvailabilityCommand(
                new Guid(),
                updateRequest,
                new Guid(),
                "User"
            );
            var updateAvailabilityHandler = new UpdateAvailabilityHandler(
                _dbService.Object,
                _str.Object,
                _strFields.Object
            );
            var result = await Assert.ThrowsAsync<ApiErrorException>(
                async () =>
                    await updateAvailabilityHandler.Handle(
                        updateAvailabilityCommand,
                        new CancellationToken()
                    )
            );

            Assert.Equal(
                JsonConvert.SerializeObject(
                    new FieldErrorObject[]
                    {
                        new FieldErrorObject("end", "End parameter is invalid")
                    }
                ),
                JsonConvert.SerializeObject(result.errors)
            );
        }
    }
}
