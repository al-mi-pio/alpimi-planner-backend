using AlpimiAPI.Database;
using AlpimiAPI.Entities.EClassroom;
using AlpimiAPI.Entities.EClassroom.Commands;
using AlpimiAPI.Locales;
using AlpimiAPI.Responses;
using AlpimiTest.TestSetup;
using AlpimiTest.TestUtilities;
using Microsoft.Extensions.Localization;
using Moq;
using Xunit;

namespace AlpimiTest.Entities.EClassroom.Commands
{
    [Collection("Sequential Tests")]
    public class UpdateClassroomCommandUnit
    {
        private readonly Mock<IDbService> _dbService = new();
        private readonly Mock<IStringLocalizer<Errors>> _str;

        public UpdateClassroomCommandUnit()
        {
            _str = ResourceSetup.Setup();
        }

        [Fact]
        public async Task ThrowsErrorWhenNameIsAlreadyTaken()
        {
            var dto = MockData.GetUpdateClassroomDTODetails();

            _dbService
                .Setup(s => s.Get<Classroom>(It.IsAny<string>(), It.IsAny<object>()))
                .ReturnsAsync(MockData.GetClassroomDetails());
            _dbService
                .Setup(s => s.GetAll<Classroom>(It.IsAny<string>(), It.IsAny<object>()))
                .ReturnsAsync(new List<Classroom> { MockData.GetClassroomDetails() });

            var updateClassroomCommand = new UpdateClassroomCommand(
                new Guid(),
                dto,
                new Guid(),
                "Admin"
            );

            var updateClassroomHandler = new UpdateClassroomHandler(_dbService.Object, _str.Object);

            var result = await Assert.ThrowsAsync<ApiErrorException>(
                async () =>
                    await updateClassroomHandler.Handle(
                        updateClassroomCommand,
                        new CancellationToken()
                    )
            );

            Assert.Equal(
                "There is already a Classroom with the name E5",
                result.errors.First().message
            );
        }

        [Fact]
        public async Task ThrowsErrorWhenCapacityIsLessThan1()
        {
            var dto = MockData.GetUpdateClassroomDTODetails();
            dto.Capacity = -1;

            var updateClassroomCommand = new UpdateClassroomCommand(
                new Guid(),
                dto,
                new Guid(),
                "Admin"
            );

            var updateClassroomHandler = new UpdateClassroomHandler(_dbService.Object, _str.Object);

            var result = await Assert.ThrowsAsync<ApiErrorException>(
                async () =>
                    await updateClassroomHandler.Handle(
                        updateClassroomCommand,
                        new CancellationToken()
                    )
            );

            Assert.Equal("Capacity parameter is invalid", result.errors.First().message);
        }
    }
}
