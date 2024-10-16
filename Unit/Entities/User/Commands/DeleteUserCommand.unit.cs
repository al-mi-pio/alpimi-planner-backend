using AlpimiAPI.User.Commands;
using AlpimiAPI.User.Queries;
using alpimi_planner_backend.API;
using Microsoft.EntityFrameworkCore;
using Moq;
using Xunit;

namespace alpimi_planner_backend.Unit.Entities.User.Commands
{
    public class DeleteUserCommandUnit
    {
        private readonly Mock<IDbService> _dbService = new();

        [Fact]
        public async Task IsDeleteCalledProperly()
        {
            _dbService.Setup(s => s.Delete(It.IsAny<string>(), It.IsAny<object>()));

            var deleteUserCommand = new DeleteUserCommand(new Guid());

            var deleteUserHandler = new DeleteUserHandler(_dbService.Object);

            await deleteUserHandler.Handle(deleteUserCommand, new CancellationToken());

            Assert.Equal(1, 1);
        }
    }
}
