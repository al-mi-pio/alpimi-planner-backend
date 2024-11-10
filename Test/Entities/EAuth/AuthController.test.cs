using System.Net;
using System.Net.Http.Headers;
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
    }
}
