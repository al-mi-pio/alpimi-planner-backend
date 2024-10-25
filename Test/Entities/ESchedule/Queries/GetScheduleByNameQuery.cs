using AlpimiAPI.Database;
using AlpimiAPI.Entities.ESchedule;
using AlpimiAPI.Entities.ESchedule.Queries;
using AlpimiAPI.Entities.EUser;
using Moq;
using Xunit;

namespace alpimi_planner_backend.Test.Entities.ESchedule.Queries
{
    public class GetScheduleByNameCommandUnit
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
                UserID = new Guid(),
                User = GetUserDetails()
            };

            return schedule;
        }

        [Fact]
        public async Task GetsScheduleWhenNameIsCorrect()
        {
            var schedule = GetScheduleDetails();

            _dbService
                .Setup(s => s.Get<Schedule>(It.IsAny<string>(), It.IsAny<object>()))
                .ReturnsAsync(schedule);

            var getScheduleByNameCommand = new GetScheduleByNameQuery(
                schedule.Name,
                new Guid(),
                "Admin"
            );

            var getScheduleByNameHandler = new GetScheduleByNameHandler(_dbService.Object);

            var result = await getScheduleByNameHandler.Handle(
                getScheduleByNameCommand,
                new CancellationToken()
            );

            Assert.Equal(schedule, result);
        }

        [Fact]
        public async Task ReturnsNullWhenNameIsIncorrect()
        {
            var schedule = GetScheduleDetails();

            _dbService
                .Setup(s => s.Get<Schedule>(It.IsAny<string>(), It.IsAny<object>()))
                .ReturnsAsync((Schedule?)null);

            var getScheduleByNameCommand = new GetScheduleByNameQuery(
                "WrongName",
                new Guid(),
                "Admin"
            );

            var getScheduleByNameHandler = new GetScheduleByNameHandler(_dbService.Object);

            var result = await getScheduleByNameHandler.Handle(
                getScheduleByNameCommand,
                new CancellationToken()
            );

            Assert.Null(result);
        }

        [Fact]
        public async Task ReturnsNullWhenWrongUserGetsDetails()
        {
            var schedule = GetScheduleDetails();

            _dbService
                .Setup(s => s.Get<Schedule>(It.IsAny<string>(), It.IsAny<object>()))
                .ReturnsAsync((Schedule?)null);

            var getScheduleByNameCommand = new GetScheduleByNameQuery(
                schedule.Name,
                new Guid(),
                "User"
            );

            var getScheduleByNameHandler = new GetScheduleByNameHandler(_dbService.Object);

            var result = await getScheduleByNameHandler.Handle(
                getScheduleByNameCommand,
                new CancellationToken()
            );

            Assert.Null(result);
        }
    }
}
