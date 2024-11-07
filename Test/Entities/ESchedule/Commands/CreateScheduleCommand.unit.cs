using AlpimiAPI.Database;
using AlpimiAPI.Entities.ESchedule;
using AlpimiAPI.Entities.ESchedule.Commands;
using AlpimiAPI.Entities.EUser;
using AlpimiAPI.Responses;
using AlpimiTest.TestUtilities;
using alpimi_planner_backend.API.Locales;
using Microsoft.Extensions.Localization;
using Moq;
using Newtonsoft.Json;
using Xunit;

namespace AlpimiTest.Entities.ESchedule.Commands
{
    public class CreateScheduleCommandUnit
    {
        private readonly Mock<IDbService> _dbService = new();
        private Mock<IStringLocalizer<Errors>> _str = new();

        private User GetUserDetails()
        {
            var user = new User()
            {
                Id = new Guid(),
                Login = "marek",
                CustomURL = "44f"
            };

            return user;
        }

        private Schedule GetScheduleDetails()
        {
            var schedule = new Schedule()
            {
                Id = new Guid(),
                Name = "Plan_Marka",
                SchoolHour = 60,
                UserId = new Guid(),
                User = GetUserDetails()
            };

            return schedule;
        }

        [Fact]
        public async Task CreatesSchedule()
        {
            var schedule = GetScheduleDetails();

            _dbService
                .Setup(s => s.Post<Schedule>(It.IsAny<string>(), It.IsAny<object>()))
                .ReturnsAsync(schedule);

            var createScheduleCommand = new CreateScheduleCommand(
                schedule.Id,
                schedule.UserId,
                schedule.Name,
                schedule.SchoolHour
            );

            var createScheduleHandler = new CreateScheduleHandler(_dbService.Object, _str.Object);

            var result = await createScheduleHandler.Handle(
                createScheduleCommand,
                new CancellationToken()
            );

            Assert.Equal(schedule.Id, result);
        }

        [Fact]
        public async Task ThrowsErrorWheNameIsTaken()
        {
            var schedule = GetScheduleDetails();
            _str = ResourceSetup.Setup();
            _dbService
                .Setup(s => s.Post<Schedule>(It.IsAny<string>(), It.IsAny<object>()))
                .ReturnsAsync(schedule);
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
