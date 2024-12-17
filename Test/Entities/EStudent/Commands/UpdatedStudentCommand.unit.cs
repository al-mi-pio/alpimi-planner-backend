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
using Xunit;

namespace AlpimiTest.Entities.EStudent.Commands
{
    [Collection("Sequential Tests")]
    public class UpdateStudentCommandUnit
    {
        private readonly Mock<IDbService> _dbService = new();
        private readonly Mock<IStringLocalizer<Errors>> _str;

        public UpdateStudentCommandUnit()
        {
            _str = ResourceSetup.Setup();
        }

        [Fact]
        public async Task ThrowsErrorWhenAlbumNumberIsAlreadyTaken()
        {
            var dto = MockData.GetUpdateStudentDTODetails();
            _dbService
                .Setup(s => s.Get<Student>(It.IsAny<string>(), It.IsAny<object>()))
                .ReturnsAsync(MockData.GetStudentDetails());
            _dbService
                .Setup(s => s.Get<Group>(It.IsAny<string>(), It.IsAny<object>()))
                .ReturnsAsync(MockData.GetGroupDetails());
            _dbService
                .Setup(s => s.GetAll<Student>(It.IsAny<string>(), It.IsAny<object>()))
                .ReturnsAsync(new List<Student> { MockData.GetStudentDetails() });

            var updateStudentCommand = new UpdateStudentCommand(
                new Guid(),
                dto,
                new Guid(),
                "Admin"
            );
            var updateStudentHandler = new UpdateStudentHandler(_dbService.Object, _str.Object);
            var result = await Assert.ThrowsAsync<ApiErrorException>(
                async () =>
                    await updateStudentHandler.Handle(updateStudentCommand, new CancellationToken())
            );

            Assert.Equal(
                "There is already a Student with the name newNumber",
                result.errors.First().message
            );
        }

        [Fact]
        public async Task ThrowsErrorWhenWrongSubgroupIdIsGiven()
        {
            var dto = MockData.GetUpdateStudentDTODetails();
            dto.SubgroupIds = [new Guid()];

            _dbService
                .Setup(s => s.Get<Student>(It.IsAny<string>(), It.IsAny<object>()))
                .ReturnsAsync(MockData.GetStudentDetails());
            _dbService
                .Setup(s => s.Get<Group>(It.IsAny<string>(), It.IsAny<object>()))
                .ReturnsAsync(MockData.GetGroupDetails());

            var createStudentCommand = new UpdateStudentCommand(
                new Guid(),
                dto,
                new Guid(),
                "User"
            );

            var createStudentHandler = new UpdateStudentHandler(_dbService.Object, _str.Object);

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
            var dto = MockData.GetUpdateStudentDTODetails();
            dto.SubgroupIds = [new Guid(), new Guid()];

            _dbService
                .Setup(s => s.Get<Student>(It.IsAny<string>(), It.IsAny<object>()))
                .ReturnsAsync(MockData.GetStudentDetails());
            _dbService
                .Setup(s => s.Get<Group>(It.IsAny<string>(), It.IsAny<object>()))
                .ReturnsAsync(MockData.GetGroupDetails());
            _dbService
                .Setup(s => s.Get<Group>(It.IsAny<string>(), It.IsAny<object>()))
                .ReturnsAsync(MockData.GetGroupDetails());

            var createStudentCommand = new UpdateStudentCommand(
                new Guid(),
                dto,
                new Guid(),
                "User"
            );

            var createStudentHandler = new UpdateStudentHandler(_dbService.Object, _str.Object);

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
            var dto = MockData.GetUpdateStudentDTODetails();
            dto.SubgroupIds = [new Guid()];
            var subgroup = MockData.GetSubgroupDetails();
            subgroup.GroupId = Guid.NewGuid();

            _dbService
                .Setup(s => s.Get<Student>(It.IsAny<string>(), It.IsAny<object>()))
                .ReturnsAsync(MockData.GetStudentDetails());
            _dbService
                .Setup(s => s.Get<Group>(It.IsAny<string>(), It.IsAny<object>()))
                .ReturnsAsync(MockData.GetGroupDetails());
            _dbService
                .Setup(s => s.Get<Subgroup>(It.IsAny<string>(), It.IsAny<object>()))
                .ReturnsAsync(subgroup);

            var createStudentCommand = new UpdateStudentCommand(
                new Guid(),
                dto,
                new Guid(),
                "User"
            );

            var createStudentHandler = new UpdateStudentHandler(_dbService.Object, _str.Object);

            var result = await Assert.ThrowsAsync<ApiErrorException>(
                async () =>
                    await createStudentHandler.Handle(createStudentCommand, new CancellationToken())
            );

            Assert.Equal(
                "Subgroup must be in the same Group as Student",
                result.errors.First().message
            );
        }
    }
}
