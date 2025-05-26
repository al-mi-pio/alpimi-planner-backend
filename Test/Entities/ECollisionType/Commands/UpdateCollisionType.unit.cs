using AlpimiAPI.Database;
using AlpimiAPI.Entities.ECollisionType;
using AlpimiAPI.Entities.ECollisionType.Commands;
using AlpimiAPI.Locales;
using AlpimiAPI.Responses;
using AlpimiTest.TestSetup;
using AlpimiTest.TestUtilities;
using Microsoft.Extensions.Localization;
using Moq;
using Xunit;

namespace AlpimiTest.Entities.ECollisionType.Commands
{
    [Collection("Sequential Tests")]
    public class UpdateCollisionTypeCommandUnit
    {
        private readonly Mock<IDbService> _dbService = new();
        private readonly Mock<IStringLocalizer<Errors>> _str;

        public UpdateCollisionTypeCommandUnit()
        {
            _str = ResourceSetup.Setup();
        }

        [Fact]
        public async Task ThrowsErrorWhenNameIsAlreadyTakenByCollisionType()
        {
            var dto = MockData.GetUpdateCollisionTypeDTODetails();
            _dbService
                .Setup(s => s.Get<CollisionType>(It.IsAny<string>(), It.IsAny<object>()))
                .ReturnsAsync(MockData.GetCollisionTypeDetails());
            _dbService
                .Setup(s => s.GetAll<CollisionType>(It.IsAny<string>(), It.IsAny<object>()))
                .ReturnsAsync(new List<CollisionType> { MockData.GetCollisionTypeDetails() });

            var updateCollisionTypeCommand = new UpdateCollisionTypeCommand(
                new Guid(),
                dto,
                new Guid(),
                "Admin"
            );
            var updateCollisionTypeHandler = new UpdateCollisionTypeHandler(
                _dbService.Object,
                _str.Object
            );
            var result = await Assert.ThrowsAsync<ApiErrorException>(
                async () =>
                    await updateCollisionTypeHandler.Handle(
                        updateCollisionTypeCommand,
                        new CancellationToken()
                    )
            );

            Assert.Equal(
                "There is already a CollisionType with the name Nie",
                result.errors.First().message
            );
        }

        [Fact]
        public async Task ThrowsErrorWhenWeightIsLessThan0OrMoreThan1()
        {
            var dto = MockData.GetUpdateCollisionTypeDTODetails();
            dto.Weight = -1;

            var updateCollisionTypeCommand = new UpdateCollisionTypeCommand(
                new Guid(),
                dto,
                new Guid(),
                "Admin"
            );
            var updateCollisionTypeHandler = new UpdateCollisionTypeHandler(
                _dbService.Object,
                _str.Object
            );
            var result = await Assert.ThrowsAsync<ApiErrorException>(
                async () =>
                    await updateCollisionTypeHandler.Handle(
                        updateCollisionTypeCommand,
                        new CancellationToken()
                    )
            );

            Assert.Equal("Weight parameter is invalid", result.errors.First().message);
        }
    }
}
