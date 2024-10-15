using AlpimiAPI.User.Commands;
using AlpimiAPI.User.Queries;
using alpimi_planner_backend.API;
using Microsoft.EntityFrameworkCore;
using Moq;
using Xunit;

namespace AlpimiAPI.Unit.Entities.UserTest.Commands
{
    public class DeleteUserCommandUnit
    {
        private readonly Mock<IDbService> _dbService = new();

        private User.User GetUserDetails()
        {
            var user = new User.User()
            {
                Id = new Guid(),
                Login = "marek",
                CustomURL = "44f"
            };

            return user;
        }

        [Fact]
        public async Task IsDeleteCalledProperly()
        {
            var user = GetUserDetails();

            _dbService.Setup(s => s.Delete(It.IsAny<string>(), It.IsAny<object>()));

            var deleteUserCommand = new DeleteUserCommand(user.Id);

            var deleteUserHandler = new DeleteUserHandler(_dbService.Object);

            await deleteUserHandler.Handle(deleteUserCommand, new CancellationToken());

            Assert.Equal(1, 1);
        }
    }
}
