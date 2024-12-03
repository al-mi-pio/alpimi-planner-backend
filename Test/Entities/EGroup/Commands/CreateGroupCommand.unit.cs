using AlpimiAPI.Database;
using AlpimiAPI.Entities.EGroup;
using AlpimiAPI.Entities.EGroup.Commands;
using AlpimiAPI.Entities.ESchedule;
using AlpimiAPI.Entities.ESubgroup;
using AlpimiAPI.Locales;
using AlpimiAPI.Responses;
using AlpimiTest.TestSetup;
using AlpimiTest.TestUtilities;
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
        public async Task ThrowsErrorWhenNameIsAlreadyTakenByGroup()
        {
            var dto = MockData.GetCreateGroupDTODetails(new Guid());

            _dbService
                .Setup(s => s.Get<Schedule>(It.IsAny<string>(), It.IsAny<object>()))
                .ReturnsAsync(MockData.GetScheduleDetails());
            _dbService
                .Setup(s => s.Get<Group>(It.IsAny<string>(), It.IsAny<object>()))
                .ReturnsAsync(MockData.GetGroupDetails());

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
        public async Task ThrowsErrorWhenNameIsAlreadyTakenBySubgroup()
        {
            var dto = MockData.GetCreateGroupDTODetails(new Guid());

            _dbService
                .Setup(s => s.Get<Schedule>(It.IsAny<string>(), It.IsAny<object>()))
                .ReturnsAsync(MockData.GetScheduleDetails());
            _dbService
                .Setup(s => s.Get<Subgroup>(It.IsAny<string>(), It.IsAny<object>()))
                .ReturnsAsync(MockData.GetSubgroupDetails());

            var createGroupCommand = new CreateGroupCommand(new Guid(), dto, new Guid(), "User");

            var createGroupHandler = new CreateGroupHandler(_dbService.Object, _str.Object);

            var result = await Assert.ThrowsAsync<ApiErrorException>(
                async () =>
                    await createGroupHandler.Handle(createGroupCommand, new CancellationToken())
            );

            Assert.Equal(
                "There is already a Subgroup with the name 13K2",
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
