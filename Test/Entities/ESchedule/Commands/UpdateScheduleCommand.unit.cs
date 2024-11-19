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
    public class UpdateScheduleCommandUnit
    {
        private readonly Mock<IDbService> _dbService = new();
        private readonly Mock<IStringLocalizer<Errors>> _str;

        public UpdateScheduleCommandUnit()
        {
            _str = ResourceSetup.Setup();
        }

        [Fact]
        public async Task ThrowsErrorWhenURLAlreadyExists()
        {
            var schedule = MockData.GetScheduleDetails();

            _dbService
                .Setup(s => s.Get<Schedule>(It.IsAny<string>(), It.IsAny<object>()))
                .ReturnsAsync(schedule);

            var updateScheduleCommand = new UpdateScheduleCommand(
                schedule.Id,
                schedule.Name,
                61,
                new Guid(),
                "Admin"
            );

            var updateScheduleHandler = new UpdateScheduleHandler(_dbService.Object, _str.Object);

            var result = await Assert.ThrowsAsync<ApiErrorException>(
                async () =>
                    await updateScheduleHandler.Handle(
                        updateScheduleCommand,
                        new CancellationToken()
                    )
            );

            Assert.Equal(
                JsonConvert.SerializeObject(
                    new ErrorObject[]
                    {
                        new ErrorObject("There is already a Schedule with the name Plan_Marka")
                    }
                ),
                JsonConvert.SerializeObject(result.errors)
            );
        }
    }
}
