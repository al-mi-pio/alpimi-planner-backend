using AlpimiAPI.Database;
using AlpimiAPI.Entities.EUser.Commands;
using Moq;
using Xunit;

namespace AlpimiTest.Entities.EUser.Commands
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
        }
    }
}
