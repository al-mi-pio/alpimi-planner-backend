using AlpimiAPI.Database;
using AlpimiAPI.Entities.ESchedule;
using AlpimiAPI.Entities.ESchedule.Commands;
using AlpimiAPI.Entities.EUser;
using AlpimiAPI.Responses;
using Moq;
using Newtonsoft.Json;
using Xunit;

namespace AlpimiTest.Entities.ESchedule.Commands
{
    public class CreateScheduleCommandUnit
    {
        private readonly Mock<IDbService> _dbService = new();

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

            var createScheduleHandler = new CreateScheduleHandler(_dbService.Object);

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

            var createScheduleHandler = new CreateScheduleHandler(_dbService.Object);

            var result = await Assert.ThrowsAsync<ApiErrorException>(
                async () =>
                    await createScheduleHandler.Handle(
                        createScheduleCommand,
                        new CancellationToken()
                    )
            );

            Assert.Equal(
                JsonConvert.SerializeObject(
                    new ErrorObject[] { new ErrorObject("Name already taken") }
                ),
                JsonConvert.SerializeObject(result.errors)
            );
        }
    }
}
