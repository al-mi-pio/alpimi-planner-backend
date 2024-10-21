using AlpimiAPI;
using AlpimiAPI.Entities.User.Queries;
using Moq;
using Xunit;

namespace AlpimiTest.Unit.Entities.User.Queres
{
    public class GetUserCommandUnit
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
        public async Task GetsUserWhenIdIsCorrect()
        {
            var user = GetUserDetails();

            _dbService
                .Setup(s =>
                    s.Get<AlpimiAPI.Entities.User.User>(It.IsAny<string>(), It.IsAny<object>())
                )
                .ReturnsAsync(user);

            var getUserCommand = new GetUserQuery(user.Id);

            var getUserHandler = new GetUserHandler(_dbService.Object);

            var result = await getUserHandler.Handle(getUserCommand, new CancellationToken());

            Assert.Equal(user, result);
        }

        [Fact]
        public async Task ReturnsNullWhenIdIsIncorrect()
        {
            var user = GetUserDetails();

            _dbService
                .Setup(s =>
                    s.Get<AlpimiAPI.Entities.User.User>(It.IsAny<string>(), It.IsAny<object>())
                )
                .ReturnsAsync((AlpimiAPI.Entities.User.User?)null);

            var getUserCommand = new GetUserQuery(new Guid());

            var getUserHandler = new GetUserHandler(_dbService.Object);

            var result = await getUserHandler.Handle(getUserCommand, new CancellationToken());

            Assert.Null(result);
        }
    }
}
