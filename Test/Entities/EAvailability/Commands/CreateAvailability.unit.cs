using AlpimiAPI.Database;
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
    public class CreateAvailabilityCommandUnit
    {
        private readonly Mock<IDbService> _dbService = new();
        private readonly Mock<IStringLocalizer<Errors>> _str;
        private readonly Mock<IStringLocalizer<Fields>> _strFields;

        public CreateAvailabilityCommandUnit()
        {
            _str = ResourceSetup.Setup();
            _strFields = ResourceSetup.FieldSetup();
        }

        [Fact]
        public async Task ThrowsErrorWhenWrongTeacherIdIsGiven()
        {
            var createAvailabilityCommand = new CreateAvailabilityCommand(
                new Guid(),
                MockData.GetCreateAvailabilityDTODetails(new Guid()),
                new Guid(),
                "User"
            );
            var createAvailabilityHandler = new CreateAvailabilityHandler(
                _dbService.Object,
                _str.Object,
                _strFields.Object
            );
            var result = await Assert.ThrowsAsync<ApiErrorException>(
                async () =>
                    await createAvailabilityHandler.Handle(
                        createAvailabilityCommand,
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
                .Setup(s => s.Get<Teacher>(It.IsAny<string>(), It.IsAny<object>()))
                .ReturnsAsync(MockData.GetTeacherDetails());
            _dbService
                .Setup(s => s.Get<ScheduleSettings>(It.IsAny<string>(), It.IsAny<object>()))
                .ReturnsAsync(MockData.GetScheduleSettingsDetails());
            _dbService
                .Setup(s => s.Get<int>(It.IsAny<string>(), It.IsAny<object>()))
                .ReturnsAsync(5);

            var createRequest = MockData.GetCreateAvailabilityDTODetails(new Guid());
            createRequest.End = 3;
            createRequest.Start = 4;
            var createAvailabilityCommand = new CreateAvailabilityCommand(
                new Guid(),
                createRequest,
                new Guid(),
                "User"
            );
            var createAvailabilityHandler = new CreateAvailabilityHandler(
                _dbService.Object,
                _str.Object,
                _strFields.Object
            );
            var result = await Assert.ThrowsAsync<ApiErrorException>(
                async () =>
                    await createAvailabilityHandler.Handle(
                        createAvailabilityCommand,
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
                .Setup(s => s.Get<Teacher>(It.IsAny<string>(), It.IsAny<object>()))
                .ReturnsAsync(MockData.GetTeacherDetails());
            _dbService
                .Setup(s => s.Get<ScheduleSettings>(It.IsAny<string>(), It.IsAny<object>()))
                .ReturnsAsync(MockData.GetScheduleSettingsDetails());
            _dbService
                .Setup(s => s.Get<int>(It.IsAny<string>(), It.IsAny<object>()))
                .ReturnsAsync(5);

            var createRequest = MockData.GetCreateAvailabilityDTODetails(new Guid());
            createRequest.Start = -1;
            var createAvailabilityCommand = new CreateAvailabilityCommand(
                new Guid(),
                createRequest,
                new Guid(),
                "User"
            );
            var createAvailabilityHandler = new CreateAvailabilityHandler(
                _dbService.Object,
                _str.Object,
                _strFields.Object
            );
            var result = await Assert.ThrowsAsync<ApiErrorException>(
                async () =>
                    await createAvailabilityHandler.Handle(
                        createAvailabilityCommand,
                        new CancellationToken()
                    )
            );

            Assert.Equal(
                JsonConvert.SerializeObject(
                    new FieldErrorObject[]
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
                .Setup(s => s.Get<Teacher>(It.IsAny<string>(), It.IsAny<object>()))
                .ReturnsAsync(MockData.GetTeacherDetails());
            _dbService
                .Setup(s => s.Get<ScheduleSettings>(It.IsAny<string>(), It.IsAny<object>()))
                .ReturnsAsync(MockData.GetScheduleSettingsDetails());
            _dbService
                .Setup(s => s.Get<int>(It.IsAny<string>(), It.IsAny<object>()))
                .ReturnsAsync(5);

            var createRequest = MockData.GetCreateAvailabilityDTODetails(new Guid());
            createRequest.End = 6;
            var createAvailabilityCommand = new CreateAvailabilityCommand(
                new Guid(),
                createRequest,
                new Guid(),
                "User"
            );
            var createAvailabilityHandler = new CreateAvailabilityHandler(
                _dbService.Object,
                _str.Object,
                _strFields.Object
            );
            var result = await Assert.ThrowsAsync<ApiErrorException>(
                async () =>
                    await createAvailabilityHandler.Handle(
                        createAvailabilityCommand,
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
