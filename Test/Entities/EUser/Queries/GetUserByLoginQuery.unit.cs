using AlpimiAPI.Database;
using AlpimiAPI.Entities.EUser;
using AlpimiAPI.Entities.EUser.Queries;
using AlpimiTest.TestUtilities;
using Moq;
using Xunit;

namespace AlpimiTest.Entities.EUser.Queres
{
    public class GetUserByLoginCommandUnit
    {
        private readonly Mock<IDbService> _dbService = new();

        [Fact]
        public async Task GetsUserWhenLoginIsCorrect()
        {
            var user = MockData.GetUserDetails();

            _dbService
                .Setup(s => s.Get<User>(It.IsAny<string>(), It.IsAny<object>()))
                .ReturnsAsync(user);

            var getUserCommand = new GetUserByLoginQuery(user.Login, new Guid(), "Admin");

            var getUserHandler = new GetUserByLoginHandler(_dbService.Object);

            var result = await getUserHandler.Handle(getUserCommand, new CancellationToken());

            Assert.Equal(user, result);
        }

        [Fact]
        public async Task ReturnsNullWhenLoginIsIncorrect()
        {
            var user = MockData.GetUserDetails();

            _dbService
                .Setup(s => s.Get<User>(It.IsAny<string>(), It.IsAny<object>()))
                .ReturnsAsync((User?)null);

            var getUserCommand = new GetUserByLoginQuery("NieMarek", new Guid(), "Admin");

            var getUserHandler = new GetUserByLoginHandler(_dbService.Object);

            var result = await getUserHandler.Handle(getUserCommand, new CancellationToken());

            Assert.Null(result);
        }

        [Fact]
        public async Task ReturnsNullWhenWrongUserGetsDetails()
        {
            var user = MockData.GetUserDetails();

            _dbService
                .Setup(s => s.Get<User>(It.IsAny<string>(), It.IsAny<object>()))
                .ReturnsAsync((User?)null);

            var getUserCommand = new GetUserByLoginQuery(user.Login, new Guid(), "User");

            var getUserHandler = new GetUserByLoginHandler(_dbService.Object);

            var result = await getUserHandler.Handle(getUserCommand, new CancellationToken());

            Assert.Null(result);
        }
    }
}
