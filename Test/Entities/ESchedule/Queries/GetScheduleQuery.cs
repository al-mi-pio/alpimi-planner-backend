using AlpimiAPI.Database;
using AlpimiAPI.Entities.ESchedule;
using AlpimiAPI.Entities.ESchedule.Queries;
using AlpimiAPI.Entities.EUser;
using AlpimiTest.TestUtilities;
using Moq;
using Xunit;

namespace AlpimiTest.Entities.ESchedule.Queries
{
    public class GetScheduleCommandUnit
    {
        private readonly Mock<IDbService> _dbService = new();

        [Fact]
        public async Task GetsScheduleWhenIdIsCorrect()
        {
            var schedule = MockData.GetScheduleDetails();

            _dbService
                .Setup(s => s.Get<Schedule>(It.IsAny<string>(), It.IsAny<object>()))
                .ReturnsAsync(schedule);

            var getScheduleCommand = new GetScheduleQuery(schedule.Id, new Guid(), "Admin");

            var getScheduleHandler = new GetScheduleHandler(_dbService.Object);

            var result = await getScheduleHandler.Handle(
                getScheduleCommand,
                new CancellationToken()
            );

            Assert.Equal(schedule, result);
        }

        [Fact]
        public async Task ReturnsNullWhenIdIsIncorrect()
        {
            var schedule = MockData.GetScheduleDetails();

            _dbService
                .Setup(s => s.Get<Schedule>(It.IsAny<string>(), It.IsAny<object>()))
                .ReturnsAsync((Schedule?)null);

            var getScheduleCommand = new GetScheduleQuery(new Guid(), new Guid(), "Admin");

            var getScheduleHandler = new GetScheduleHandler(_dbService.Object);

            var result = await getScheduleHandler.Handle(
                getScheduleCommand,
                new CancellationToken()
            );
            Assert.Null(result);
        }

        [Fact]
        public async Task ReturnsNullWhenWrongUserGetsDetails()
        {
            var schedule = MockData.GetScheduleDetails();

            _dbService
                .Setup(s => s.Get<Schedule>(It.IsAny<string>(), It.IsAny<object>()))
                .ReturnsAsync((Schedule?)null);

            var getScheduleCommand = new GetScheduleQuery(new Guid(), new Guid(), "User");

            var getScheduleHandler = new GetScheduleHandler(_dbService.Object);

            var result = await getScheduleHandler.Handle(
                getScheduleCommand,
                new CancellationToken()
            );
            Assert.Null(result);
        }
    }
}
