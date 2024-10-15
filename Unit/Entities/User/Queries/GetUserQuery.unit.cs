using System.Net;
using AlpimiAPI.User.Commands;
using AlpimiAPI.User.Queries;
using alpimi_planner_backend.API;
using Microsoft.EntityFrameworkCore;
using Moq;
using Xunit;

namespace alpimi_planner_backend.Unit.Entities.User.Queres
{
    public class GetUserCommandUnit
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
        public async Task IsGetCalledProperly()
        {
            var user = GetUserDetails();

            _dbService
                .Setup(s => s.Get<AlpimiAPI.User.User>(It.IsAny<string>(), It.IsAny<object>()))
                .ReturnsAsync(user);

            var getUserCommand = new GetUserQuery(user.Id);

            var getUserHandler = new GetUserHandler(_dbService.Object);

            var result = await getUserHandler.Handle(getUserCommand, new CancellationToken());

            Assert.Equal(user, result);
        }
    }
}
