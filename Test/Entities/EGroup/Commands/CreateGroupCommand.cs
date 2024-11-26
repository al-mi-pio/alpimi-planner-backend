using AlpimiAPI.Database;
using AlpimiAPI.Entities.EGroup.Commands;
using AlpimiAPI.Entities.ESchedule;
using AlpimiAPI.Responses;
using AlpimiTest.TestSetup;
using AlpimiTest.TestUtilities;
using alpimi_planner_backend.API.Locales;
using Microsoft.Extensions.Localization;
using Moq;
using Newtonsoft.Json;
using Xunit;

namespace AlpimiTest.Entities.EGroup.Commands
{
    [Collection("Sequential Tests")]
    public class CreateGroupCommandUnit
    {
        private readonly Mock<IDbService> _dbService = new();
        private readonly Mock<IStringLocalizer<Errors>> _str;

        public CreateGroupCommandUnit()
        {
            _str = ResourceSetup.Setup();
        }

        [Fact]
        public async Task ThrowsErrorWhenWrongScheduleIdIsGiven()
        {
            var createGroupCommand = new CreateGroupCommand(
                new Guid(),
                MockData.GetCreateGroupDTODetails(new Guid()),
                new Guid(),
                "User"
            );

            var createGroupHandler = new CreateGroupHandler(_dbService.Object, _str.Object);

            var result = await Assert.ThrowsAsync<ApiErrorException>(
                async () =>
                    await createGroupHandler.Handle(createGroupCommand, new CancellationToken())
            );

            Assert.Equal(
                JsonConvert.SerializeObject(
                    new ErrorObject[] { new ErrorObject("Schedule was not found") }
                ),
                JsonConvert.SerializeObject(result.errors)
            );
        }

        [Fact]
        public async Task ThrowsErrorWhenNameIsAlreadyTaken()
        {
            var dto = MockData.GetCreateGroupDTODetails(new Guid());
            var schedule = MockData.GetScheduleDetails();

            _dbService
                .Setup(s => s.Get<Schedule>(It.IsAny<string>(), It.IsAny<object>()))
                .ReturnsAsync(schedule);
            _dbService
                .Setup(s => s.GetAll<Guid>(It.IsAny<string>(), It.IsAny<object>()))
                .ReturnsAsync(new List<Guid> { new Guid() });

            var createGroupCommand = new CreateGroupCommand(new Guid(), dto, new Guid(), "User");

            var createGroupHandler = new CreateGroupHandler(_dbService.Object, _str.Object);

            var result = await Assert.ThrowsAsync<ApiErrorException>(
                async () =>
                    await createGroupHandler.Handle(createGroupCommand, new CancellationToken())
            );

            Assert.Equal(
                "There is already a Group with the name 13K2",
                result.errors.First().message
            );
        }

        [Fact]
        public async Task ThrowsErrorWhenStudentCountIsLessThan1()
        {
            var dto = MockData.GetCreateGroupDTODetails(new Guid());
            dto.StudentCount = -1;

            var createGroupCommand = new CreateGroupCommand(new Guid(), dto, new Guid(), "User");

            var createGroupHandler = new CreateGroupHandler(_dbService.Object, _str.Object);

            var result = await Assert.ThrowsAsync<ApiErrorException>(
                async () =>
                    await createGroupHandler.Handle(createGroupCommand, new CancellationToken())
            );

            Assert.Equal("StudentCount parameter is invalid", result.errors.First().message);
        }
    }
}
