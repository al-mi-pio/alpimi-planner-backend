using AlpimiAPI.Database;
using AlpimiAPI.Entities.EUser;
using AlpimiAPI.Entities.EUser.Commands;
using AlpimiAPI.Locales;
using AlpimiAPI.Responses;
using AlpimiAPI.Utilities;
using AlpimiTest.TestSetup;
using AlpimiTest.TestUtilities;
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
        private readonly Mock<IStringLocalizer<Fields>> _strFields;

        public UpdateUserCommandUnit()
        {
            _str = ResourceSetup.Setup();
            _strFields = ResourceSetup.FieldSetup();
        }

        [Fact]
        public async Task ThrowsErrorWhenLoginAlreadyExists()
        {
            var dto = MockData.GetUpdateUserDTODetails();
            _dbService
                .Setup(s => s.Get<User>(It.IsAny<string>(), It.IsAny<object>()))
                .ReturnsAsync(MockData.GetUserDetails());
            _dbService
                .Setup(s => s.GetAll<string>(It.IsAny<string>(), It.IsAny<object>()))
                .ReturnsAsync(["TakenLogin"]);

            var updateUserCommand = new UpdateUserCommand(new Guid(), dto, new Guid(), "Admin");
            var updateUserHandler = new UpdateUserHandler(
                _dbService.Object,
                _str.Object,
                _strFields.Object
            );
            var result = await Assert.ThrowsAsync<ApiErrorException>(
                async () =>
                    await updateUserHandler.Handle(updateUserCommand, new CancellationToken())
            );

            Assert.Equal(
                JsonConvert.SerializeObject(
                    new ErrorObject[]
                    {
                        new FieldErrorObject(
                            "login",
                            "There is already a Login with the name UpdatedMarek"
                        )
                    }
                ),
                JsonConvert.SerializeObject(result.errors)
            );
        }

        [Fact]
        public async Task ThrowsErrorWhenURLAlreadyExists()
        {
            var dto = MockData.GetUpdateUserDTODetails();
            _dbService
                .Setup(s => s.Get<User>(It.IsAny<string>(), It.IsAny<object>()))
                .ReturnsAsync(MockData.GetUserDetails());
            _dbService
                .Setup(s => s.Get<string>(It.IsAny<string>(), It.IsAny<object>()))
                .ReturnsAsync("TakenURL");

            var updateUserCommand = new UpdateUserCommand(new Guid(), dto, new Guid(), "Admin");
            var updateUserHandler = new UpdateUserHandler(
                _dbService.Object,
                _str.Object,
                _strFields.Object
            );
            var result = await Assert.ThrowsAsync<ApiErrorException>(
                async () =>
                    await updateUserHandler.Handle(updateUserCommand, new CancellationToken())
            );

            Assert.Equal(
                JsonConvert.SerializeObject(
                    new ErrorObject[]
                    {
                        new FieldErrorObject(
                            "customUrl",
                            "There is already a Custom URL with the name UpdatedURL"
                        )
                    }
                ),
                JsonConvert.SerializeObject(result.errors)
            );
        }

        [Fact]
        public async Task ThrowsErrorWhenLoginContainsAnIllegalSymbol()
        {
            var dto = MockData.GetUpdateUserDTODetails();
            dto.Login = "る";
            _dbService
                .Setup(s => s.Get<User>(It.IsAny<string>(), It.IsAny<object>()))
                .ReturnsAsync(MockData.GetUserDetails());

            var updateUserCommand = new UpdateUserCommand(new Guid(), dto, new Guid(), "Admin");
            var updateUserHandler = new UpdateUserHandler(
                _dbService.Object,
                _str.Object,
                _strFields.Object
            );
            var result = await Assert.ThrowsAsync<ApiErrorException>(
                async () =>
                    await updateUserHandler.Handle(updateUserCommand, new CancellationToken())
            );
            var allowedCharacters = Configuration.GetAllowedCharacterTypesForLogin();

            Assert.Equal(
                JsonConvert.SerializeObject(
                    new ErrorObject[]
                    {
                        new ErrorObject(
                            "Login can only contain the following: "
                                + string.Join(", ", allowedCharacters!)
                        )
                    }
                ),
                JsonConvert.SerializeObject(result.errors)
            );
        }

        [Fact]
        public async Task ThrowsErrorWhenCustomURLContainsAnIllegalSymbol()
        {
            var dto = MockData.GetUpdateUserDTODetails();
            dto.CustomURL = "る";
            _dbService
                .Setup(s => s.Get<User>(It.IsAny<string>(), It.IsAny<object>()))
                .ReturnsAsync(MockData.GetUserDetails());

            var updateUserCommand = new UpdateUserCommand(new Guid(), dto, new Guid(), "Admin");
            var updateUserHandler = new UpdateUserHandler(
                _dbService.Object,
                _str.Object,
                _strFields.Object
            );
            var result = await Assert.ThrowsAsync<ApiErrorException>(
                async () =>
                    await updateUserHandler.Handle(updateUserCommand, new CancellationToken())
            );
            var allowedCharacters = Configuration.GetAllowedCharacterTypesForCustomURL();

            Assert.Equal(
                JsonConvert.SerializeObject(
                    new ErrorObject[]
                    {
                        new ErrorObject(
                            "CustomURL can only contain the following: "
                                + string.Join(", ", allowedCharacters!)
                        )
                    }
                ),
                JsonConvert.SerializeObject(result.errors)
            );
        }
    }
}
