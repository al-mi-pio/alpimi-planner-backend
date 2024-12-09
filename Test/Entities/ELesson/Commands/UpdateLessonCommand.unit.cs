using AlpimiAPI.Database;
using AlpimiAPI.Entities.EGroup;
using AlpimiAPI.Entities.ELesson;
using AlpimiAPI.Entities.ELesson.Commands;
using AlpimiAPI.Locales;
using AlpimiAPI.Responses;
using AlpimiTest.TestSetup;
using AlpimiTest.TestUtilities;
using Microsoft.Extensions.Localization;
using Moq;
using Xunit;

namespace AlpimiTest.Entities.ELesson.Commands
{
    [Collection("Sequential Tests")]
    public class UpdateLessonCommandUnit
    {
        private readonly Mock<IDbService> _dbService = new();
        private readonly Mock<IStringLocalizer<Errors>> _str;

        public UpdateLessonCommandUnit()
        {
            _str = ResourceSetup.Setup();
        }

        [Fact]
        public async Task ThrowsErrorWhenNameIsAlreadyTakenByLesson()
        {
            var dto = MockData.GetUpdateLessonDTODetails();

            _dbService
                .Setup(s => s.Get<Group>(It.IsAny<string>(), It.IsAny<object>()))
                .ReturnsAsync(MockData.GetGroupDetails());

            _dbService
                .Setup(s => s.Get<Lesson>(It.IsAny<string>(), It.IsAny<object>()))
                .ReturnsAsync(MockData.GetLessonDetails());

            _dbService
                .Setup(s => s.GetAll<Lesson>(It.IsAny<string>(), It.IsAny<object>()))
                .ReturnsAsync(new List<Lesson> { MockData.GetLessonDetails() });

            var updateLessonCommand = new UpdateLessonCommand(new Guid(), dto, new Guid(), "Admin");

            var updateLessonHandler = new UpdateLessonHandler(_dbService.Object, _str.Object);

            var result = await Assert.ThrowsAsync<ApiErrorException>(
                async () =>
                    await updateLessonHandler.Handle(updateLessonCommand, new CancellationToken())
            );

            Assert.Equal(
                "There is already a Lesson with the name Bazie Danych",
                result.errors.First().message
            );
        }

        [Fact]
        public async Task ThrowsErrorWhenNumberOfHoursIsLessThan1()
        {
            var dto = MockData.GetUpdateLessonDTODetails();
            dto.AmountOfHours = -1;

            var updateLessonCommand = new UpdateLessonCommand(new Guid(), dto, new Guid(), "Admin");

            var updateLessonHandler = new UpdateLessonHandler(_dbService.Object, _str.Object);

            var result = await Assert.ThrowsAsync<ApiErrorException>(
                async () =>
                    await updateLessonHandler.Handle(updateLessonCommand, new CancellationToken())
            );

            Assert.Equal("AmountOfHours parameter is invalid", result.errors.First().message);
        }
    }
}
