using AlpimiAPI.Database;
using AlpimiAPI.Entities.EUser;
using AlpimiAPI.Entities.EUser.Commands;
using AlpimiAPI.Responses;
using AlpimiTest.TestSetup;
using AlpimiTest.TestUtilities;
using alpimi_planner_backend.API.Locales;
using Microsoft.Extensions.Localization;
using Moq;
using Newtonsoft.Json;
using Xunit;

namespace AlpimiTest.Entities.EUser.Commands
{
    [Collection("Sequential Tests")]
    public class UpdateUserCommandUnit
    {
        private readonly Mock<IDbService> _dbService = new();
        private readonly Mock<IStringLocalizer<Errors>> _str;

        public UpdateUserCommandUnit()
        {
            _str = ResourceSetup.Setup();
        }

        [Fact]
        public async Task ThrowsErrorWhenURLAlreadyExists()
        {
            var user = MockData.GetUserDetails();

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
