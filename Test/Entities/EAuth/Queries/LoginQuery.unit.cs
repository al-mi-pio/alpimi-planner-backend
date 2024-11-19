using AlpimiAPI.Database;
using AlpimiAPI.Entities.EAuth;
using AlpimiAPI.Entities.EAuth.Queries;
using AlpimiAPI.Entities.EUser;
using AlpimiAPI.Responses;
using AlpimiTest.TestSetup;
using AlpimiTest.TestUtilities;
using alpimi_planner_backend.API.Locales;
using Microsoft.Extensions.Localization;
using Moq;
using Newtonsoft.Json;
using Xunit;

namespace AlpimiTest.Entities.EAuth.Queries
{
    [Collection("Sequential Tests")]
    public class LoginQueryUnit
    {
        private readonly Mock<IDbService> _dbService = new();
        private readonly Mock<IStringLocalizer<Errors>> _str;

        public LoginQueryUnit()
        {
            _str = ResourceSetup.Setup();
        }

        [Fact]
        public async Task ThrowsErrorWhenIncorrectLoginIsGiven()
        {
            var auth = MockData.GetAuthDetails();

            _dbService
                .Setup(s => s.Get<User>(It.IsAny<string>(), It.IsAny<object>()))
                .ReturnsAsync((User?)null);

            var loginCommand = new LoginQuery("wrongLogin", "sssSSS1!");

            var loginHandler = new LoginHandler(_dbService.Object, _str.Object);

            var result = await Assert.ThrowsAsync<ApiErrorException>(
                async () => await loginHandler.Handle(loginCommand, new CancellationToken())
            );

            Assert.Equal(
                JsonConvert.SerializeObject(
                    new ErrorObject[] { new ErrorObject("Invalid login or password") }
                ),
                JsonConvert.SerializeObject(result.errors)
            );
        }

        [Fact]
        public async Task ThrowsErrorWhenIncorrectPasswordIsGiven()
        {
            var auth = MockData.GetAuthDetails();
            _dbService
                .Setup(s => s.Post<Auth>(It.IsAny<string>(), It.IsAny<object>()))
                .ReturnsAsync(auth);
            _dbService
                .Setup(s => s.Get<User>(It.IsAny<string>(), It.IsAny<object>()))
                .ReturnsAsync(auth.User);

            var loginCommand = new LoginQuery(auth.User.Login, "wrongPassword");

            var loginHandler = new LoginHandler(_dbService.Object, _str.Object);

            var result = await Assert.ThrowsAsync<ApiErrorException>(
                async () => await loginHandler.Handle(loginCommand, new CancellationToken())
            );

            Assert.Equal(
                JsonConvert.SerializeObject(
                    new ErrorObject[] { new ErrorObject("Invalid password") }
                ),
                JsonConvert.SerializeObject(result.errors)
            );
        }
    }
}
