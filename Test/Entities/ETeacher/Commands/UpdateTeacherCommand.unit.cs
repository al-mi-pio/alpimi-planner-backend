using AlpimiAPI.Database;
using AlpimiAPI.Entities.ESchedule;
using AlpimiAPI.Entities.ETeacher;
using AlpimiAPI.Entities.ETeacher.Commands;
using AlpimiAPI.Locales;
using AlpimiAPI.Responses;
using AlpimiTest.TestSetup;
using AlpimiTest.TestUtilities;
using Microsoft.Extensions.Localization;
using Moq;
using Xunit;

namespace AlpimiTest.Entities.ETeacher.Commands
{
    [Collection("Sequential Tests")]
    public class UpdateTeacherCommandUnit
    {
        private readonly Mock<IDbService> _dbService = new();
        private readonly Mock<IStringLocalizer<Errors>> _str;

        public UpdateTeacherCommandUnit()
        {
            _str = ResourceSetup.Setup();
        }

        [Fact]
        public async Task ThrowsErrorWhenNameAndSurnameAreAlreadyTaken()
        {
            var dto = MockData.GetUpdateTeacherDTODetails();

            _dbService
                .Setup(s => s.Get<Teacher>(It.IsAny<string>(), It.IsAny<object>()))
                .ReturnsAsync(MockData.GetTeacherDetails());
            _dbService
                .Setup(s => s.GetAll<Teacher>(It.IsAny<string>(), It.IsAny<object>()))
                .ReturnsAsync(new List<Teacher> { MockData.GetTeacherDetails() });

            var updateTeacherCommand = new UpdateTeacherCommand(
                new Guid(),
                dto,
                new Guid(),
                "Admin"
            );

            var updateTeacherHandler = new UpdateTeacherHandler(_dbService.Object, _str.Object);

            var result = await Assert.ThrowsAsync<ApiErrorException>(
                async () =>
                    await updateTeacherHandler.Handle(updateTeacherCommand, new CancellationToken())
            );

            Assert.Equal(
                "There is already a Teacher with the name Pan Jan",
                result.errors.First().message
            );
        }
    }
}
