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
using Xunit;

namespace AlpimiTest.Entities.EGroup.Commands
{
    [Collection("Sequential Tests")]
    public class UpdateGroupCommandUnit
    {
        private readonly Mock<IDbService> _dbService = new();
        private readonly Mock<IStringLocalizer<Errors>> _str;

        public UpdateGroupCommandUnit()
        {
            _str = ResourceSetup.Setup();
        }

        [Fact]
        public async Task ThrowsErrorWhenNameIsAlreadyTakenByGroup()
        {
            var dto = MockData.GetUpdateGroupDTODetails();

            _dbService
                .Setup(s => s.Get<Group>(It.IsAny<string>(), It.IsAny<object>()))
                .ReturnsAsync(MockData.GetGroupDetails());
            _dbService
                .Setup(s => s.GetAll<Group>(It.IsAny<string>(), It.IsAny<object>()))
                .ReturnsAsync(new List<Group> { MockData.GetGroupDetails() });

            var updateGroupCommand = new UpdateGroupCommand(new Guid(), dto, new Guid(), "Admin");

            var updateGroupHandler = new UpdateGroupHandler(_dbService.Object, _str.Object);

            var result = await Assert.ThrowsAsync<ApiErrorException>(
                async () =>
                    await updateGroupHandler.Handle(updateGroupCommand, new CancellationToken())
            );

            Assert.Equal(
                "There is already a Group with the name 24L4",
                result.errors.First().message
            );
        }

        [Fact]
        public async Task ThrowsErrorWhenNameIsAlreadyTakenBySubgroup()
        {
            var dto = MockData.GetUpdateGroupDTODetails();

            _dbService
                .Setup(s => s.Get<Group>(It.IsAny<string>(), It.IsAny<object>()))
                .ReturnsAsync(MockData.GetGroupDetails());
            _dbService
                .Setup(s => s.GetAll<Subgroup>(It.IsAny<string>(), It.IsAny<object>()))
                .ReturnsAsync(new List<Subgroup> { MockData.GetSubgroupDetails() });

            var updateGroupCommand = new UpdateGroupCommand(new Guid(), dto, new Guid(), "Admin");

            var updateGroupHandler = new UpdateGroupHandler(_dbService.Object, _str.Object);

            var result = await Assert.ThrowsAsync<ApiErrorException>(
                async () =>
                    await updateGroupHandler.Handle(updateGroupCommand, new CancellationToken())
            );

            Assert.Equal(
                "There is already a Subgroup with the name 24L4",
                result.errors.First().message
            );
        }

        [Fact]
        public async Task ThrowsErrorWhenStudentCountIsLessThan1()
        {
            var dto = MockData.GetUpdateGroupDTODetails();
            dto.StudentCount = -1;

            var updateGroupCommand = new UpdateGroupCommand(new Guid(), dto, new Guid(), "Admin");

            var updateGroupHandler = new UpdateGroupHandler(_dbService.Object, _str.Object);

            var result = await Assert.ThrowsAsync<ApiErrorException>(
                async () =>
                    await updateGroupHandler.Handle(updateGroupCommand, new CancellationToken())
            );

            Assert.Equal("StudentCount parameter is invalid", result.errors.First().message);
        }

        [Fact]
        public async Task ThrowsErrorWhenStudentCountInGroupIsLessThanSubgroup()
        {
            var dto = MockData.GetUpdateGroupDTODetails();
            dto.StudentCount = 1;

            _dbService
                .Setup(s => s.Get<Group>(It.IsAny<string>(), It.IsAny<object>()))
                .ReturnsAsync(MockData.GetGroupDetails());

            _dbService
                .Setup(s => s.GetAll<Guid>(It.IsAny<string>(), It.IsAny<object>()))
                .ReturnsAsync(new List<Guid> { new Guid() });

            var updateGroupCommand = new UpdateGroupCommand(new Guid(), dto, new Guid(), "Admin");

            var updateGroupHandler = new UpdateGroupHandler(_dbService.Object, _str.Object);

            var result = await Assert.ThrowsAsync<ApiErrorException>(
                async () =>
                    await updateGroupHandler.Handle(updateGroupCommand, new CancellationToken())
            );

            Assert.Equal(
                "Student count in a subgroup cannot be greater than the student count in a group",
                result.errors.First().message
            );
        }
    }
}
