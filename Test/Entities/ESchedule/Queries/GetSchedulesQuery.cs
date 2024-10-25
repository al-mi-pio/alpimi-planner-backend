using AlpimiAPI.Database;
using AlpimiAPI.Entities.ESchedule;
using AlpimiAPI.Entities.ESchedule.Queries;
using AlpimiAPI.Entities.EUser;
using Moq;
using Xunit;

namespace alpimi_planner_backend.Test.Entities.ESchedule.Queries
{
    public class GetScheduleCommandUnit
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

        private IEnumerable<Schedule> GetSchedulesDetails()
        {
            var schedule1 = new Schedule()
            {
                Id = new Guid(),
                Name = "Plan_Marka",
                SchoolHour = 60,
                UserID = new Guid(),
                User = GetUserDetails()
            };
            var schedule2 = new Schedule()
            {
                Id = new Guid(),
                Name = "Plan_Marka",
                SchoolHour = 60,
                UserID = new Guid(),
                User = GetUserDetails()
            };

            return [schedule1, schedule2];
        }

        [Fact]
        public async Task GetsSchedules()
        {
            var schedules = GetSchedulesDetails();

            _dbService
                .Setup(s => s.GetAll<Schedule>(It.IsAny<string>(), It.IsAny<object>()))
                .ReturnsAsync(schedules);

            var getSchedulesCommand = new GetSchedulesQuery(new Guid(), "Admin");

            var getSchedulesHandler = new GetSchedulesHandler(_dbService.Object);

            var result = await getSchedulesHandler.Handle(
                getSchedulesCommand,
                new CancellationToken()
            );

            Assert.Equal(schedules, result);
        }

        [Fact]
        public async Task ReturnsEmptyWhenWrongUserGetsSchedules()
        {
            IEnumerable<Schedule> schedules = Enumerable.Empty<Schedule>();

            _dbService
                .Setup(s => s.GetAll<Schedule>(It.IsAny<string>(), It.IsAny<object>()))
                .ReturnsAsync(schedules);

            var getSchedulesCommand = new GetSchedulesQuery(new Guid(), "User");

            var getSchedulesHandler = new GetSchedulesHandler(_dbService.Object);

            var result = await getSchedulesHandler.Handle(
                getSchedulesCommand,
                new CancellationToken()
            );

            Assert.Empty(result!);
        }
    }
}
