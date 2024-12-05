using System.Net;
using System.Net.Http.Headers;
using AlpimiAPI.Entities.ELessonType;
using AlpimiAPI.Responses;
using AlpimiAPI.Settings;
using AlpimiTest.TestSetup;
using AlpimiTest.TestUtilities;
using Xunit;

namespace AlpimiTest.Entities.ELessonType
{
    [Collection("Sequential Tests")]
    public class LessonTypeControllerTest : IAsyncLifetime
    {
        CustomWebApplicationFactory<Program> _factory;
        HttpClient _client;
        Guid userId;
        Guid scheduleId;

        public LessonTypeControllerTest()
        {
            _factory = new CustomWebApplicationFactory<Program>();
            _client = _factory.CreateClient();
        }

        public async Task InitializeAsync()
        {
            await DbHelper.UserCleaner(_client);
            userId = await DbHelper.SetupUser(_client, MockData.GetCreateUserDTODetails());
            scheduleId = await DbHelper.SetupSchedule(
                _client,
                userId,
                MockData.GetCreateScheduleDTODetails()
            );
        }

        public async Task DisposeAsync()
        {
            await DbHelper.UserCleaner(_client);
        }

        [Fact]
        public async Task LessonTypeIsDeleted()
        {
            var lessonTypeRequest = MockData.GetCreateLessonTypeDTODetails(scheduleId);
            var lessonTypeId = await DbHelper.SetupLessonType(_client, lessonTypeRequest);

            _client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue(
                "Bearer",
                TestAuthorization.GetToken("Admin", "User", new Guid())
            );

            var response = await _client.DeleteAsync($"/api/LessonType/{lessonTypeId}");
            Assert.Equal(HttpStatusCode.NoContent, response.StatusCode);

            var query = $"?scheduleId={scheduleId}";
            response = await _client.GetAsync($"/api/LessonType");
            var stringResponse = await response.Content.ReadAsStringAsync();
            Assert.DoesNotContain(lessonTypeRequest.Name, stringResponse);
        }

        [Fact]
        public async Task LessonTypeIsCreated()
        {
            var lessonTypeRequest = MockData.GetCreateLessonTypeDTODetails(scheduleId);

            _client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue(
                "Bearer",
                TestAuthorization.GetToken("Admin", "User", userId)
            );

            var response = await _client.PostAsJsonAsync("/api/LessonType", lessonTypeRequest);
            Assert.Equal(HttpStatusCode.OK, response.StatusCode);

            var query = $"?scheduleId={scheduleId}";
            response = await _client.GetAsync($"/api/LessonType{query}");
            var stringResponse = await response.Content.ReadAsStringAsync();
            Assert.Contains(lessonTypeRequest.Name, stringResponse);
        }

        [Fact]
        public async Task UpdateLessonTypeReturnsUpdatedLessonType()
        {
            var lessonTypeUpdateRequest = MockData.GetUpdateLessonTypeDTODetails();
            var lessonTypeId = await DbHelper.SetupLessonType(
                _client,
                MockData.GetCreateLessonTypeDTODetails(scheduleId)
            );

            _client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue(
                "Bearer",
                TestAuthorization.GetToken("Admin", "Bob", userId)
            );

            var response = await _client.PatchAsJsonAsync(
                $"/api/LessonType/{lessonTypeId}",
                lessonTypeUpdateRequest
            );
            var jsonResponse = await response.Content.ReadFromJsonAsync<
                ApiGetResponse<LessonType>
            >();

            Assert.Equal(lessonTypeUpdateRequest.Name, jsonResponse!.Content.Name);
        }

        [Fact]
        public async Task UpdateLessonTypeThrowsNotFoundErrorWhenWrongIdIsGiven()
        {
            var lessonTypeUpdateRequest = MockData.GetUpdateLessonTypeDTODetails();

            var lessonTypeId = await DbHelper.SetupLessonType(
                _client,
                MockData.GetCreateLessonTypeDTODetails(scheduleId)
            );

            _client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue(
                "Bearer",
                TestAuthorization.GetToken("Admin", "User", userId)
            );
            var response = await _client.PatchAsJsonAsync(
                $"/api/LessonType/{new Guid()}",
                lessonTypeUpdateRequest
            );

            Assert.Equal(HttpStatusCode.NotFound, response.StatusCode);
        }

        [Fact]
        public async Task UpdateLessonTypeThrowsNotFoundErrorWhenWrongUserAttemptsUpdate()
        {
            var lessonTypeUpdateRequest = MockData.GetUpdateLessonTypeDTODetails();

            var lessonTypeId = await DbHelper.SetupLessonType(
                _client,
                MockData.GetCreateLessonTypeDTODetails(scheduleId)
            );

            _client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue(
                "Bearer",
                TestAuthorization.GetToken("User", "User", new Guid())
            );
            var response = await _client.PatchAsJsonAsync(
                $"/api/LessonType/{lessonTypeId}",
                lessonTypeUpdateRequest
            );

            Assert.Equal(HttpStatusCode.NotFound, response.StatusCode);
        }

        [Fact]
        public async Task GetAllLessonTypeReturnsDaysOff()
        {
            var lessonTypeRequest1 = MockData.GetCreateLessonTypeDTODetails(scheduleId);
            var lessonTypeRequest2 = MockData.GetCreateSecondLessonTypeDTODetails(scheduleId);

            await DbHelper.SetupLessonType(_client, lessonTypeRequest1);
            await DbHelper.SetupLessonType(_client, lessonTypeRequest2);

            _client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue(
                "Bearer",
                TestAuthorization.GetToken("Admin", "User", userId)
            );
            var query = $"?scheduleId={scheduleId}";
            var response = await _client.GetAsync($"/api/LessonType{query}");
            var stringResponse = await response.Content.ReadAsStringAsync();

            Assert.Contains(lessonTypeRequest1.Name, stringResponse);
            Assert.Contains(lessonTypeRequest2.Name, stringResponse);
        }

        [Fact]
        public async Task GetAllLessonTypeReturnsEmptyContentWhenWrongUserAttemptsGet()
        {
            var lessonTypeRequest1 = MockData.GetCreateLessonTypeDTODetails(scheduleId);
            var lessonTypeRequest2 = MockData.GetCreateSecondLessonTypeDTODetails(scheduleId);

            await DbHelper.SetupLessonType(_client, lessonTypeRequest1);
            await DbHelper.SetupLessonType(_client, lessonTypeRequest2);

            _client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue(
                "Bearer",
                TestAuthorization.GetToken("User", "User", new Guid())
            );
            var query = $"?scheduleId={scheduleId}";
            var response = await _client.GetAsync($"/api/LessonType{query}");
            var stringResponse = await response.Content.ReadAsStringAsync();

            Assert.DoesNotContain(lessonTypeRequest1.Name, stringResponse);
            Assert.DoesNotContain(lessonTypeRequest2.Name, stringResponse);
        }

        [Fact]
        public async Task GetAllLessonTypeReturnsEmptyContentWhenWrongIdIsGiven()
        {
            var lessonTypeRequest1 = MockData.GetCreateLessonTypeDTODetails(scheduleId);
            var lessonTypeRequest2 = MockData.GetCreateSecondLessonTypeDTODetails(scheduleId);

            await DbHelper.SetupLessonType(_client, lessonTypeRequest1);
            await DbHelper.SetupLessonType(_client, lessonTypeRequest2);

            _client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue(
                "Bearer",
                TestAuthorization.GetToken("Admin", "User", userId)
            );
            var query = $"?scheduleId={new Guid()}";
            var response = await _client.GetAsync($"/api/LessonType{query}");
            var stringResponse = await response.Content.ReadAsStringAsync();

            Assert.DoesNotContain(lessonTypeRequest1.Name, stringResponse);
            Assert.DoesNotContain(lessonTypeRequest2.Name, stringResponse);
        }

        [Fact]
        public async Task GetLessonTypeReturnsLessonType()
        {
            var lessonTypeRequest = MockData.GetCreateLessonTypeDTODetails(scheduleId);

            var lessonTypeId = await DbHelper.SetupLessonType(_client, lessonTypeRequest);

            _client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue(
                "Bearer",
                TestAuthorization.GetToken("Admin", "User", userId)
            );

            var response = await _client.GetAsync($"/api/LessonType/{lessonTypeId}");
            var jsonResponse = await response.Content.ReadFromJsonAsync<
                ApiGetResponse<LessonType>
            >();

            Assert.Equal(lessonTypeRequest.Name, jsonResponse!.Content.Name);
        }

        [Fact]
        public async Task GetScheduleThrowsNotFoundWhenWrongIdIsGiven()
        {
            var lessonTypeRequest = MockData.GetCreateLessonTypeDTODetails(scheduleId);

            await DbHelper.SetupLessonType(_client, lessonTypeRequest);

            _client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue(
                "Bearer",
                TestAuthorization.GetToken("Admin", "User", userId)
            );

            var response = await _client.GetAsync($"/api/LessonType/{new Guid()}");

            Assert.Equal(HttpStatusCode.NotFound, response.StatusCode);
        }

        [Fact]
        public async Task GetScheduleThrowsNotFoundErrorWhenWrongUserTokenIsGiven()
        {
            var lessonTypeRequest = MockData.GetCreateLessonTypeDTODetails(scheduleId);

            var lessonTypeId = await DbHelper.SetupLessonType(_client, lessonTypeRequest);

            _client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue(
                "Bearer",
                TestAuthorization.GetToken("User", "User", new Guid())
            );

            var response = await _client.GetAsync($"/api/LessonType/{lessonTypeId}");

            Assert.Equal(HttpStatusCode.NotFound, response.StatusCode);
        }

        [Fact]
        public async Task LessonTypeControllerThrowsUnauthorized()
        {
            _client.DefaultRequestHeaders.Authorization = null;

            var response = await _client.DeleteAsync($"/api/LessonType/{new Guid()}");
            Assert.Equal(HttpStatusCode.Unauthorized, response.StatusCode);

            response = await _client.PostAsJsonAsync(
                "/api/LessonType",
                MockData.GetCreateLessonTypeDTODetails(scheduleId)
            );
            Assert.Equal(HttpStatusCode.Unauthorized, response.StatusCode);

            var query = $"?scheduleId={new Guid()}";
            response = await _client.GetAsync($"/api/LessonType{query}");
            Assert.Equal(HttpStatusCode.Unauthorized, response.StatusCode);

            response = await _client.PatchAsJsonAsync(
                $"/api/LessonType/{new Guid()}",
                MockData.GetUpdateLessonTypeDTODetails()
            );
            Assert.Equal(HttpStatusCode.Unauthorized, response.StatusCode);

            response = await _client.GetAsync($"/api/LessonType/{new Guid()}");
            Assert.Equal(HttpStatusCode.Unauthorized, response.StatusCode);
        }

        [Fact]
        public async Task LessonTypeControllerThrowsTooManyRequests()
        {
            for (int i = 0; i != RateLimiterSettings.permitLimit; i++)
            {
                await _client.GetAsync("/api/LessonType");
            }

            _client.DefaultRequestHeaders.Authorization = null;

            var response = await _client.DeleteAsync($"/api/LessonType/{new Guid()}");
            Assert.Equal(HttpStatusCode.TooManyRequests, response.StatusCode);

            response = await _client.PostAsJsonAsync(
                "/api/LessonType",
                MockData.GetCreateLessonTypeDTODetails(scheduleId)
            );
            Assert.Equal(HttpStatusCode.TooManyRequests, response.StatusCode);

            var query = $"?scheduleId={new Guid()}";
            response = await _client.GetAsync($"/api/LessonType{query}");
            Assert.Equal(HttpStatusCode.TooManyRequests, response.StatusCode);

            response = await _client.PatchAsJsonAsync(
                $"/api/LessonType/{new Guid()}",
                MockData.GetUpdateLessonTypeDTODetails()
            );
            Assert.Equal(HttpStatusCode.TooManyRequests, response.StatusCode);

            response = await _client.GetAsync($"/api/LessonType/{new Guid()}");
            Assert.Equal(HttpStatusCode.TooManyRequests, response.StatusCode);
        }
    }
}
