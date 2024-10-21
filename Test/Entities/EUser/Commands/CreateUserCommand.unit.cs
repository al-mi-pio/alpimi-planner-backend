using AlpimiAPI;
using AlpimiAPI.Entities.EUser;
using AlpimiAPI.Entities.EUser.Commands;
using AlpimiAPI.Settings;
using Moq;
using Xunit;

namespace AlpimiTest.Entities.EUser.Commands
{
    public class CreateUserCommandUnit
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
        public async Task CreatesUserWhenPaswordIsCorrect()
        {
            var user = GetUserDetails();

            _dbService
                .Setup(s => s.Post<User>(It.IsAny<string>(), It.IsAny<object>()))
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
        public async Task ThrowsErrorWhenPasswordIsTooShort()
        {
            var user = GetUserDetails();

            _dbService
                .Setup(s => s.Post<User>(It.IsAny<string>(), It.IsAny<object>()))
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
                    + AuthSettings.MinimumPasswordLength
                    + " characters",
                result.Message
            );
        }

        [Fact]
        public async Task ThrowsErrorWhenPasswordIsTooLong()
        {
            var user = GetUserDetails();

            _dbService
                .Setup(s => s.Post<User>(It.IsAny<string>(), It.IsAny<object>()))
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
                    + AuthSettings.MaximumPasswordLength
                    + " characters",
                result.Message
            );
        }

        [Fact]
        public async Task ThrowsErrorWhenPasswordDosentContainSmallLetters()
        {
            var user = GetUserDetails();

            _dbService
                .Setup(s => s.Post<User>(It.IsAny<string>(), It.IsAny<object>()))
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
            var requiredCharacters = AuthSettings.RequiredCharacters;
            if (requiredCharacters == null)
            {
                requiredCharacters = [RequiredCharacterTypes.SmallLetter];
            }
            Assert.Equal(
                "Password must contain at least one of the following: "
                    + string.Join(", ", requiredCharacters),
                result.Message
            );
        }

        [Fact]
        public async Task ThrowsErrorWhenPasswordDosentContainBigLetters()
        {
            var user = GetUserDetails();

            _dbService
                .Setup(s => s.Post<User>(It.IsAny<string>(), It.IsAny<object>()))
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
            var requiredCharacters = AuthSettings.RequiredCharacters;
            if (requiredCharacters == null)
            {
                requiredCharacters = [RequiredCharacterTypes.BigLetter];
            }
            Assert.Equal(
                "Password must contain at least one of the following: "
                    + string.Join(", ", requiredCharacters),
                result.Message
            );
        }

        [Fact]
        public async Task ThrowsErrorWhenPasswordDosentContainSymbols()
        {
            var user = GetUserDetails();

            _dbService
                .Setup(s => s.Post<User>(It.IsAny<string>(), It.IsAny<object>()))
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
            var requiredCharacters = AuthSettings.RequiredCharacters;
            if (requiredCharacters == null)
            {
                requiredCharacters = [RequiredCharacterTypes.Symbol];
            }
            Assert.Equal(
                "Password must contain at least one of the following: "
                    + string.Join(", ", requiredCharacters),
                result.Message
            );
        }

        [Fact]
        public async Task ThrowsErrorWhenPasswordDosentContainDigits()
        {
            var user = GetUserDetails();

            _dbService
                .Setup(s => s.Post<User>(It.IsAny<string>(), It.IsAny<object>()))
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
            var requiredCharacters = AuthSettings.RequiredCharacters;
            if (requiredCharacters == null)
            {
                requiredCharacters = [RequiredCharacterTypes.Digit];
            }
            Assert.Equal(
                "Password must contain at least one of the following: "
                    + string.Join(", ", requiredCharacters),
                result.Message
            );
        }

        [Fact]
        public async Task ThrowsErrorWhenLoginAlreadyExists()
        {
            var user = GetUserDetails();

            _dbService
                .Setup(s => s.Post<User>(It.IsAny<string>(), It.IsAny<object>()))
                .ReturnsAsync(user);
            _dbService
                .Setup(s => s.Get<User>(It.IsAny<string>(), It.IsAny<object>()))
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
            var requiredCharacters = AuthSettings.RequiredCharacters;

            Assert.Equal("Login already taken", result.Message);
        }

        [Fact]
        public async Task ThrowsErrorWhenURLAlreadyExists()
        {
            var user = GetUserDetails();

            _dbService
                .Setup(s => s.Post<User>(It.IsAny<string>(), It.IsAny<object>()))
                .ReturnsAsync(user);
            _dbService
                .Setup(s => s.Get<string>(It.IsAny<string>(), It.IsAny<object>()))
                .ReturnsAsync(user.CustomURL);
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

            Assert.Equal("URL already taken", result.Message);
        }
    }
}
