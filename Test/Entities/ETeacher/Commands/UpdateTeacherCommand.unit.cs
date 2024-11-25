using AlpimiAPI.Database;
using AlpimiAPI.Entities.ESchedule;
using AlpimiAPI.Entities.ETeacher;
using AlpimiAPI.Entities.ETeacher.Commands;
using AlpimiAPI.Responses;
using AlpimiTest.TestSetup;
using AlpimiTest.TestUtilities;
using alpimi_planner_backend.API.Locales;
using Microsoft.Extensions.Localization;
using Moq;
using Newtonsoft.Json;
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

            var schedule = MockData.GetScheduleDetails();

            _dbService
                .Setup(s => s.Get<Schedule>(It.IsAny<string>(), It.IsAny<object>()))
                .ReturnsAsync(schedule);
            _dbService
                .Setup(s => s.GetAll<Guid>(It.IsAny<string>(), It.IsAny<object>()))
                .ReturnsAsync(new List<Guid> { new Guid() });

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
