using AlpimiAPI;
using AlpimiAPI.Entities.EUser;
using AlpimiAPI.Entities.EUser.Commands;
using Moq;
using Xunit;

namespace AlpimiTest.Entities.EUser.Commands
{
    public class UpdateUserCommandUnit
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

        [Fact]
        public async Task ReturnsUpdatedUserWhenIdIsCorrect()
        {
            var user = GetUserDetails();

            _dbService
                .Setup(s => s.Update<User>(It.IsAny<string>(), It.IsAny<object>()))
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
                .Setup(s => s.Update<User>(It.IsAny<string>(), It.IsAny<object>()))
                .ReturnsAsync((User?)null);

            var updateUserCommand = new UpdateUserCommand(new Guid(), "marek2", "f44");

            var updateUserHandler = new UpdateUserHandler(_dbService.Object);

            var result = await updateUserHandler.Handle(updateUserCommand, new CancellationToken());

            Assert.Null(result);
        }
    }
}
