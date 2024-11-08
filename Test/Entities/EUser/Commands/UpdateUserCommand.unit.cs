using AlpimiAPI.Database;
using AlpimiAPI.Entities.EUser;
using AlpimiAPI.Entities.EUser.Commands;
using AlpimiAPI.Responses;
using AlpimiTest.TestUtilities;
using alpimi_planner_backend.API.Locales;
using Microsoft.Extensions.Localization;
using Moq;
using Newtonsoft.Json;
using Xunit;

namespace AlpimiTest.Entities.EUser.Commands
{
    public class UpdateUserCommandUnit
    {
        private readonly Mock<IDbService> _dbService = new();

        [Fact]
        public async Task ReturnsUpdatedUserWhenIdIsCorrect()
        {
            var user = MockData.GetUserDetails();
            Mock<IStringLocalizer<Errors>> _str = await ResourceSetup.Setup();

            _dbService
                .Setup(s => s.Update<User>(It.IsAny<string>(), It.IsAny<object>()))
                .ReturnsAsync(user);

            var updateUserCommand = new UpdateUserCommand(
                user.Id,
                "marek2",
                "f44",
                new Guid(),
                "Admin"
            );

            var updateUserHandler = new UpdateUserHandler(_dbService.Object, _str.Object);

            var result = await updateUserHandler.Handle(updateUserCommand, new CancellationToken());

            Assert.Equal(user, result);
        }

        [Fact]
        public async Task ThrowsErrorWhenURLAlreadyExists()
        {
            var user = MockData.GetUserDetails();
            Mock<IStringLocalizer<Errors>> _str = await ResourceSetup.Setup();

            _dbService
                .Setup(s => s.Update<User>(It.IsAny<string>(), It.IsAny<object>()))
                .ReturnsAsync(user);
            _dbService
                .Setup(s => s.Get<string>(It.IsAny<string>(), It.IsAny<object>()))
                .ReturnsAsync(user.CustomURL);

            var updateUserCommand = new UpdateUserCommand(
                user.Id,
                user.Login,
                user.CustomURL,
                new Guid(),
                "Admin"
            );

            var updateUserHandler = new UpdateUserHandler(_dbService.Object, _str.Object);

            var result = await Assert.ThrowsAsync<ApiErrorException>(
                async () =>
                    await updateUserHandler.Handle(updateUserCommand, new CancellationToken())
            );

            Assert.Equal(
                JsonConvert.SerializeObject(
                    new ErrorObject[]
                    {
                        new ErrorObject("There is already a URL with the name 44f")
                    }
                ),
                JsonConvert.SerializeObject(result.errors)
            );
        }
    }
}
