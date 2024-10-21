using AlpimiAPI.User.Commands;
using alpimi_planner_backend.API;
using Moq;
using Xunit;

namespace AlpimiTest.Entities.User.Commands
{
    public class UpdateUserCommandUnit
    {
        private readonly Mock<IDbService> _dbService = new();

        private AlpimiAPI.User.User GetUserDetails()
        {
            var user = new AlpimiAPI.User.User()
            {
                Id = new Guid(),
                Login = "marek",
                CustomURL = "44f"
            };

            return user;
        }

        [Fact]
        public async Task ReturnsUpdatedUserWhenIdIsCorrect()
        {
            var user = GetUserDetails();

            _dbService
                .Setup(s => s.Update<AlpimiAPI.User.User>(It.IsAny<string>(), It.IsAny<object>()))
                .ReturnsAsync(user);

            var updateUserCommand = new UpdateUserCommand(user.Id, "marek2", "f44");

            var updateUserHandler = new UpdateUserHandler(_dbService.Object);

            var result = await updateUserHandler.Handle(updateUserCommand, new CancellationToken());

            Assert.Equal(user, result);
        }

        [Fact]
        public async Task ReturnsNullWhenIdIsIncorrect()
        {
            var user = GetUserDetails();

            _dbService
                .Setup(s => s.Update<AlpimiAPI.User.User>(It.IsAny<string>(), It.IsAny<object>()))
                .ReturnsAsync((AlpimiAPI.User.User?)null);

            var updateUserCommand = new UpdateUserCommand(new Guid(), "marek2", "f44");

            var updateUserHandler = new UpdateUserHandler(_dbService.Object);

            var result = await updateUserHandler.Handle(updateUserCommand, new CancellationToken());

            Assert.Null(result);
        }
    }
}
