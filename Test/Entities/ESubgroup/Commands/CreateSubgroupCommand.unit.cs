using AlpimiAPI.Database;
using AlpimiAPI.Entities.EGroup;
using AlpimiAPI.Entities.ESubgroup;
using AlpimiAPI.Entities.ESubgroup.Commands;
using AlpimiAPI.Responses;
using AlpimiTest.TestSetup;
using AlpimiTest.TestUtilities;
using alpimi_planner_backend.API.Locales;
using Microsoft.Extensions.Localization;
using Moq;
using Newtonsoft.Json;
using Xunit;

namespace AlpimiTest.Entities.ESubgroup.Commands
{
    [Collection("Sequential Tests")]
    public class CreateSubgroupCommandUnit
    {
        private readonly Mock<IDbService> _dbService = new();
        private readonly Mock<IStringLocalizer<Errors>> _str;

        public CreateSubgroupCommandUnit()
        {
            _str = ResourceSetup.Setup();
        }

        [Fact]
        public async Task ThrowsErrorWhenWrongGroupIdIsGiven()
        {
            var createSubgroupCommand = new CreateSubgroupCommand(
                new Guid(),
                MockData.GetCreateSubgroupDTODetails(new Guid()),
                new Guid(),
                "User"
            );

            var createSubgroupHandler = new CreateSubgroupHandler(_dbService.Object, _str.Object);

            var result = await Assert.ThrowsAsync<ApiErrorException>(
                async () =>
                    await createSubgroupHandler.Handle(
                        createSubgroupCommand,
                        new CancellationToken()
                    )
            );

            Assert.Equal(
                JsonConvert.SerializeObject(
                    new ErrorObject[] { new ErrorObject("Group was not found") }
                ),
                JsonConvert.SerializeObject(result.errors)
            );
        }

        [Fact]
        public async Task ThrowsErrorWhenNameIsAlreadyTakenBySubgroup()
        {
            var dto = MockData.GetCreateSubgroupDTODetails(new Guid());

            _dbService
                .Setup(s => s.Get<Group>(It.IsAny<string>(), It.IsAny<object>()))
                .ReturnsAsync(MockData.GetGroupDetails());
            _dbService
                .Setup(s => s.GetAll<Subgroup>(It.IsAny<string>(), It.IsAny<object>()))
                .ReturnsAsync(new List<Subgroup> { MockData.GetSubgroupDetails() });

            var createSubgroupCommand = new CreateSubgroupCommand(
                new Guid(),
                dto,
                new Guid(),
                "User"
            );

            var createSubgroupHandler = new CreateSubgroupHandler(_dbService.Object, _str.Object);

            var result = await Assert.ThrowsAsync<ApiErrorException>(
                async () =>
                    await createSubgroupHandler.Handle(
                        createSubgroupCommand,
                        new CancellationToken()
                    )
            );

            Assert.Equal(
                "There is already a Subgroup with the name K03",
                result.errors.First().message
            );
        }

        [Fact]
        public async Task ThrowsErrorWhenNameIsAlreadyTakenByGroup()
        {
            var dto = MockData.GetCreateSubgroupDTODetails(new Guid());

            _dbService
                .Setup(s => s.Get<Group>(It.IsAny<string>(), It.IsAny<object>()))
                .ReturnsAsync(MockData.GetGroupDetails());
            _dbService
                .Setup(s => s.GetAll<Group>(It.IsAny<string>(), It.IsAny<object>()))
                .ReturnsAsync(new List<Group> { MockData.GetGroupDetails() });

            var createSubgroupCommand = new CreateSubgroupCommand(
                new Guid(),
                dto,
                new Guid(),
                "User"
            );

            var createSubgroupHandler = new CreateSubgroupHandler(_dbService.Object, _str.Object);

            var result = await Assert.ThrowsAsync<ApiErrorException>(
                async () =>
                    await createSubgroupHandler.Handle(
                        createSubgroupCommand,
                        new CancellationToken()
                    )
            );

            Assert.Equal(
                "There is already a Group with the name K03",
                result.errors.First().message
            );
        }

        [Fact]
        public async Task ThrowsErrorWhenStudentCountIsLessThan1()
        {
            var dto = MockData.GetCreateSubgroupDTODetails(new Guid());
            dto.StudentCount = -1;

            var createSubgroupCommand = new CreateSubgroupCommand(
                new Guid(),
                dto,
                new Guid(),
                "User"
            );

            var createSubgroupHandler = new CreateSubgroupHandler(_dbService.Object, _str.Object);

            var result = await Assert.ThrowsAsync<ApiErrorException>(
                async () =>
                    await createSubgroupHandler.Handle(
                        createSubgroupCommand,
                        new CancellationToken()
                    )
            );

            Assert.Equal("StudentCount parameter is invalid", result.errors.First().message);
        }

        [Fact]
        public async Task ThrowsErrorWhenStudentCountInSubgroupIsMoreThanGroup()
        {
            var dto = MockData.GetCreateSubgroupDTODetails(new Guid());
            dto.StudentCount = 100;

            var createSubgroupCommand = new CreateSubgroupCommand(
                new Guid(),
                dto,
                new Guid(),
                "User"
            );

            _dbService
                .Setup(s => s.Get<Group>(It.IsAny<string>(), It.IsAny<object>()))
                .ReturnsAsync(MockData.GetGroupDetails());

            var createSubgroupHandler = new CreateSubgroupHandler(_dbService.Object, _str.Object);

            var result = await Assert.ThrowsAsync<ApiErrorException>(
                async () =>
                    await createSubgroupHandler.Handle(
                        createSubgroupCommand,
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
