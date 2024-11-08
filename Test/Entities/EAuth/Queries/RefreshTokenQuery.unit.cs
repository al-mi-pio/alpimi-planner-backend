using AlpimiAPI.Database;
using AlpimiAPI.Entities.EAuth.Queries;
using Moq;
using Xunit;

namespace AlpimiTest.Entities.EAuth.Queries
{
    public class RefreshTokenQueryUnit
    {
        private readonly Mock<IDbService> _dbService = new();

        [Fact]
        public void GivesRefreshedToken()
        {
            _dbService
                .Setup(s => s.Get<String>(It.IsAny<string>(), It.IsAny<object>()))
                .ReturnsAsync("Token");

            var refreshTokenCommand = new RefreshTokenQuery("string", new Guid(), "string");

            var refreshTokenHandler = new RefreshTokenHandler();

            var result = refreshTokenHandler.Handle(refreshTokenCommand, new CancellationToken());

            Assert.IsType<String>(result);
        }
    }
}
