using AlpimiAPI.Database;
using AlpimiAPI.Entities.EGroup;
using AlpimiAPI.Entities.EStudent;
using AlpimiAPI.Entities.EStudent.Commands;
using AlpimiAPI.Entities.ESubgroup;
using AlpimiAPI.Locales;
using AlpimiAPI.Responses;
using AlpimiTest.TestSetup;
using AlpimiTest.TestUtilities;
using Microsoft.Extensions.Localization;
using Moq;
using Newtonsoft.Json;
using Xunit;

namespace AlpimiTest.Entities.EStudent.Commands
{
    [Collection("Sequential Tests")]
    public class CreateStudentCommandUnit
    {
        private readonly Mock<IDbService> _dbService = new();
        private readonly Mock<IStringLocalizer<Errors>> _str;

        public CreateStudentCommandUnit()
        {
            _str = ResourceSetup.Setup();
        }

        [Fact]
        public async Task ThrowsErrorWhenWrongGroupIdIsGiven()
        {
            var createStudentCommand = new CreateStudentCommand(
                new Guid(),
                MockData.GetCreateStudentDTODetails(new Guid()),
                new Guid(),
                "User"
            );

            var createStudentHandler = new CreateStudentHandler(_dbService.Object, _str.Object);

            var result = await Assert.ThrowsAsync<ApiErrorException>(
                async () =>
                    await createStudentHandler.Handle(createStudentCommand, new CancellationToken())
            );

            Assert.Equal(
                JsonConvert.SerializeObject(
                    new ErrorObject[]
                    {
                        new ErrorObject(
                            "Group with id 00000000-0000-0000-0000-000000000000 was not found"
                        )
                    }
                ),
                JsonConvert.SerializeObject(result.errors)
            );
        }

        [Fact]
        public async Task ThrowsErrorWhenWrongSubgroupIdIsGiven()
        {
            var dto = MockData.GetCreateStudentDTODetails(new Guid());
            dto.SubgroupIds = [new Guid()];

            _dbService
                .Setup(s => s.Get<Group>(It.IsAny<string>(), It.IsAny<object>()))
                .ReturnsAsync(MockData.GetGroupDetails());

            var createStudentCommand = new CreateStudentCommand(
                new Guid(),
                dto,
                new Guid(),
                "User"
            );

            var createStudentHandler = new CreateStudentHandler(_dbService.Object, _str.Object);

            var result = await Assert.ThrowsAsync<ApiErrorException>(
                async () =>
                    await createStudentHandler.Handle(createStudentCommand, new CancellationToken())
            );

            Assert.Equal(
                "Subgroup with id 00000000-0000-0000-0000-000000000000 was not found",
                result.errors.First().message
            );
        }

        [Fact]
        public async Task ThrowsErrorWhenDuplicatedSubgroupIdsAreGiven()
        {
            var dto = MockData.GetCreateStudentDTODetails(new Guid());
            dto.SubgroupIds = [new Guid(), new Guid()];

            _dbService
                .Setup(s => s.Get<Group>(It.IsAny<string>(), It.IsAny<object>()))
                .ReturnsAsync(MockData.GetGroupDetails());

            var createStudentCommand = new CreateStudentCommand(
                new Guid(),
                dto,
                new Guid(),
                "User"
            );

            var createStudentHandler = new CreateStudentHandler(_dbService.Object, _str.Object);

            var result = await Assert.ThrowsAsync<ApiErrorException>(
                async () =>
                    await createStudentHandler.Handle(createStudentCommand, new CancellationToken())
            );

            Assert.Equal(
                "Cannot add multiple Subgroup with the value 00000000-0000-0000-0000-000000000000",
                result.errors.First().message
            );
        }

        [Fact]
        public async Task ThrowsErrorWhenGroupIdsFromStudentAndSubgroupDontMatch()
        {
            var dto = MockData.GetCreateStudentDTODetails(new Guid());
            dto.SubgroupIds = [new Guid()];
            var subgroup = MockData.GetSubgroupDetails();
            subgroup.GroupId = Guid.NewGuid();

            _dbService
                .Setup(s => s.Get<Group>(It.IsAny<string>(), It.IsAny<object>()))
                .ReturnsAsync(MockData.GetGroupDetails());
            _dbService
                .Setup(s => s.Get<Subgroup>(It.IsAny<string>(), It.IsAny<object>()))
                .ReturnsAsync(subgroup);

            var createStudentCommand = new CreateStudentCommand(
                new Guid(),
                dto,
                new Guid(),
                "User"
            );

            var createStudentHandler = new CreateStudentHandler(_dbService.Object, _str.Object);

            var result = await Assert.ThrowsAsync<ApiErrorException>(
                async () =>
                    await createStudentHandler.Handle(createStudentCommand, new CancellationToken())
            );

            Assert.Equal(
                "Subgroup must be in the same Group as Student",
                result.errors.First().message
            );
        }

        [Fact]
        public async Task ThrowsErrorWhenNameIsAlreadyTaken()
        {
            var dto = MockData.GetCreateStudentDTODetails(new Guid());

            _dbService
                .Setup(s => s.Get<Group>(It.IsAny<string>(), It.IsAny<object>()))
                .ReturnsAsync(MockData.GetGroupDetails());
            _dbService
                .Setup(s => s.Get<Student>(It.IsAny<string>(), It.IsAny<object>()))
                .ReturnsAsync(MockData.GetStudentDetails());

            var createStudentCommand = new CreateStudentCommand(
                new Guid(),
                dto,
                new Guid(),
                "User"
            );

            var createStudentHandler = new CreateStudentHandler(_dbService.Object, _str.Object);

            var result = await Assert.ThrowsAsync<ApiErrorException>(
                async () =>
                    await createStudentHandler.Handle(createStudentCommand, new CancellationToken())
            );

            Assert.Equal(
                "There is already a Student with the name 88776655",
                result.errors.First().message
            );
        }
    }
}
