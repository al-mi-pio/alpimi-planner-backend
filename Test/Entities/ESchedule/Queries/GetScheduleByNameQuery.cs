using AlpimiAPI.Database;
using AlpimiAPI.Entities.ESchedule;
using AlpimiAPI.Entities.ESchedule.Queries;
using AlpimiTest.TestUtilities;
using Moq;
using Xunit;

namespace alpimi_planner_backend.Test.Entities.ESchedule.Queries
{
    public class GetScheduleByNameCommandUnit
    {
        private readonly Mock<IDbService> _dbService = new();

        [Fact]
        public async Task GetsScheduleWhenNameIsCorrect()
        {
            var schedule = MockData.GetScheduleDetails();

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
    }
}
