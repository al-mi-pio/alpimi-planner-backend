using AlpimiAPI.Database;
using AlpimiAPI.Entities.EGroup;
using AlpimiAPI.Entities.ESchedule;
using AlpimiAPI.Entities.EStudent;
using AlpimiAPI.Entities.EStudent.Commands;
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
                    new ErrorObject[] { new ErrorObject("Group was not found") }
                ),
                JsonConvert.SerializeObject(result.errors)
            );
        }

        [Fact]
        public async Task ThrowsErrorWhenNameIsAlreadyTakenByStudent()
        {
            var dto = MockData.GetCreateStudentDTODetails(new Guid());

            _dbService
                .Setup(s => s.Get<Group>(It.IsAny<string>(), It.IsAny<object>()))
                .ReturnsAsync(MockData.GetGroupDetails());
            _dbService
                .Setup(s => s.GetAll<Student>(It.IsAny<string>(), It.IsAny<object>()))
                .ReturnsAsync(new List<Student> { MockData.GetStudentDetails() });

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
