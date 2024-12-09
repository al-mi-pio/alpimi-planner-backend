using AlpimiAPI.Database;
using AlpimiAPI.Entities.EClassroom;
using AlpimiAPI.Entities.EClassroom.Commands;
using AlpimiAPI.Entities.EClassroom.Commands;
using AlpimiAPI.Entities.ESchedule;
using AlpimiAPI.Locales;
using AlpimiAPI.Responses;
using AlpimiTest.TestSetup;
using AlpimiTest.TestUtilities;
using Microsoft.Extensions.Localization;
using Moq;
using Newtonsoft.Json;
using Xunit;

namespace AlpimiTest.Entities.EClassroom.Commands
{
    [Collection("Sequential Tests")]
    public class CreateClassroomCommandUnit
    {
        private readonly Mock<IDbService> _dbService = new();
        private readonly Mock<IStringLocalizer<Errors>> _str;

        public CreateClassroomCommandUnit()
        {
            _str = ResourceSetup.Setup();
        }

        [Fact]
        public async Task ThrowsErrorWhenWrongScheduleIdIsGiven()
        {
            var createClassroomCommand = new CreateClassroomCommand(
                new Guid(),
                MockData.GetCreateClassroomDTODetails(new Guid()),
                new Guid(),
                "User"
            );

            var createClassroomHandler = new CreateClassroomHandler(_dbService.Object, _str.Object);

            var result = await Assert.ThrowsAsync<ApiErrorException>(
                async () =>
                    await createClassroomHandler.Handle(
                        createClassroomCommand,
                        new CancellationToken()
                    )
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
        public async Task ThrowsErrorWhenNameIsAlreadyTaken()
        {
            var dto = MockData.GetCreateClassroomDTODetails(new Guid());

            _dbService
                .Setup(s => s.Get<Schedule>(It.IsAny<string>(), It.IsAny<object>()))
                .ReturnsAsync(MockData.GetScheduleDetails());
            _dbService
                .Setup(s => s.Get<Classroom>(It.IsAny<string>(), It.IsAny<object>()))
                .ReturnsAsync(MockData.GetClassroomDetails());

            var createClassroomCommand = new CreateClassroomCommand(
                new Guid(),
                dto,
                new Guid(),
                "User"
            );

            var createClassroomHandler = new CreateClassroomHandler(_dbService.Object, _str.Object);

            var result = await Assert.ThrowsAsync<ApiErrorException>(
                async () =>
                    await createClassroomHandler.Handle(
                        createClassroomCommand,
                        new CancellationToken()
                    )
            );

            Assert.Equal(
                "There is already a Classroom with the name G120",
                result.errors.First().message
            );
        }

        [Fact]
        public async Task ThrowsErrorWhenCapacityIsLessThan1()
        {
            var dto = MockData.GetCreateClassroomDTODetails(new Guid());
            dto.Capacity = -1;

            var createClassroomCommand = new CreateClassroomCommand(
                new Guid(),
                dto,
                new Guid(),
                "User"
            );

            var createClassroomHandler = new CreateClassroomHandler(_dbService.Object, _str.Object);

            var result = await Assert.ThrowsAsync<ApiErrorException>(
                async () =>
                    await createClassroomHandler.Handle(
                        createClassroomCommand,
                        new CancellationToken()
                    )
            );

            Assert.Equal("Capacity parameter is invalid", result.errors.First().message);
        }

        [Fact]
        public async Task ThrowsErrorWhenDuplicatedClassroomTypeIdsAreGiven()
        {
            var dto = MockData.GetCreateClassroomDTODetails(new Guid());
            dto.ClassroomTypeIds = [new Guid(), new Guid()];

            _dbService
                .Setup(s => s.Get<Schedule>(It.IsAny<string>(), It.IsAny<object>()))
                .ReturnsAsync(MockData.GetScheduleDetails());

            var createClassroomCommand = new CreateClassroomCommand(
                new Guid(),
                dto,
                new Guid(),
                "User"
            );

            var createClassroomHandler = new CreateClassroomHandler(_dbService.Object, _str.Object);

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
            var dto = MockData.GetCreateClassroomDTODetails(new Guid());
            dto.ClassroomTypeIds = [new Guid()];

            _dbService
                .Setup(s => s.Get<Schedule>(It.IsAny<string>(), It.IsAny<object>()))
                .ReturnsAsync(MockData.GetScheduleDetails());

            var createClassroomCommand = new CreateClassroomCommand(
                new Guid(),
                dto,
                new Guid(),
                "User"
            );

            var createClassroomHandler = new CreateClassroomHandler(_dbService.Object, _str.Object);

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
    }
}
