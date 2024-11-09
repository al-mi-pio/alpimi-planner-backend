using System.Net;
using System.Net.Http.Headers;
using AlpimiAPI.Entities.EUser;
using AlpimiAPI.Entities.EUser.DTO;
using AlpimiAPI.Responses;
using AlpimiTest.TestUtilities;
using Sprache;
using Xunit;

namespace AlpimiTest.Entities.EUser
{
    [Collection("Sequential Tests")]
    public class UserControllerTest
    {
        CustomWebApplicationFactory<Program> _factory;
        HttpClient _client;

        public UserControllerTest()
        {
            _factory = new CustomWebApplicationFactory<Program>();
            _client = _factory.CreateClient();
        }

        [Fact]
        public async Task DeleteUserReturnsNoContentStatusCode()
        {
            var user = MockData.GetUserDetails();
            _client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue(
                "Bearer",
                TestAuthorization.GetToken("Admin", user.Login, user.Id)
            );
            var response = await _client.DeleteAsync(
                "/api/User/b70eda99-ed0a-4c06-bc65-44166ce58bb0"
            );
            Assert.Equal(HttpStatusCode.NoContent, response.StatusCode);
        }

        [Fact]
        public async Task DeleteUserThrowsForbiddenErrorWhenNoTokenIsGiven()
        {
            var user = MockData.GetUserDetails();
            _client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue(
                "Bearer",
                TestAuthorization.GetToken("User", user.Login, user.Id)
            );
            var response = await _client.DeleteAsync(
                "/api/User/b70eda99-ed0a-4c06-bc65-44166ce58bb0"
            );

            Assert.Equal(HttpStatusCode.Forbidden, response.StatusCode);
        }

        [Fact]
        public async Task CreateUSerReturnsOkStatusCode()
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
                TestAuthorization.GetToken("Admin", user.Login, user.Id)
            );

            var response = await _client.PostAsJsonAsync("/api/User", userRequest);
            var jsonId = await response.Content.ReadFromJsonAsync<ApiGetResponse<Guid>>();

            await _client.DeleteAsync($"/api/User/{jsonId!.Content}");

            Assert.Equal(HttpStatusCode.OK, response.StatusCode);
        }

        [Fact]
        public async Task CreateUserThrowsUnothorizedErrorWhenNoTokenIsGiven()
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
            var user = MockData.GetUserDetails();
            var userRequest = new CreateUserDTO
            {
                Login = user.Login,
                CustomURL = user.CustomURL!,
                Password = "sssSSS1!"
            };
            var userUpdateRequest = new UpdateUserDTO { Login = "New_Login" };

            _client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue(
                "Bearer",
                TestAuthorization.GetToken("Admin", user.Login, user.Id)
            );

            var userId = await _client.PostAsJsonAsync("/api/User", userRequest);
            var jsonId = await userId.Content.ReadFromJsonAsync<ApiGetResponse<Guid>>();

            var response = await _client.PatchAsJsonAsync(
                $"/api/User/{jsonId!.Content}",
                userUpdateRequest
            );
            var jsonResponse = await response.Content.ReadFromJsonAsync<ApiGetResponse<User>>();

            await _client.DeleteAsync($"/api/User/{jsonId.Content}");

            Assert.Equal(userUpdateRequest.Login, jsonResponse!.Content.Login);
            Assert.Equal(userRequest.CustomURL, jsonResponse!.Content.CustomURL);
        }

        [Fact]
        public async Task UpdateUserThrowsNotFoundErrorWhenWrongIdIsGiven()
        {
            var user = MockData.GetUserDetails();
            var userRequest = new CreateUserDTO
            {
                Login = user.Login,
                CustomURL = user.CustomURL!,
                Password = "sssSSS1!"
            };
            var userUpdateRequest = new UpdateUserDTO { Login = "New_Login" };

            _client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue(
                "Bearer",
                TestAuthorization.GetToken("Admin", user.Login, user.Id)
            );

            var userId = await _client.PostAsJsonAsync("/api/User", userRequest);
            var jsonId = await userId.Content.ReadFromJsonAsync<ApiGetResponse<Guid>>();

            var response = await _client.PatchAsJsonAsync(
                $"/api/User/{new Guid()}",
                userUpdateRequest
            );

            await _client.DeleteAsync($"/api/User/{jsonId!.Content}");

            Assert.Equal(HttpStatusCode.NotFound, response.StatusCode);
        }

        [Fact]
        public async Task UpdateUserThrowsNotFoundErrorWhenWrongUserAttemptsUpdate()
        {
            var user = MockData.GetUserDetails();
            var userRequest = new CreateUserDTO
            {
                Login = user.Login,
                CustomURL = user.CustomURL!,
                Password = "sssSSS1!"
            };
            var userUpdateRequest = new UpdateUserDTO { Login = "New_Login" };

            _client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue(
                "Bearer",
                TestAuthorization.GetToken("Admin", user.Login, user.Id)
            );

            var userId = await _client.PostAsJsonAsync("/api/User", userRequest);
            var jsonId = await userId.Content.ReadFromJsonAsync<ApiGetResponse<Guid>>();

            _client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue(
                "Bearer",
                TestAuthorization.GetToken("User", user.Login, user.Id)
            );
            var response = await _client.PatchAsJsonAsync(
                $"/api/User/{jsonId!.Content}",
                userUpdateRequest
            );
            _client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue(
                "Bearer",
                TestAuthorization.GetToken("Admin", user.Login, user.Id)
            );
            await _client.DeleteAsync($"/api/User/{jsonId.Content}");

            Assert.Equal(HttpStatusCode.NotFound, response.StatusCode);
        }

        [Fact]
        public async Task UpdateUserThrowsUnothorizedErrorWhenNoTokenIsGiven()
        {
            var user = MockData.GetUserDetails();
            var userRequest = new CreateUserDTO
            {
                Login = user.Login,
                CustomURL = user.CustomURL!,
                Password = "sssSSS1!"
            };
            var userUpdateRequest = new UpdateUserDTO { Login = "New_Login" };

            _client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue(
                "Bearer",
                TestAuthorization.GetToken("Admin", user.Login, user.Id)
            );

            var userId = await _client.PostAsJsonAsync("/api/User", userRequest);
            var jsonId = await userId.Content.ReadFromJsonAsync<ApiGetResponse<Guid>>();

            _client.DefaultRequestHeaders.Authorization = null;
            var response = await _client.PatchAsJsonAsync(
                $"/api/User/{jsonId!.Content}",
                userUpdateRequest
            );
            _client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue(
                "Bearer",
                TestAuthorization.GetToken("Admin", user.Login, user.Id)
            );
            await _client.DeleteAsync($"/api/User/{jsonId.Content}");

            Assert.Equal(HttpStatusCode.Unauthorized, response.StatusCode);
        }

        [Fact]
        public async Task GetUserReturnsUser()
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
                TestAuthorization.GetToken("Admin", user.Login, user.Id)
            );

            var userId = await _client.PostAsJsonAsync("/api/User", userRequest);
            var jsonId = await userId.Content.ReadFromJsonAsync<ApiGetResponse<Guid>>();

            var response = await _client.GetAsync($"/api/User/{jsonId!.Content}");
            var jsonResponse = await response.Content.ReadFromJsonAsync<ApiGetResponse<User>>();

            await _client.DeleteAsync($"/api/User/{jsonId.Content}");

            Assert.Equal(userRequest.Login, jsonResponse!.Content.Login);
            Assert.Equal(userRequest.CustomURL, jsonResponse!.Content.CustomURL);
        }

        [Fact]
        public async Task GetUserThrowsNotFoundErrorWhenWrongUserAttemptsGet()
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
                TestAuthorization.GetToken("Admin", user.Login, user.Id)
            );

            var userId = await _client.PostAsJsonAsync("/api/User", userRequest);
            var jsonId = await userId.Content.ReadFromJsonAsync<ApiGetResponse<Guid>>();

            _client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue(
                "Bearer",
                TestAuthorization.GetToken("User", user.Login, new Guid())
            );
            var response = await _client.GetAsync($"/api/User/{jsonId!.Content}");

            _client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue(
                "Bearer",
                TestAuthorization.GetToken("Admin", user.Login, user.Id)
            );
            await _client.DeleteAsync($"/api/User/{jsonId.Content}");

            Assert.Equal(HttpStatusCode.NotFound, response.StatusCode);
        }

        [Fact]
        public async Task GetUserThrowsNotFoundErrorWhenWhenWrongIdIsGiven()
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
                TestAuthorization.GetToken("Admin", user.Login, user.Id)
            );

            var userId = await _client.PostAsJsonAsync("/api/User", userRequest);
            var jsonId = await userId.Content.ReadFromJsonAsync<ApiGetResponse<Guid>>();

            var response = await _client.GetAsync($"/api/User/{new Guid()}");

            await _client.DeleteAsync($"/api/User/{jsonId!.Content}");

            Assert.Equal(HttpStatusCode.NotFound, response.StatusCode);
        }

        [Fact]
        public async Task GetUserThrowsThrowsUnothorizedErrorWhenNoTokenIsGiven()
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
                TestAuthorization.GetToken("Admin", user.Login, user.Id)
            );

            var userId = await _client.PostAsJsonAsync("/api/User", userRequest);
            var jsonId = await userId.Content.ReadFromJsonAsync<ApiGetResponse<Guid>>();

            _client.DefaultRequestHeaders.Authorization = null;
            var response = await _client.GetAsync($"/api/User/{new Guid()}");

            _client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue(
                "Bearer",
                TestAuthorization.GetToken("Admin", user.Login, user.Id)
            );
            await _client.DeleteAsync($"/api/User/{jsonId!.Content}");

            Assert.Equal(HttpStatusCode.Unauthorized, response.StatusCode);
        }

        [Fact]
        public async Task GetUserByLoginReturnsUser()
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
                TestAuthorization.GetToken("Admin", user.Login, user.Id)
            );

            var userId = await _client.PostAsJsonAsync("/api/User", userRequest);
            var jsonId = await userId.Content.ReadFromJsonAsync<ApiGetResponse<Guid>>();

            var response = await _client.GetAsync($"/api/User/byLogin/{userRequest.Login}");
            var jsonResponse = await response.Content.ReadFromJsonAsync<ApiGetResponse<User>>();

            await _client.DeleteAsync($"/api/User/{jsonId!.Content}");

            Assert.Equal(userRequest.Login, jsonResponse!.Content.Login);
            Assert.Equal(userRequest.CustomURL, jsonResponse!.Content.CustomURL);
        }

        [Fact]
        public async Task GetUserByLoginThrowsNotFoundErrorWhenWrongUserAttemptsGet()
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
                TestAuthorization.GetToken("Admin", user.Login, user.Id)
            );

            var userId = await _client.PostAsJsonAsync("/api/User", userRequest);
            var jsonId = await userId.Content.ReadFromJsonAsync<ApiGetResponse<Guid>>();

            _client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue(
                "Bearer",
                TestAuthorization.GetToken("User", user.Login, new Guid())
            );
            var response = await _client.GetAsync($"/api/User/byLogin/{userRequest.Login}");

            _client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue(
                "Bearer",
                TestAuthorization.GetToken("Admin", user.Login, user.Id)
            );
            await _client.DeleteAsync($"/api/User/{jsonId!.Content}");

            Assert.Equal(HttpStatusCode.NotFound, response.StatusCode);
        }

        [Fact]
        public async Task GetUserByLoginThrowsNotFoundErrorWhenWhenWrongLoginIsGiven()
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
                TestAuthorization.GetToken("Admin", user.Login, user.Id)
            );

            var userId = await _client.PostAsJsonAsync("/api/User", userRequest);
            var jsonId = await userId.Content.ReadFromJsonAsync<ApiGetResponse<Guid>>();

            var response = await _client.GetAsync("/api/User/byLogin/WrongLogin");

            await _client.DeleteAsync($"/api/User/{jsonId!.Content}");

            Assert.Equal(HttpStatusCode.NotFound, response.StatusCode);
        }

        [Fact]
        public async Task GetUserByLoginThrowsThrowsUnothorizedErrorWhenNoTokenIsGiven()
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
                TestAuthorization.GetToken("Admin", user.Login, user.Id)
            );

            var userId = await _client.PostAsJsonAsync("/api/User", userRequest);
            var jsonId = await userId.Content.ReadFromJsonAsync<ApiGetResponse<Guid>>();

            _client.DefaultRequestHeaders.Authorization = null;
            var response = await _client.GetAsync($"/api/User/byLogin{userRequest.Login}");

            _client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue(
                "Bearer",
                TestAuthorization.GetToken("Admin", user.Login, user.Id)
            );
            await _client.DeleteAsync($"/api/User/{jsonId!.Content}");

            Assert.Equal(HttpStatusCode.Unauthorized, response.StatusCode);
        }
    }
}
