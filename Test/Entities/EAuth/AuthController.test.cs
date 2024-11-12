using System.Net;
using System.Net.Http.Headers;
using AlpimiAPI.Entities.EAuth.Queries;
using AlpimiAPI.Entities.ESchedule;
using AlpimiAPI.Responses;
using AlpimiTest.TestUtilities;
using Xunit;

namespace AlpimiTest.Entities.EAuth
{
    [Collection("Sequential Tests")]
    public class AuthControllerTest
    {
        CustomWebApplicationFactory<Program> _factory;
        HttpClient _client;

        public AuthControllerTest()
        {
            _factory = new CustomWebApplicationFactory<Program>();
            _client = _factory.CreateClient();
        }

        [Fact]
        public async Task RefreshTokenThrowsUnothorizedErrorWhenNoJWTTokenIsGiven()
        {
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
            var userId = await DbHelper.SetupUser(_client, MockData.GetCreateUserDTODetails());
            var loginRequest = MockData.GetLoginDTODetails();

            var response = await _client.PostAsJsonAsync("/api/Auth/login", loginRequest);

            await DbHelper.UserCleaner(_client);

            Assert.Equal(HttpStatusCode.OK, response.StatusCode);
        }
    }
}
