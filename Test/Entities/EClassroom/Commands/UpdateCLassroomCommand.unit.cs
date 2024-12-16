using AlpimiAPI.Database;
using AlpimiAPI.Entities.EClassroom;
using AlpimiAPI.Entities.EClassroom.Commands;
using AlpimiAPI.Entities.EClassroomType;
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

        [Fact]
        public async Task ThrowsErrorWhenDuplicatedClassroomTypeIdsAreGiven()
        {
            var dto = MockData.GetUpdateClassroomDTODetails();
            dto.ClassroomTypeIds = [new Guid(), new Guid()];
            _dbService
                .Setup(s => s.Get<Classroom>(It.IsAny<string>(), It.IsAny<object>()))
                .ReturnsAsync(MockData.GetClassroomDetails());

            var createClassroomCommand = new UpdateClassroomCommand(
                new Guid(),
                dto,
                new Guid(),
                "User"
            );
            var createClassroomHandler = new UpdateClassroomHandler(_dbService.Object, _str.Object);
            var result = await Assert.ThrowsAsync<ApiErrorException>(
                async () =>
                    await createClassroomHandler.Handle(
                        createClassroomCommand,
                        new CancellationToken()
                    )
            );

            Assert.Equal(
                "Cannot add multiple ClassroomType with the value 00000000-0000-0000-0000-000000000000",
                result.errors.First().message
            );
        }

        [Fact]
        public async Task ThrowsErrorWhenWrongClassroomTypeIdIsGiven()
        {
            var dto = MockData.GetUpdateClassroomDTODetails();
            dto.ClassroomTypeIds = [new Guid()];
            _dbService
                .Setup(s => s.Get<Classroom>(It.IsAny<string>(), It.IsAny<object>()))
                .ReturnsAsync(MockData.GetClassroomDetails());

            var createClassroomCommand = new UpdateClassroomCommand(
                new Guid(),
                dto,
                new Guid(),
                "User"
            );
            var createClassroomHandler = new UpdateClassroomHandler(_dbService.Object, _str.Object);
            var result = await Assert.ThrowsAsync<ApiErrorException>(
                async () =>
                    await createClassroomHandler.Handle(
                        createClassroomCommand,
                        new CancellationToken()
                    )
            );

            Assert.Equal(
                "ClassroomType with id 00000000-0000-0000-0000-000000000000 was not found",
                result.errors.First().message
            );
        }

        [Fact]
        public async Task ThrowsErrorWhenScheduleIdsFromClassroomAndClassroomTypeDontMatch()
        {
            var dto = MockData.GetUpdateClassroomDTODetails();
            dto.ClassroomTypeIds = [new Guid()];
            var classroomType = MockData.GetClassroomTypeDetails();
            classroomType.ScheduleId = Guid.NewGuid();
            _dbService
                .Setup(s => s.Get<Classroom>(It.IsAny<string>(), It.IsAny<object>()))
                .ReturnsAsync(MockData.GetClassroomDetails());
            _dbService
                .Setup(s => s.Get<ClassroomType>(It.IsAny<string>(), It.IsAny<object>()))
                .ReturnsAsync(classroomType);

            var createClassroomCommand = new UpdateClassroomCommand(
                new Guid(),
                dto,
                new Guid(),
                "User"
            );
            var createClassroomHandler = new UpdateClassroomHandler(_dbService.Object, _str.Object);
            var result = await Assert.ThrowsAsync<ApiErrorException>(
                async () =>
                    await createClassroomHandler.Handle(
                        createClassroomCommand,
                        new CancellationToken()
                    )
            );

            Assert.Equal(
                "ClassroomType must be in the same Schedule as Classroom",
                result.errors.First().message
            );
        }
    }
}
