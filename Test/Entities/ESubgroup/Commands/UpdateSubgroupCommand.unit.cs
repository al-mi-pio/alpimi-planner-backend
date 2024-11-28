using AlpimiAPI.Database;
using AlpimiAPI.Entities.EGroup;
using AlpimiAPI.Entities.ESchedule;
using AlpimiAPI.Entities.ESubgroup;
using AlpimiAPI.Entities.ESubgroup.Commands;
using AlpimiAPI.Responses;
using AlpimiTest.TestSetup;
using AlpimiTest.TestUtilities;
using alpimi_planner_backend.API.Locales;
using Microsoft.Extensions.Localization;
using Moq;
using Xunit;

namespace AlpimiTest.Entities.ESubgroup.Commands
{
    [Collection("Sequential Tests")]
    public class UpdateSubgroupCommandUnit
    {
        private readonly Mock<IDbService> _dbService = new();
        private readonly Mock<IStringLocalizer<Errors>> _str;

        public UpdateSubgroupCommandUnit()
        {
            _str = ResourceSetup.Setup();
        }

        [Fact]
        public async Task ThrowsErrorWhenNameIsAlreadyTakenBySubgroup()
        {
            var dto = MockData.GetUpdateSubgroupDTODetails();

            _dbService
                .Setup(s => s.Get<Group>(It.IsAny<string>(), It.IsAny<object>()))
                .ReturnsAsync(MockData.GetGroupDetails());

            _dbService
                .Setup(s => s.Get<Subgroup>(It.IsAny<string>(), It.IsAny<object>()))
                .ReturnsAsync(MockData.GetSubgroupDetails());

            _dbService
                .Setup(s => s.GetAll<Subgroup>(It.IsAny<string>(), It.IsAny<object>()))
                .ReturnsAsync(new List<Subgroup> { MockData.GetSubgroupDetails() });

            var updateSubgroupCommand = new UpdateSubgroupCommand(
                new Guid(),
                dto,
                new Guid(),
                "Admin"
            );

            var updateSubgroupHandler = new UpdateSubgroupHandler(_dbService.Object, _str.Object);

            var result = await Assert.ThrowsAsync<ApiErrorException>(
                async () =>
                    await updateSubgroupHandler.Handle(
                        updateSubgroupCommand,
                        new CancellationToken()
                    )
            );

            Assert.Equal(
                "There is already a Subgroup with the name L02",
                result.errors.First().message
            );
        }

        [Fact]
        public async Task ThrowsErrorWhenNameIsAlreadyTakenByGroup()
        {
            var dto = MockData.GetUpdateSubgroupDTODetails();

            _dbService
                .Setup(s => s.Get<Group>(It.IsAny<string>(), It.IsAny<object>()))
                .ReturnsAsync(MockData.GetGroupDetails());
            _dbService
                .Setup(s => s.GetAll<Group>(It.IsAny<string>(), It.IsAny<object>()))
                .ReturnsAsync(new List<Group> { MockData.GetGroupDetails() });

            var updateSubgroupCommand = new UpdateSubgroupCommand(
                new Guid(),
                dto,
                new Guid(),
                "Admin"
            );

            var updateSubgroupHandler = new UpdateSubgroupHandler(_dbService.Object, _str.Object);

            var result = await Assert.ThrowsAsync<ApiErrorException>(
                async () =>
                    await updateSubgroupHandler.Handle(
                        updateSubgroupCommand,
                        new CancellationToken()
                    )
            );

            Assert.Equal(
                "There is already a Group with the name L02",
                result.errors.First().message
            );
        }

        [Fact]
        public async Task ThrowsErrorWhenStudentCountIsLessThan1()
        {
            var dto = MockData.GetUpdateSubgroupDTODetails();
            dto.StudentCount = -1;

            var updateSubgroupCommand = new UpdateSubgroupCommand(
                new Guid(),
                dto,
                new Guid(),
                "Admin"
            );

            var updateSubgroupHandler = new UpdateSubgroupHandler(_dbService.Object, _str.Object);

            var result = await Assert.ThrowsAsync<ApiErrorException>(
                async () =>
                    await updateSubgroupHandler.Handle(
                        updateSubgroupCommand,
                        new CancellationToken()
                    )
            );

            Assert.Equal("StudentCount parameter is invalid", result.errors.First().message);
        }

        [Fact]
        public async Task ThrowsErrorWhenStudentCountInSubgroupIsMoreThanGroup()
        {
            var dto = MockData.GetUpdateSubgroupDTODetails();
            dto.StudentCount = 100;

            _dbService
                .Setup(s => s.Get<Group>(It.IsAny<string>(), It.IsAny<object>()))
                .ReturnsAsync(MockData.GetGroupDetails());

            var updateSubgroupCommand = new UpdateSubgroupCommand(
                new Guid(),
                dto,
                new Guid(),
                "Admin"
            );

            var updateSubgroupHandler = new UpdateSubgroupHandler(_dbService.Object, _str.Object);

            var result = await Assert.ThrowsAsync<ApiErrorException>(
                async () =>
                    await updateSubgroupHandler.Handle(
                        updateSubgroupCommand,
                        new CancellationToken()
                    )
            );

            Assert.Equal(
                "Student count in a subgroup cannot be greater than the student count in a group",
                result.errors.First().message
            );
        }
    }
}
