using AlpimiAPI.Database;
using AlpimiAPI.Entities.ESchedule;
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
    public class CreateTeacherCommandUnit
    {
        private readonly Mock<IDbService> _dbService = new();
        private readonly Mock<IStringLocalizer<Errors>> _str;

        public CreateTeacherCommandUnit()
        {
            _str = ResourceSetup.Setup();
        }

        [Fact]
        public async Task ThrowsErrorWhenWrongScheduleIdIsGiven()
        {
            var createTeacherCommand = new CreateTeacherCommand(
                new Guid(),
                MockData.GetCreateTeacherDTODetails(new Guid()),
                new Guid(),
                "User"
            );

            var createTeacherHandler = new CreateTeacherHandler(_dbService.Object, _str.Object);

            var result = await Assert.ThrowsAsync<ApiErrorException>(
                async () =>
                    await createTeacherHandler.Handle(createTeacherCommand, new CancellationToken())
            );

            Assert.Equal(
                JsonConvert.SerializeObject(
                    new ErrorObject[] { new ErrorObject("Schedule was not found") }
                ),
                JsonConvert.SerializeObject(result.errors)
            );
        }

        [Fact]
        public async Task ThrowsErrorWhenNameAndSurnameIsAlreadyTaken()
        {
            var dto = MockData.GetCreateTeacherDTODetails(new Guid());
            var schedule = MockData.GetScheduleDetails();

            _dbService
                .Setup(s => s.Get<Schedule>(It.IsAny<string>(), It.IsAny<object>()))
                .ReturnsAsync(schedule);
            _dbService
                .Setup(s => s.GetAll<Guid>(It.IsAny<string>(), It.IsAny<object>()))
                .ReturnsAsync(new List<Guid> { new Guid() });

            var createTeacherCommand = new CreateTeacherCommand(
                new Guid(),
                dto,
                new Guid(),
                "User"
            );

            var createTeacherHandler = new CreateTeacherHandler(_dbService.Object, _str.Object);

            var result = await Assert.ThrowsAsync<ApiErrorException>(
                async () =>
                    await createTeacherHandler.Handle(createTeacherCommand, new CancellationToken())
            );

            Assert.Equal(
                "There is already a Teacher with the name Jac Pie",
                result.errors.First().message
            );
        }
    }
}
