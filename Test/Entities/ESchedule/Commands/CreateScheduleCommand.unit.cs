using AlpimiAPI.Database;
using AlpimiAPI.Entities.ESchedule;
using AlpimiAPI.Entities.ESchedule.Commands;
using AlpimiAPI.Responses;
using AlpimiTest.TestSetup;
using AlpimiTest.TestUtilities;
using alpimi_planner_backend.API.Locales;
using Microsoft.Extensions.Localization;
using Moq;
using Newtonsoft.Json;
using Xunit;

namespace AlpimiTest.Entities.ESchedule.Commands
{
    [Collection("Sequential Tests")]
    public class CreateScheduleCommandUnit
    {
        private readonly Mock<IDbService> _dbService = new();
        private readonly Mock<IStringLocalizer<Errors>> _str;

        public CreateScheduleCommandUnit()
        {
            _str = ResourceSetup.Setup();
        }

        [Fact]
        public async Task ThrowsErrorWheNameIsTaken()
        {
            var schedule = MockData.GetScheduleDetails();

            _dbService
                .Setup(s => s.Get<Schedule>(It.IsAny<string>(), It.IsAny<object>()))
                .ReturnsAsync(schedule);

            var createScheduleCommand = new CreateScheduleCommand(
                schedule.Id,
                schedule.UserId,
                "TakenName",
                schedule.SchoolHour
            );

            var createScheduleHandler = new CreateScheduleHandler(_dbService.Object, _str.Object);

            var result = await Assert.ThrowsAsync<ApiErrorException>(
                async () =>
                    await createScheduleHandler.Handle(
                        createScheduleCommand,
                        new CancellationToken()
                    )
            );

            Assert.Equal(
                JsonConvert.SerializeObject(
                    new ErrorObject[]
                    {
                        new ErrorObject("There is already a Schedule with the name TakenName")
                    }
                ),
                JsonConvert.SerializeObject(result.errors)
            );
        }
    }
}
