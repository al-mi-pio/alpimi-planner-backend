using AlpimiAPI.Database;
using AlpimiAPI.Entities.EClassroomType;
using AlpimiAPI.Entities.EClassroomType.Commands;
using AlpimiAPI.Entities.ESchedule;
using AlpimiAPI.Locales;
using AlpimiAPI.Responses;
using AlpimiTest.TestSetup;
using AlpimiTest.TestUtilities;
using Microsoft.Extensions.Localization;
using Moq;
using Newtonsoft.Json;
using Xunit;

namespace AlpimiTest.Entities.EClassroomType.Commands
{
    [Collection("Sequential Tests")]
    public class CreateClassroomTypeCommandUnit
    {
        private readonly Mock<IDbService> _dbService = new();
        private readonly Mock<IStringLocalizer<Errors>> _str;

        public CreateClassroomTypeCommandUnit()
        {
            _str = ResourceSetup.Setup();
        }

        [Fact]
        public async Task ThrowsErrorWhenWrongScheduleIdIsGiven()
        {
            var createClassroomTypeCommand = new CreateClassroomTypeCommand(
                new Guid(),
                MockData.GetCreateClassroomTypeDTODetails(new Guid()),
                new Guid(),
                "User"
            );

            var createClassroomTypeHandler = new CreateClassroomTypeHandler(
                _dbService.Object,
                _str.Object
            );

            var result = await Assert.ThrowsAsync<ApiErrorException>(
                async () =>
                    await createClassroomTypeHandler.Handle(
                        createClassroomTypeCommand,
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
            var dto = MockData.GetCreateClassroomTypeDTODetails(new Guid());
            var schedule = MockData.GetScheduleDetails();

            _dbService
                .Setup(s => s.Get<Schedule>(It.IsAny<string>(), It.IsAny<object>()))
                .ReturnsAsync(schedule);
            _dbService
                .Setup(s => s.Get<ClassroomType>(It.IsAny<string>(), It.IsAny<object>()))
                .ReturnsAsync(MockData.GetClassroomTypeDetails());

            var createClassroomTypeCommand = new CreateClassroomTypeCommand(
                new Guid(),
                dto,
                new Guid(),
                "User"
            );

            var createClassroomTypeHandler = new CreateClassroomTypeHandler(
                _dbService.Object,
                _str.Object
            );

            var result = await Assert.ThrowsAsync<ApiErrorException>(
                async () =>
                    await createClassroomTypeHandler.Handle(
                        createClassroomTypeCommand,
                        new CancellationToken()
                    )
            );

            Assert.Equal(
                "There is already a ClassroomType with the name Komputerowa",
                result.errors.First().message
            );
        }
    }
}
