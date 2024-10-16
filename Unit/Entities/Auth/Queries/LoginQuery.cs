using AlpimiAPI.Auth.Queries;
using AlpimiAPI.User;
using AlpimiAPI.User.Queries;
using alpimi_planner_backend.API;
using Moq;
using Xunit;

namespace alpimi_planner_backend.Unit.Entities.Auth.Queries
{
    public class LoginQueryUnit
    {
        private readonly Mock<IDbService> _dbService = new();

        private AlpimiAPI.Auth.Auth GetAuthDetails()
        {
            var user = new AlpimiAPI.User.User()
            {
                Id = new Guid(),
                Login = "marek",
                CustomURL = "44f"
            };
            var auth = new AlpimiAPI.Auth.Auth()
            {
                Password = "A665A45920422F9D417E4867EFDC4FB8A04A1F3FFF1FA07E998E86F7F7A27AE3",
                Id = new Guid(),
                UserID = user.Id,
                User = user
            };

            return auth;
        }

        [Fact]
        public async Task IsLoginCalledProperly()
        {
            var auth = GetAuthDetails();

            _dbService
                .Setup(s => s.Post<AlpimiAPI.User.User>(It.IsAny<string>(), It.IsAny<object>()))
                .ReturnsAsync(auth.User);
            _dbService
                .Setup(s => s.Post<AlpimiAPI.Auth.Auth>(It.IsAny<string>(), It.IsAny<object>()))
                .ReturnsAsync(auth);
            Environment.SetEnvironmentVariable("JWT_KEY", "VeryLongFakeJWT_ThatWeMockedForTests");

            var loginCommand = new LoginQuery(auth.User.Login, "123");

            var loginHandler = new LoginHandler(_dbService.Object);

            var result = await loginHandler.Handle(loginCommand, new CancellationToken());

            Assert.IsType<String>(result);
        }
    }
}
