using AlpimiAPI.Database;
using AlpimiAPI.Entities.ELessonType;
using AlpimiAPI.Entities.ELessonType.Commands;
using AlpimiAPI.Entities.ESchedule;
using AlpimiAPI.Locales;
using AlpimiAPI.Responses;
using AlpimiTest.TestSetup;
using AlpimiTest.TestUtilities;
using Microsoft.Extensions.Localization;
using Moq;
using Newtonsoft.Json;
using Xunit;

namespace AlpimiTest.Entities.ELessonType.Commands
{
    [Collection("Sequential Tests")]
    public class CreateLessonTypeCommandUnit
    {
        private readonly Mock<IDbService> _dbService = new();
        private readonly Mock<IStringLocalizer<Errors>> _str;

        public CreateLessonTypeCommandUnit()
        {
            _str = ResourceSetup.Setup();
        }

        [Fact]
        public async Task ThrowsErrorWhenWrongScheduleIdIsGiven()
        {
            var createLessonTypeCommand = new CreateLessonTypeCommand(
                new Guid(),
                MockData.GetCreateLessonTypeDTODetails(new Guid()),
                new Guid(),
                "User"
            );
            var createLessonTypeHandler = new CreateLessonTypeHandler(
                _dbService.Object,
                _str.Object
            );
            var result = await Assert.ThrowsAsync<ApiErrorException>(
                async () =>
                    await createLessonTypeHandler.Handle(
                        createLessonTypeCommand,
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
        public async Task ThrowsErrorWhenNameIsAlreadyTaken()
        {
            var dto = MockData.GetCreateLessonTypeDTODetails(new Guid());
            _dbService
                .Setup(s => s.Get<Schedule>(It.IsAny<string>(), It.IsAny<object>()))
                .ReturnsAsync(MockData.GetScheduleDetails());
            _dbService
                .Setup(s => s.Get<LessonType>(It.IsAny<string>(), It.IsAny<object>()))
                .ReturnsAsync(MockData.GetLessonTypeDetails());

            var createLessonTypeCommand = new CreateLessonTypeCommand(
                new Guid(),
                dto,
                new Guid(),
                "User"
            );
            var createLessonTypeHandler = new CreateLessonTypeHandler(
                _dbService.Object,
                _str.Object
            );
            var result = await Assert.ThrowsAsync<ApiErrorException>(
                async () =>
                    await createLessonTypeHandler.Handle(
                        createLessonTypeCommand,
                        new CancellationToken()
                    )
            );

            Assert.Equal(
                "There is already a LessonType with the name Komputerowa",
                result.errors.First().message
            );
        }

        [Fact]
        public async Task ThrowsErrorWhenColorIsLessThan0()
        {
            var dto = MockData.GetCreateLessonTypeDTODetails(new Guid());
            dto.Color = -1;

            var createLessonTypeCommand = new CreateLessonTypeCommand(
                new Guid(),
                dto,
                new Guid(),
                "User"
            );
            var createLessonTypeHandler = new CreateLessonTypeHandler(
                _dbService.Object,
                _str.Object
            );
            var result = await Assert.ThrowsAsync<ApiErrorException>(
                async () =>
                    await createLessonTypeHandler.Handle(
                        createLessonTypeCommand,
                        new CancellationToken()
                    )
            );

            Assert.Equal("Color parameter is invalid", result.errors.First().message);
        }

        [Fact]
        public async Task ThrowsErrorWhenColorIsMoreThan359()
        {
            var dto = MockData.GetCreateLessonTypeDTODetails(new Guid());
            dto.Color = 360;

            var createLessonTypeCommand = new CreateLessonTypeCommand(
                new Guid(),
                dto,
                new Guid(),
                "User"
            );
            var createLessonTypeHandler = new CreateLessonTypeHandler(
                _dbService.Object,
                _str.Object
            );
            var result = await Assert.ThrowsAsync<ApiErrorException>(
                async () =>
                    await createLessonTypeHandler.Handle(
                        createLessonTypeCommand,
                        new CancellationToken()
                    )
            );

            Assert.Equal("Color parameter is invalid", result.errors.First().message);
        }
    }
}
