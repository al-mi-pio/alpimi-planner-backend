using AlpimiAPI.Database;
using AlpimiAPI.Entities.ELessonType;
using AlpimiAPI.Entities.ELessonType.Commands;
using AlpimiAPI.Locales;
using AlpimiAPI.Responses;
using AlpimiTest.TestSetup;
using AlpimiTest.TestUtilities;
using Microsoft.Extensions.Localization;
using Moq;
using Xunit;

namespace AlpimiTest.Entities.ELessonType.Commands
{
    [Collection("Sequential Tests")]
    public class UpdateLessonTypeCommandUnit
    {
        private readonly Mock<IDbService> _dbService = new();
        private readonly Mock<IStringLocalizer<Errors>> _str;
        private readonly Mock<IStringLocalizer<Fields>> _strFields;

        public UpdateLessonTypeCommandUnit()
        {
            _str = ResourceSetup.Setup();
            _strFields = ResourceSetup.FieldSetup();
        }

        [Fact]
        public async Task ThrowsErrorWhenNameIsAlreadyTaken()
        {
            var dto = MockData.GetUpdateLessonTypeDTODetails();
            _dbService
                .Setup(s => s.Get<LessonType>(It.IsAny<string>(), It.IsAny<object>()))
                .ReturnsAsync(MockData.GetLessonTypeDetails());
            _dbService
                .Setup(s => s.GetAll<LessonType>(It.IsAny<string>(), It.IsAny<object>()))
                .ReturnsAsync(new List<LessonType> { MockData.GetLessonTypeDetails() });

            var updateLessonTypeCommand = new UpdateLessonTypeCommand(
                new Guid(),
                dto,
                new Guid(),
                "Admin"
            );
            var updateLessonTypeHandler = new UpdateLessonTypeHandler(
                _dbService.Object,
                _str.Object,
                _strFields.Object
            );
            var result = await Assert.ThrowsAsync<ApiErrorException>(
                async () =>
                    await updateLessonTypeHandler.Handle(
                        updateLessonTypeCommand,
                        new CancellationToken()
                    )
            );

            Assert.Equal(
                "There is already a Lesson Type with the name E5",
                result.errors.First().message
            );
        }

        [Fact]
        public async Task ThrowsErrorWhenColorIsLessThan0()
        {
            var dto = MockData.GetUpdateLessonTypeDTODetails();
            dto.Color = -1;

            var updateLessonTypeCommand = new UpdateLessonTypeCommand(
                new Guid(),
                dto,
                new Guid(),
                "Admin"
            );
            var updateLessonTypeHandler = new UpdateLessonTypeHandler(
                _dbService.Object,
                _str.Object,
                _strFields.Object
            );
            var result = await Assert.ThrowsAsync<ApiErrorException>(
                async () =>
                    await updateLessonTypeHandler.Handle(
                        updateLessonTypeCommand,
                        new CancellationToken()
                    )
            );

            Assert.Equal("Color parameter is invalid", result.errors.First().message);
        }

        [Fact]
        public async Task ThrowsErrorWhenColorIsMoreThan359()
        {
            var dto = MockData.GetUpdateLessonTypeDTODetails();
            dto.Color = 360;

            var updateLessonTypeCommand = new UpdateLessonTypeCommand(
                new Guid(),
                dto,
                new Guid(),
                "Admin"
            );
            var updateLessonTypeHandler = new UpdateLessonTypeHandler(
                _dbService.Object,
                _str.Object,
                _strFields.Object
            );
            var result = await Assert.ThrowsAsync<ApiErrorException>(
                async () =>
                    await updateLessonTypeHandler.Handle(
                        updateLessonTypeCommand,
                        new CancellationToken()
                    )
            );

            Assert.Equal("Color parameter is invalid", result.errors.First().message);
        }
    }
}
