using System.Net;
using System.Net.Http.Headers;
using AlpimiAPI.Entities.EAuth.Queries;
using AlpimiAPI.Entities.ESchedule;
using AlpimiAPI.Entities.EUser;
using AlpimiAPI.Responses;
using AlpimiTest.TestUtilities;
using Xunit;

namespace AlpimiTest.Entities.EAuth
{
    [Collection("Sequential Tests")]
    public class AuthControllerTest : IAsyncLifetime
    {
        CustomWebApplicationFactory<Program> _factory;
        HttpClient _client;

        public AuthControllerTest()
        {
            _factory = new CustomWebApplicationFactory<Program>();
            _client = _factory.CreateClient();
        }

        public async Task InitializeAsync()
        {
            await DbHelper.UserCleaner(_client);
            await DbHelper.SetupUser(_client, MockData.GetCreateUserDTODetails());
        }

        public async Task DisposeAsync()
        {
            await DbHelper.UserCleaner(_client);
        }

        [Fact]
        public async Task RefreshTokenThrowsUnothorizedErrorWhenNoJWTTokenIsGiven()
        {
            _client.DefaultRequestHeaders.Authorization = null;
            var response = await _client.GetAsync("/api/Auth/refresh");
            Assert.Equal(HttpStatusCode.Unauthorized, response.StatusCode);
        }

        [Fact]
        public async Task RefreshTokenReturnsOKStatusCodeWhenCorrectJWTTokenIsGiven()
        {
            _client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue(
                "Bearer",
                TestAuthorization.GetToken("Admin", "Random", new Guid())
            );
            var response = await _client.GetAsync("/api/Auth/refresh");

            Assert.Equal(HttpStatusCode.OK, response.StatusCode);
        }

        [Fact]
        public async Task LoginReturnOKStatusCode()
        {
            var loginRequest = MockData.GetLoginDTODetails();

            var response = await _client.PostAsJsonAsync("/api/Auth/login", loginRequest);

            Assert.Equal(HttpStatusCode.OK, response.StatusCode);
        }
    }
}
