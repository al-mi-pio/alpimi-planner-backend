using AlpimiAPI.User.Commands;
using alpimi_planner_backend.API;
using Moq;
using Xunit;

namespace alpimi_planner_backend.Unit.Entities.User.Commands
{
    public class CreateUserCommandUnit
    {
        private readonly Mock<IDbService> _dbService = new();

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
                .Setup(s => s.Create<AlpimiAPI.User.User>(It.IsAny<string>(), It.IsAny<object>()))
                .ReturnsAsync(user);

            var createUserCommand = new CreateUserCommand(user.Id, user.Login, user.CustomURL);

            var createUserHandler = new CreateUserHandler(_dbService.Object);

            var result = await createUserHandler.Handle(createUserCommand, new CancellationToken());

            Assert.Equal(user.Id, result);
        }
    }
}
