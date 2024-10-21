using AlpimiAPI;
using AlpimiAPI.Entities.Auth.Queries;
using Moq;
using Xunit;

namespace AlpimiTest.Entities.Auth.Queries
{
    public class LoginQueryUnit
    {
        private readonly Mock<IDbService> _dbService = new();

        private AlpimiAPI.Entities.Auth.Auth GetAuthDetails()
        {
            var user = new AlpimiAPI.Entities.User.User()
            {
                Id = new Guid(),
                Login = "SaltFinal",
                CustomURL = "44f"
            };
            var auth = new AlpimiAPI.Entities.Auth.Auth()
            {
                Password = "RPhZLnao+2lWH4JvwGZRLI/14QI=",
                Id = new Guid(),
                Salt = "zr+8L0dX4IBdGUgvHDM1Zw==",
                UserID = user.Id,
                User = user
            };

            return auth;
        }

        [Fact]
        public async Task GivesTokenIfLoginAndPasswordAreCorrect()
        {
            var auth = GetAuthDetails();

            _dbService
                .Setup(s =>
                    s.Get<AlpimiAPI.Entities.User.User>(It.IsAny<string>(), It.IsAny<object>())
                )
                .ReturnsAsync(auth.User);
            _dbService
                .Setup(s =>
                    s.Post<AlpimiAPI.Entities.Auth.Auth>(It.IsAny<string>(), It.IsAny<object>())
                )
                .ReturnsAsync(auth);

            var loginCommand = new LoginQuery(auth.User.Login, "sssSSS1!");

            var loginHandler = new LoginHandler(_dbService.Object);

            var result = await loginHandler.Handle(loginCommand, new CancellationToken());

            Assert.IsType<String>(result);
        }

        [Fact]
        public async Task ThrowsErrorWhenIncorrectLoginIsGiven()
        {
            var auth = GetAuthDetails();

            _dbService
                .Setup(s =>
                    s.Get<AlpimiAPI.Entities.User.User>(It.IsAny<string>(), It.IsAny<object>())
                )
                .ReturnsAsync((AlpimiAPI.Entities.User.User?)null);
            _dbService
                .Setup(s =>
                    s.Post<AlpimiAPI.Entities.Auth.Auth>(It.IsAny<string>(), It.IsAny<object>())
                )
                .ReturnsAsync(auth);

            var loginCommand = new LoginQuery("wrongLogin", "sssSSS1!");

            var loginHandler = new LoginHandler(_dbService.Object);

            var result = await Assert.ThrowsAsync<BadHttpRequestException>(
                async () => await loginHandler.Handle(loginCommand, new CancellationToken())
            );

            Assert.Equal("Invalid login or password", result.Message);
        }

        [Fact]
        public async Task ThrowsErrorWhenIncorrectPasswordIsGiven()
        {
            var auth = GetAuthDetails();

            _dbService
                .Setup(s =>
                    s.Get<AlpimiAPI.Entities.User.User>(It.IsAny<string>(), It.IsAny<object>())
                )
                .ReturnsAsync(auth.User);
            _dbService
                .Setup(s =>
                    s.Post<AlpimiAPI.Entities.Auth.Auth>(It.IsAny<string>(), It.IsAny<object>())
                )
                .ReturnsAsync(auth);

            var loginCommand = new LoginQuery(auth.User.Login, "wrongPassword");

            var loginHandler = new LoginHandler(_dbService.Object);

            var result = await Assert.ThrowsAsync<BadHttpRequestException>(
                async () => await loginHandler.Handle(loginCommand, new CancellationToken())
            );

            Assert.Equal("Invalid password", result.Message);
        }
    }
}
