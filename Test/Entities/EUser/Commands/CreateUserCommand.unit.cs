using AlpimiAPI.Database;
using AlpimiAPI.Entities.EUser;
using AlpimiAPI.Entities.EUser.Commands;
using AlpimiAPI.Responses;
using AlpimiAPI.Settings;
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
    public class CreateUserCommandUnit
    {
        private readonly Mock<IDbService> _dbService = new();
        private readonly Mock<IStringLocalizer<Errors>> _str;

        public CreateUserCommandUnit()
        {
            _str = ResourceSetup.Setup();
        }

        [Fact]
        public async Task ThrowsErrorWhenPasswordIsTooShort()
        {
            var dto = MockData.GetCreateUserDTODetails();
            dto.Password = "Rand1!";

            var user = MockData.GetUserDetails();

            var createUserCommand = new CreateUserCommand(user.Id, new Guid(), dto);

            var createUserHandler = new CreateUserHandler(_dbService.Object, _str.Object);

            var result = await Assert.ThrowsAsync<ApiErrorException>(
                async () =>
                    await createUserHandler.Handle(createUserCommand, new CancellationToken())
            );

            Assert.Equal(
                JsonConvert.SerializeObject(
                    new ErrorObject[]
                    {
                        new ErrorObject(
                            "Password cannot be shorter than "
                                + AuthSettings.MinimumPasswordLength
                                + " characters"
                        )
                    }
                ),
                JsonConvert.SerializeObject(result.errors)
            );
        }

        [Fact]
        public async Task ThrowsErrorWhenPasswordIsTooLong()
        {
            var dto = MockData.GetCreateUserDTODetails();
            dto.Password =
                "RandomRandomRandomRandomRandomRandomRandomRandomRandomRandomRandomRandomRandomRandomRandomRandomRandomRandomRandomRandomRandomRandomRandomRandomRandomRandomRandomRandomRandomRandomRandomRandomRandomRandomRandomRandomRandomRandomRandomRandomRandomRandomRandomRandomRandomRandomRandomRandomRandomRandomRandomRandomRandomRandomRandomRandomRandomRandomRandomRandom1!";

            var user = MockData.GetUserDetails();

            var createUserCommand = new CreateUserCommand(user.Id, new Guid(), dto);

            var createUserHandler = new CreateUserHandler(_dbService.Object, _str.Object);

            var result = await Assert.ThrowsAsync<ApiErrorException>(
                async () =>
                    await createUserHandler.Handle(createUserCommand, new CancellationToken())
            );

            Assert.Equal(
                JsonConvert.SerializeObject(
                    new ErrorObject[]
                    {
                        new ErrorObject(
                            "Password cannot be longer than "
                                + AuthSettings.MaximumPasswordLength
                                + " characters"
                        )
                    }
                ),
                JsonConvert.SerializeObject(result.errors)
            );
        }

        [Fact]
        public async Task ThrowsErrorWhenPasswordDosentContainSmallLetters()
        {
            var dto = MockData.GetCreateUserDTODetails();
            dto.Password = "RANDOMBIG1!";

            var user = MockData.GetUserDetails();

            var createUserCommand = new CreateUserCommand(user.Id, new Guid(), dto);

            var createUserHandler = new CreateUserHandler(_dbService.Object, _str.Object);

            var result = await Assert.ThrowsAsync<ApiErrorException>(
                async () =>
                    await createUserHandler.Handle(createUserCommand, new CancellationToken())
            );
            var requiredCharacters = AuthSettings.RequiredCharacters;

            Assert.Equal(
                JsonConvert.SerializeObject(
                    new ErrorObject[]
                    {
                        new ErrorObject(
                            "Password must contain at least one of the following: "
                                + string.Join(", ", requiredCharacters!)
                        )
                    }
                ),
                JsonConvert.SerializeObject(result.errors)
            );
        }

        [Fact]
        public async Task ThrowsErrorWhenPasswordDosentContainBigLetters()
        {
            var dto = MockData.GetCreateUserDTODetails();
            dto.Password = "randomsmall1!";

            var user = MockData.GetUserDetails();

            var createUserCommand = new CreateUserCommand(user.Id, new Guid(), dto);

            var createUserHandler = new CreateUserHandler(_dbService.Object, _str.Object);

            var result = await Assert.ThrowsAsync<ApiErrorException>(
                async () =>
                    await createUserHandler.Handle(createUserCommand, new CancellationToken())
            );
            var requiredCharacters = AuthSettings.RequiredCharacters;

            Assert.Equal(
                JsonConvert.SerializeObject(
                    new ErrorObject[]
                    {
                        new ErrorObject(
                            "Password must contain at least one of the following: "
                                + string.Join(", ", requiredCharacters!)
                        )
                    }
                ),
                JsonConvert.SerializeObject(result.errors)
            );
        }

        [Fact]
        public async Task ThrowsErrorWhenPasswordDosentContainSymbols()
        {
            var dto = MockData.GetCreateUserDTODetails();
            dto.Password = "Randomsmall1";

            var user = MockData.GetUserDetails();

            var createUserCommand = new CreateUserCommand(user.Id, new Guid(), dto);

            var createUserHandler = new CreateUserHandler(_dbService.Object, _str.Object);

            var result = await Assert.ThrowsAsync<ApiErrorException>(
                async () =>
                    await createUserHandler.Handle(createUserCommand, new CancellationToken())
            );
            var requiredCharacters = AuthSettings.RequiredCharacters;

            Assert.Equal(
                JsonConvert.SerializeObject(
                    new ErrorObject[]
                    {
                        new ErrorObject(
                            "Password must contain at least one of the following: "
                                + string.Join(", ", requiredCharacters!)
                        )
                    }
                ),
                JsonConvert.SerializeObject(result.errors)
            );
        }

        [Fact]
        public async Task ThrowsErrorWhenPasswordDosentContainDigits()
        {
            var dto = MockData.GetCreateUserDTODetails();
            dto.Password = "Randomsmall!";

            var user = MockData.GetUserDetails();

            var createUserCommand = new CreateUserCommand(user.Id, new Guid(), dto);

            var createUserHandler = new CreateUserHandler(_dbService.Object, _str.Object);

            var result = await Assert.ThrowsAsync<ApiErrorException>(
                async () =>
                    await createUserHandler.Handle(createUserCommand, new CancellationToken())
            );
            var requiredCharacters = AuthSettings.RequiredCharacters;

            Assert.Equal(
                JsonConvert.SerializeObject(
                    new ErrorObject[]
                    {
                        new ErrorObject(
                            "Password must contain at least one of the following: "
                                + string.Join(", ", requiredCharacters!)
                        )
                    }
                ),
                JsonConvert.SerializeObject(result.errors)
            );
        }

        [Fact]
        public async Task ThrowsErrorWhenLoginAlreadyExists()
        {
            var dto = MockData.GetCreateUserDTODetails();

            var user = MockData.GetUserDetails();

            _dbService
                .Setup(s => s.Get<User>(It.IsAny<string>(), It.IsAny<object>()))
                .ReturnsAsync(user);

            var createUserCommand = new CreateUserCommand(user.Id, new Guid(), dto);

            var createUserHandler = new CreateUserHandler(_dbService.Object, _str.Object);

            var result = await Assert.ThrowsAsync<ApiErrorException>(
                async () =>
                    await createUserHandler.Handle(createUserCommand, new CancellationToken())
            );
            var requiredCharacters = AuthSettings.RequiredCharacters;

            Assert.Equal(
                JsonConvert.SerializeObject(
                    new ErrorObject[]
                    {
                        new ErrorObject("There is already a User with the name Marek")
                    }
                ),
                JsonConvert.SerializeObject(result.errors)
            );
        }

        [Fact]
        public async Task ThrowsErrorWhenURLAlreadyExists()
        {
            var dto = MockData.GetCreateUserDTODetails();

            var user = MockData.GetUserDetails();

            _dbService
                .Setup(s => s.Get<string>(It.IsAny<string>(), It.IsAny<object>()))
                .ReturnsAsync(user.CustomURL);
            var createUserCommand = new CreateUserCommand(user.Id, new Guid(), dto);

            var createUserHandler = new CreateUserHandler(_dbService.Object, _str.Object);

            var result = await Assert.ThrowsAsync<ApiErrorException>(
                async () =>
                    await createUserHandler.Handle(createUserCommand, new CancellationToken())
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

        [Fact]
        public async Task ThrowsMultipleErrorMessages()
        {
            var dto = MockData.GetCreateUserDTODetails();
            dto.Password = "R1!";

            var user = MockData.GetUserDetails();

            var createUserCommand = new CreateUserCommand(user.Id, new Guid(), dto);

            var createUserHandler = new CreateUserHandler(_dbService.Object, _str.Object);

            var result = await Assert.ThrowsAsync<ApiErrorException>(
                async () =>
                    await createUserHandler.Handle(createUserCommand, new CancellationToken())
            );
            var requiredCharacters = AuthSettings.RequiredCharacters;

            Assert.Equal(
                JsonConvert.SerializeObject(
                    new ErrorObject[]
                    {
                        new ErrorObject(
                            "Password cannot be shorter than "
                                + AuthSettings.MinimumPasswordLength
                                + " characters"
                        ),
                        new ErrorObject(
                            "Password must contain at least one of the following: "
                                + string.Join(", ", requiredCharacters!)
                        )
                    }
                ),
                JsonConvert.SerializeObject(result.errors)
            );
        }
    }
}
