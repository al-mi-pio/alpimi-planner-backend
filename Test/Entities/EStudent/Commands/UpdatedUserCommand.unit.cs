using AlpimiAPI.Database;
using AlpimiAPI.Entities.EGroup;
using AlpimiAPI.Entities.EStudent;
using AlpimiAPI.Entities.EStudent.Commands;
using AlpimiAPI.Responses;
using AlpimiTest.TestSetup;
using AlpimiTest.TestUtilities;
using alpimi_planner_backend.API.Locales;
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
    }
}
