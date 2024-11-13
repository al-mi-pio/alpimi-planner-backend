using AlpimiAPI.Database;
using AlpimiAPI.Entities.ESchedule;
using AlpimiAPI.Entities.ESchedule.Queries;
using AlpimiAPI.Entities.EUser;
using AlpimiTest.TestUtilities;
using Moq;
using Xunit;

namespace AlpimiTest.Entities.ESchedule.Queries
{
    [Collection("Sequential Tests")]
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
    }
}
