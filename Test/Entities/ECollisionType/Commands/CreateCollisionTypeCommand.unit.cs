using AlpimiAPI.Database;
using AlpimiAPI.Entities.ECollisionType;
using AlpimiAPI.Entities.ECollisionType.Commands;
using AlpimiAPI.Entities.ESchedule;
using AlpimiAPI.Locales;
using AlpimiAPI.Responses;
using AlpimiTest.TestSetup;
using AlpimiTest.TestUtilities;
using Microsoft.Extensions.Localization;
using Moq;
using Newtonsoft.Json;
using Xunit;

namespace AlpimiTest.Entities.ECollisionType.Commands
{
    [Collection("Sequential Tests")]
    public class CreateCollisionTypeCommandUnit
    {
        private readonly Mock<IDbService> _dbService = new();
        private readonly Mock<IStringLocalizer<Errors>> _str;
        private readonly Mock<IStringLocalizer<Fields>> _strFields;

        public CreateCollisionTypeCommandUnit()
        {
            _str = ResourceSetup.Setup();
            _strFields = ResourceSetup.FieldSetup();
        }

        [Fact]
        public async Task ThrowsErrorWhenWrongScheduleIdIsGiven()
        {
            var createCollisionTypeCommand = new CreateCollisionTypeCommand(
                new Guid(),
                MockData.GetCreateCollisionTypeDTODetails(new Guid()),
                new Guid(),
                "User"
            );
            var createCollisionTypeHandler = new CreateCollisionTypeHandler(
                _dbService.Object,
                _str.Object,
                _strFields.Object
            );
            var result = await Assert.ThrowsAsync<ApiErrorException>(
                async () =>
                    await createCollisionTypeHandler.Handle(
                        createCollisionTypeCommand,
                        new CancellationToken()
                    )
            );

            Assert.Equal(
                JsonConvert.SerializeObject(
                    new ErrorObject[]
                    {
                        new ErrorObject(
                            "Schedule with id 00000000-0000-0000-0000-000000000000 was not found"
                        )
                    }
                ),
                JsonConvert.SerializeObject(result.errors)
            );
        }

        [Fact]
        public async Task ThrowsErrorWhenNameIsAlreadyTakenByCollisionType()
        {
            var dto = MockData.GetCreateCollisionTypeDTODetails(new Guid());
            _dbService
                .Setup(s => s.Get<Schedule>(It.IsAny<string>(), It.IsAny<object>()))
                .ReturnsAsync(MockData.GetScheduleDetails());
            _dbService
                .Setup(s => s.Get<CollisionType>(It.IsAny<string>(), It.IsAny<object>()))
                .ReturnsAsync(MockData.GetCollisionTypeDetails());

            var createCollisionTypeCommand = new CreateCollisionTypeCommand(
                new Guid(),
                dto,
                new Guid(),
                "User"
            );
            var createCollisionTypeHandler = new CreateCollisionTypeHandler(
                _dbService.Object,
                _str.Object,
                _strFields.Object
            );
            var result = await Assert.ThrowsAsync<ApiErrorException>(
                async () =>
                    await createCollisionTypeHandler.Handle(
                        createCollisionTypeCommand,
                        new CancellationToken()
                    )
            );

            Assert.Equal(
                "There is already a Collision Type with the name Nauczyciel / 2",
                result.errors.First().message
            );
        }

        [Fact]
        public async Task ThrowsErrorWhenWeightIsLessThan0OrMoreThan1()
        {
            var dto = MockData.GetCreateCollisionTypeDTODetails(new Guid());
            dto.Weight = -1;

            var createCollisionTypeCommand = new CreateCollisionTypeCommand(
                new Guid(),
                dto,
                new Guid(),
                "User"
            );
            var createCollisionTypeHandler = new CreateCollisionTypeHandler(
                _dbService.Object,
                _str.Object,
                _strFields.Object
            );
            var result = await Assert.ThrowsAsync<ApiErrorException>(
                async () =>
                    await createCollisionTypeHandler.Handle(
                        createCollisionTypeCommand,
                        new CancellationToken()
                    )
            );

            Assert.Equal("Weight parameter is invalid", result.errors.First().message);
        }
    }
}
