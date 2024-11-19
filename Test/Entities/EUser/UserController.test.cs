using System.Net;
using System.Net.Http.Headers;
using AlpimiAPI.Entities.EUser;
using AlpimiAPI.Entities.EUser.DTO;
using AlpimiAPI.Responses;
using AlpimiAPI.Settings;
using AlpimiTest.TestUtilities;
using Xunit;

namespace AlpimiTest.Entities.EUser
{
    [Collection("Sequential Tests")]
    public class UserControllerTest : IAsyncLifetime
    {
        CustomWebApplicationFactory<Program> _factory;
        HttpClient _client;

        public UserControllerTest()
        {
            _factory = new CustomWebApplicationFactory<Program>();
            _client = _factory.CreateClient();
        }

        public async Task InitializeAsync()
        {
            await DbHelper.UserCleaner(_client);
        }

        public async Task DisposeAsync()
        {
            await DbHelper.UserCleaner(_client);
        }

        [Fact]
        public async Task UserIsDeleted()
        {
            _client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue(
                "Bearer",
                TestAuthorization.GetToken("Admin", "User", new Guid())
            );
            var userId = await DbHelper.SetupUser(_client, MockData.GetCreateUserDTODetails());

            var response = await _client.DeleteAsync($"/api/User/{userId}");
            Assert.Equal(HttpStatusCode.NoContent, response.StatusCode);

            response = await _client.GetAsync($"/api/User/{userId}");
            Assert.Equal(HttpStatusCode.NotFound, response.StatusCode);
        }

        [Fact]
        public async Task DeleteUserThrowsForbiddenErrorWhenNoTokenIsGiven()
        {
            _client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue(
                "Bearer",
                TestAuthorization.GetToken("User", "User", new Guid())
            );
            var response = await _client.DeleteAsync($"/api/User/{new Guid()}");

            Assert.Equal(HttpStatusCode.Forbidden, response.StatusCode);
        }

        [Fact]
        public async Task UserIsCreated()
        {
            var userRequest = MockData.GetCreateUserDTODetails();

            _client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue(
                "Bearer",
                TestAuthorization.GetToken("Admin", "Bob", new Guid())
            );

            var response = await _client.PostAsJsonAsync("/api/User", userRequest);
            var jsonId = await response.Content.ReadFromJsonAsync<ApiGetResponse<Guid>>();

            Assert.Equal(HttpStatusCode.OK, response.StatusCode);

            response = await _client.GetAsync($"/api/User/{jsonId!.Content}");
            Assert.Equal(HttpStatusCode.OK, response.StatusCode);
        }

        [Fact]
        public async Task CreateUserThrowsForbiddenErrorWhenWrongTokenIsGiven()
        {
            var user = MockData.GetUserDetails();
            var userRequest = new CreateUserDTO
            {
                Login = user.Login,
                CustomURL = user.CustomURL!,
                Password = "sssSSS1!"
            };

            _client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue(
                "Bearer",
                TestAuthorization.GetToken("User", user.Login, user.Id)
            );

            var response = await _client.PostAsJsonAsync("/api/User", userRequest);

            Assert.Equal(HttpStatusCode.Forbidden, response.StatusCode);
        }

        [Fact]
        public async Task UpdateUserReturnsUpdatedUser()
        {
            var userUpdateRequest = MockData.GetUpdateUserDTODetails();

            var userId = await DbHelper.SetupUser(_client, MockData.GetCreateUserDTODetails());

            _client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue(
                "Bearer",
                TestAuthorization.GetToken("Admin", "User", new Guid())
            );
            var response = await _client.PatchAsJsonAsync($"/api/User/{userId}", userUpdateRequest);
            var jsonResponse = await response.Content.ReadFromJsonAsync<ApiGetResponse<User>>();

            Assert.Equal(userUpdateRequest.Login, jsonResponse!.Content.Login);
            Assert.Equal(userUpdateRequest.CustomURL, jsonResponse!.Content.CustomURL);
        }

        [Fact]
        public async Task UpdateUserThrowsNotFoundErrorWhenWrongIdIsGiven()
        {
            var userUpdateRequest = MockData.GetUpdateUserDTODetails();

            await DbHelper.SetupUser(_client, MockData.GetCreateUserDTODetails());

            _client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue(
                "Bearer",
                TestAuthorization.GetToken("Admin", "User", new Guid())
            );
            var response = await _client.PatchAsJsonAsync(
                $"/api/User/{new Guid()}",
                userUpdateRequest
            );
            await response.Content.ReadFromJsonAsync<ApiGetResponse<User>>();

            Assert.Equal(HttpStatusCode.NotFound, response.StatusCode);
        }

        [Fact]
        public async Task UpdateUserThrowsNotFoundErrorWhenWrongUserAttemptsUpdate()
        {
            var userUpdateRequest = MockData.GetUpdateUserDTODetails();

            var userId = await DbHelper.SetupUser(_client, MockData.GetCreateUserDTODetails());

            _client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue(
                "Bearer",
                TestAuthorization.GetToken("User", "User", new Guid())
            );
            var response = await _client.PatchAsJsonAsync($"/api/User/{userId}", userUpdateRequest);
            var jsonResponse = await response.Content.ReadFromJsonAsync<ApiGetResponse<User>>();

            Assert.Equal(HttpStatusCode.NotFound, response.StatusCode);
        }

        [Fact]
        public async Task GetUserReturnsUser()
        {
            var userRequest = MockData.GetCreateUserDTODetails();

            var userId = await DbHelper.SetupUser(_client, userRequest);

            _client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue(
                "Bearer",
                TestAuthorization.GetToken("Admin", "User", new Guid())
            );
            var response = await _client.GetAsync($"/api/User/{userId}");
            var jsonResponse = await response.Content.ReadFromJsonAsync<ApiGetResponse<User>>();

            Assert.Equal(userRequest.Login, jsonResponse!.Content.Login);
            Assert.Equal(userRequest.CustomURL, jsonResponse!.Content.CustomURL);
        }

        [Fact]
        public async Task GetUserThrowsNotFoundErrorWhenWrongUserAttemptsGet()
        {
            var userId = await DbHelper.SetupUser(_client, MockData.GetCreateUserDTODetails());

            _client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue(
                "Bearer",
                TestAuthorization.GetToken("User", "User", new Guid())
            );
            var response = await _client.GetAsync($"/api/User/{userId}");

            Assert.Equal(HttpStatusCode.NotFound, response.StatusCode);
        }

        [Fact]
        public async Task GetUserThrowsNotFoundErrorWhenWhenWrongIdIsGiven()
        {
            await DbHelper.SetupUser(_client, MockData.GetCreateUserDTODetails());

            _client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue(
                "Bearer",
                TestAuthorization.GetToken("Admin", "User", new Guid())
            );
            var response = await _client.GetAsync($"/api/User/{new Guid()}");

            Assert.Equal(HttpStatusCode.NotFound, response.StatusCode);
        }

        [Fact]
        public async Task GetUserByLoginReturnsUser()
        {
            var userRequest = MockData.GetCreateUserDTODetails();

            await DbHelper.SetupUser(_client, userRequest);

            _client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue(
                "Bearer",
                TestAuthorization.GetToken("Admin", "User", new Guid())
            );
            var response = await _client.GetAsync($"/api/User/byLogin/{userRequest.Login}");
            var jsonResponse = await response.Content.ReadFromJsonAsync<ApiGetResponse<User>>();

            Assert.Equal(userRequest.Login, jsonResponse!.Content.Login);
            Assert.Equal(userRequest.CustomURL, jsonResponse!.Content.CustomURL);
        }

        [Fact]
        public async Task GetUserByLoginThrowsNotFoundErrorWhenWrongUserAttemptsGet()
        {
            var userRequest = MockData.GetCreateUserDTODetails();

            await DbHelper.SetupUser(_client, userRequest);

            _client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue(
                "Bearer",
                TestAuthorization.GetToken("User", "User", new Guid())
            );
            var response = await _client.GetAsync($"/api/User/byLogin/{userRequest.Login}");

            Assert.Equal(HttpStatusCode.NotFound, response.StatusCode);
        }

        [Fact]
        public async Task GetUserByLoginThrowsNotFoundErrorWhenWhenWrongLoginIsGiven()
        {
            await DbHelper.SetupUser(_client, MockData.GetCreateUserDTODetails());

            _client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue(
                "Bearer",
                TestAuthorization.GetToken("Admin", "User", new Guid())
            );
            var response = await _client.GetAsync("/api/User/byLogin/WrongLogin");

            Assert.Equal(HttpStatusCode.NotFound, response.StatusCode);
        }

        [Fact]
        public async Task GetUserByLoginThrowsUnauthorizedErrorWhenNoTokenIsGiven()
        {
            _client.DefaultRequestHeaders.Authorization = null;

            var response = await _client.GetAsync("/api/User/byLogin/AnyLogin");
            Assert.Equal(HttpStatusCode.Unauthorized, response.StatusCode);

            response = await _client.PatchAsJsonAsync(
                $"/api/User/{new Guid()}",
                MockData.GetUpdateUserDTODetails()
            );
            Assert.Equal(HttpStatusCode.Unauthorized, response.StatusCode);

            response = await _client.GetAsync($"/api/User/{new Guid()}");
            Assert.Equal(HttpStatusCode.Unauthorized, response.StatusCode);

            response = await _client.PostAsJsonAsync(
                "/api/User",
                MockData.GetUpdateUserDTODetails()
            );
            Assert.Equal(HttpStatusCode.Unauthorized, response.StatusCode);

            response = await _client.DeleteAsync($"/api/User/{new Guid()}");
            Assert.Equal(HttpStatusCode.Unauthorized, response.StatusCode);
        }

        [Fact]
        public async Task UserControllerThrowsTooManyRequests()
        {
            _client.DefaultRequestHeaders.Authorization = null;
            for (int i = 0; i != RateLimiterSettings.permitLimit; i++)
            {
                await _client.GetAsync($"/api/User/{new Guid()}");
            }

            var response = await _client.GetAsync($"/api/User/{new Guid()}");
            Assert.Equal(HttpStatusCode.TooManyRequests, response.StatusCode);

            response = await _client.GetAsync("/api/User/byLogin/AnyLogin");
            Assert.Equal(HttpStatusCode.TooManyRequests, response.StatusCode);

            response = await _client.PatchAsJsonAsync(
                $"/api/User/{new Guid()}",
                MockData.GetUpdateUserDTODetails()
            );
            Assert.Equal(HttpStatusCode.TooManyRequests, response.StatusCode);

            response = await _client.PostAsJsonAsync(
                "/api/User",
                MockData.GetUpdateUserDTODetails()
            );
            Assert.Equal(HttpStatusCode.TooManyRequests, response.StatusCode);

            response = await _client.DeleteAsync($"/api/User/{new Guid()}");
            Assert.Equal(HttpStatusCode.TooManyRequests, response.StatusCode);
        }
    }
}
