using AlpimiAPI.Database;
using AlpimiAPI.Entities.ESchedule;
using AlpimiAPI.Entities.ESchedule.Commands;
using AlpimiAPI.Locales;
using AlpimiAPI.Responses;
using AlpimiAPI.Utilities;
using AlpimiTest.TestSetup;
using AlpimiTest.TestUtilities;
using Microsoft.Extensions.Localization;
using Moq;
using Newtonsoft.Json;
using Xunit;

namespace AlpimiTest.Entities.ESchedule.Commands
{
    [Collection("Sequential Tests")]
    public class UpdateScheduleCommandUnit
    {
        private readonly Mock<IDbService> _dbService = new();
        private readonly Mock<IStringLocalizer<Errors>> _str;
        private readonly Mock<IStringLocalizer<Fields>> _strFields;

        public UpdateScheduleCommandUnit()
        {
            _str = ResourceSetup.Setup();
            _strFields = ResourceSetup.FieldSetup();
        }

        [Fact]
        public async Task ThrowsErrorWhenURLAlreadyExists()
        {
            var dto = MockData.GetUpdateScheduleDTODetails();
            _dbService
                .Setup(s => s.Get<Schedule>(It.IsAny<string>(), It.IsAny<object>()))
                .ReturnsAsync(MockData.GetScheduleDetails());

            var updateScheduleCommand = new UpdateScheduleCommand(
                Guid.NewGuid(),
                dto,
                new Guid(),
                "Admin"
            );
            var updateScheduleHandler = new UpdateScheduleHandler(
                _dbService.Object,
                _str.Object,
                _strFields.Object
            );
            var result = await Assert.ThrowsAsync<ApiErrorException>(
                async () =>
                    await updateScheduleHandler.Handle(
                        updateScheduleCommand,
                        new CancellationToken()
                    )
            );

            Assert.Equal(
                JsonConvert.SerializeObject(
                    new ErrorObject[]
                    {
                        new FieldErrorObject(
                            "name",
                            "There is already a Schedule with the name UpdatedPlan"
                        )
                    }
                ),
                JsonConvert.SerializeObject(result.errors)
            );
        }

        [Fact]
        public async Task ThrowsErrorWhenNameContainsAnIllegalSymbol()
        {
            var dto = MockData.GetUpdateScheduleDTODetails();
            dto.Name = "る";
            _dbService
                .Setup(s => s.Get<Schedule>(It.IsAny<string>(), It.IsAny<object>()))
                .ReturnsAsync(MockData.GetScheduleDetails());

            var updateScheduleCommand = new UpdateScheduleCommand(
                Guid.NewGuid(),
                dto,
                new Guid(),
                "Admin"
            );
            var updateScheduleHandler = new UpdateScheduleHandler(
                _dbService.Object,
                _str.Object,
                _strFields.Object
            );
            var result = await Assert.ThrowsAsync<ApiErrorException>(
                async () =>
                    await updateScheduleHandler.Handle(
                        updateScheduleCommand,
                        new CancellationToken()
                    )
            );
            var allowedCharacters = Configuration.GetAllowedCharacterTypesForScheduleName();

            Assert.Equal(
                JsonConvert.SerializeObject(
                    new ErrorObject[]
                    {
                        new FieldErrorObject(
                            "name",
                            "Name can only contain the following: "
                                + string.Join(", ", allowedCharacters!)
                        )
                    }
                ),
                JsonConvert.SerializeObject(result.errors)
            );
        }
    }
}
