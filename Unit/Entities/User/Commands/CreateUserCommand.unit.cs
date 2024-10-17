using AlpimiAPI.User.Commands;
using alpimi_planner_backend.API;
using alpimi_planner_backend.API.Configuration;
using Moq;
using Xunit;

namespace alpimi_planner_backend.Unit.Entities.User.Commands
{
    public class CreateUserCommandUnit
    {
        private readonly Mock<IDbService> _dbService = new();
        private readonly AuthConfiguration authConfig = new AuthConfiguration();

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
        public async Task IsCreateCalledProperly()
        {
            var user = GetUserDetails();

            _dbService
                .Setup(s => s.Post<AlpimiAPI.User.User>(It.IsAny<string>(), It.IsAny<object>()))
                .ReturnsAsync(user);

            var createUserCommand = new CreateUserCommand(
                user.Id,
                new Guid(),
                user.Login,
                user.CustomURL,
                "RandomPassword!1"
            );

            var createUserHandler = new CreateUserHandler(_dbService.Object);

            var result = await createUserHandler.Handle(createUserCommand, new CancellationToken());

            Assert.Equal(user.Id, result);
        }

        [Fact]
        public async Task DoesPasswordMinLengthWorkProperly()
        {
            var user = GetUserDetails();

            _dbService
                .Setup(s => s.Post<AlpimiAPI.User.User>(It.IsAny<string>(), It.IsAny<object>()))
                .ReturnsAsync(user);

            var createUserCommand = new CreateUserCommand(
                user.Id,
                new Guid(),
                user.Login,
                user.CustomURL,
                "Random"
            );

            var createUserHandler = new CreateUserHandler(_dbService.Object);

            var result = await Assert.ThrowsAsync<BadHttpRequestException>(
                async () =>
                    await createUserHandler.Handle(createUserCommand, new CancellationToken())
            );
            Assert.Equal(
                "Password cannot be shorter than "
                    + authConfig.GetMinimumPasswordLength()
                    + " characters",
                result.Message
            );
        }

        [Fact]
        public async Task DoesPasswordMaxLengthWorkProperly()
        {
            var user = GetUserDetails();

            _dbService
                .Setup(s => s.Post<AlpimiAPI.User.User>(It.IsAny<string>(), It.IsAny<object>()))
                .ReturnsAsync(user);

            var createUserCommand = new CreateUserCommand(
                user.Id,
                new Guid(),
                user.Login,
                user.CustomURL,
                "RandomRandomRandomRandomRandomRandomRandomRandomRandomRandomRandomRandomRandomRandomRandomRandomRandomRandomRandomRandomRandomRandomRandomRandomRandomRandomRandomRandomRandomRandomRandomRandomRandomRandomRandomRandomRandomRandomRandomRandomRandomRandomRandomRandomRandomRandomRandomRandomRandomRandomRandomRandomRandomRandomRandomRandomRandomRandomRandomRandom"
            );

            var createUserHandler = new CreateUserHandler(_dbService.Object);

            var result = await Assert.ThrowsAsync<BadHttpRequestException>(
                async () =>
                    await createUserHandler.Handle(createUserCommand, new CancellationToken())
            );
            Assert.Equal(
                "Password cannot be longer than "
                    + authConfig.GetMaximumPasswordLength()
                    + " characters",
                result.Message
            );
        }

        [Fact]
        public async Task DoesPasswordContainsSmallLetterWorkProperly()
        {
            var user = GetUserDetails();

            _dbService
                .Setup(s => s.Post<AlpimiAPI.User.User>(It.IsAny<string>(), It.IsAny<object>()))
                .ReturnsAsync(user);

            var createUserCommand = new CreateUserCommand(
                user.Id,
                new Guid(),
                user.Login,
                user.CustomURL,
                "RANDOMBIG1!"
            );

            var createUserHandler = new CreateUserHandler(_dbService.Object);

            var result = await Assert.ThrowsAsync<BadHttpRequestException>(
                async () =>
                    await createUserHandler.Handle(createUserCommand, new CancellationToken())
            );
            Assert.Equal(
                "Password must contain at least one of the following: lower-case letters, upper-case letters, digits, special characters",
                result.Message
            );
        }

        [Fact]
        public async Task DoesPasswordContainsBigLetterWorkProperly()
        {
            var user = GetUserDetails();

            _dbService
                .Setup(s => s.Post<AlpimiAPI.User.User>(It.IsAny<string>(), It.IsAny<object>()))
                .ReturnsAsync(user);

            var createUserCommand = new CreateUserCommand(
                user.Id,
                new Guid(),
                user.Login,
                user.CustomURL,
                "randomsmall1!"
            );

            var createUserHandler = new CreateUserHandler(_dbService.Object);

            var result = await Assert.ThrowsAsync<BadHttpRequestException>(
                async () =>
                    await createUserHandler.Handle(createUserCommand, new CancellationToken())
            );
            Assert.Equal(
                "Password must contain at least one of the following: lower-case letters, upper-case letters, digits, special characters",
                result.Message
            );
        }

        [Fact]
        public async Task DoesPasswordContainsSymbolWorkProperly()
        {
            var user = GetUserDetails();

            _dbService
                .Setup(s => s.Post<AlpimiAPI.User.User>(It.IsAny<string>(), It.IsAny<object>()))
                .ReturnsAsync(user);

            var createUserCommand = new CreateUserCommand(
                user.Id,
                new Guid(),
                user.Login,
                user.CustomURL,
                "Randomsmall1"
            );

            var createUserHandler = new CreateUserHandler(_dbService.Object);

            var result = await Assert.ThrowsAsync<BadHttpRequestException>(
                async () =>
                    await createUserHandler.Handle(createUserCommand, new CancellationToken())
            );
            Assert.Equal(
                "Password must contain at least one of the following: lower-case letters, upper-case letters, digits, special characters",
                result.Message
            );
        }

        [Fact]
        public async Task DoesPasswordContainsDigitWorkProperly()
        {
            var user = GetUserDetails();

            _dbService
                .Setup(s => s.Post<AlpimiAPI.User.User>(It.IsAny<string>(), It.IsAny<object>()))
                .ReturnsAsync(user);

            var createUserCommand = new CreateUserCommand(
                user.Id,
                new Guid(),
                user.Login,
                user.CustomURL,
                "Randomsmall!"
            );

            var createUserHandler = new CreateUserHandler(_dbService.Object);

            var result = await Assert.ThrowsAsync<BadHttpRequestException>(
                async () =>
                    await createUserHandler.Handle(createUserCommand, new CancellationToken())
            );
            Assert.Equal(
                "Password must contain at least one of the following: lower-case letters, upper-case letters, digits, special characters",
                result.Message
            );
        }
    }
}
