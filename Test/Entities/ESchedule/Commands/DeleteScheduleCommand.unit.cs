using AlpimiAPI.Database;
using AlpimiAPI.Entities.ESchedule.Commands;
using Moq;
using Xunit;

namespace AlpimiTest.Entities.ESchedule.Commands
{
    [Collection("Sequential Tests")]
    public class DeleteScheduleCommandUnit
    {
        private readonly Mock<IDbService> _dbService = new();

        [Fact]
        public async Task IsDeleteCalledProperly()
        {
            _dbService.Setup(s => s.Delete(It.IsAny<string>(), It.IsAny<object>()));

            var deleteScheduleCommand = new DeleteScheduleCommand(new Guid(), new Guid(), "Admin");

            var deleteScheduleHandler = new DeleteScheduleHandler(_dbService.Object);

            await deleteScheduleHandler.Handle(deleteScheduleCommand, new CancellationToken());
        }
    }
}
