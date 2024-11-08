using AlpimiAPI.Database;
using AlpimiAPI.Entities.EUser;
using AlpimiAPI.Entities.EUser.Queries;
using AlpimiTest.TestUtilities;
using Moq;
using Xunit;

namespace AlpimiTest.Entities.WUser.Queres
{
    public class GetUserCommandUnit
    {
        private readonly Mock<IDbService> _dbService = new();

        [Fact]
        public async Task GetsUserWhenIdIsCorrect()
        {
            var user = MockData.GetUserDetails();

            _dbService
                .Setup(s => s.Get<User>(It.IsAny<string>(), It.IsAny<object>()))
                .ReturnsAsync(user);

            var getUserCommand = new GetUserQuery(user.Id, new Guid(), "Admin");

            var getUserHandler = new GetUserHandler(_dbService.Object);

            var result = await getUserHandler.Handle(getUserCommand, new CancellationToken());

            Assert.Equal(user, result);
        }
    }
}
