using AlpimiAPI;
using AlpimiAPI.Entities.User.Queries;
using Moq;
using Xunit;

namespace AlpimiTest.Unit.Entities.User.Queres
{
    public class GetUserByLoginCommandUnit
    {
        private readonly Mock<IDbService> _dbService = new();

        private AlpimiAPI.Entities.User.User GetUserDetails()
        {
            var user = new AlpimiAPI.Entities.User.User()
            {
                Id = new Guid(),
                Login = "marek",
                CustomURL = "44f"
            };

            return user;
        }

        [Fact]
        public async Task GetsUserWhenLoginIsCorrect()
        {
            var user = GetUserDetails();

            _dbService
                .Setup(s =>
                    s.Get<AlpimiAPI.Entities.User.User>(It.IsAny<string>(), It.IsAny<object>())
                )
                .ReturnsAsync(user);

            var getUserCommand = new GetUserByLoginQuery(user.Login);

            var getUserHandler = new GetUserByLoginHandler(_dbService.Object);

            var result = await getUserHandler.Handle(getUserCommand, new CancellationToken());

            Assert.Equal(user, result);
        }

        [Fact]
        public async Task ReturnsNullWhenLoginIsIncorrect()
        {
            var user = GetUserDetails();

            _dbService
                .Setup(s =>
                    s.Get<AlpimiAPI.Entities.User.User>(It.IsAny<string>(), It.IsAny<object>())
                )
                .ReturnsAsync((AlpimiAPI.Entities.User.User?)null);

            var getUserCommand = new GetUserByLoginQuery("NieMarek");

            var getUserHandler = new GetUserByLoginHandler(_dbService.Object);

            var result = await getUserHandler.Handle(getUserCommand, new CancellationToken());

            Assert.Null(result);
        }
    }
}
