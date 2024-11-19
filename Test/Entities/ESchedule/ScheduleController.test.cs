using System.Net;
using System.Net.Http.Headers;
using AlpimiAPI.Entities.ESchedule;
using AlpimiAPI.Entities.ESchedule.DTO;
using AlpimiAPI.Responses;
using AlpimiAPI.Settings;
using AlpimiTest.TestUtilities;
using Xunit;

namespace AlpimiTest.Entities.ESchedule
{
    [Collection("Sequential Tests")]
    public class ScheduleControllerTest : IAsyncLifetime
    {
        CustomWebApplicationFactory<Program> _factory;
        HttpClient _client;
        Guid userId;

        public ScheduleControllerTest()
        {
            _factory = new CustomWebApplicationFactory<Program>();
            _client = _factory.CreateClient();
        }

        public async Task InitializeAsync()
        {
            await DbHelper.UserCleaner(_client);
            userId = await DbHelper.SetupUser(_client, MockData.GetCreateUserDTODetails());
        }

        public async Task DisposeAsync()
        {
            await DbHelper.UserCleaner(_client);
        }

        [Fact]
        public async Task ScheduleIsDeleted()
        {
            var scheduleId = await DbHelper.SetupSchedule(
                _client,
                userId,
                MockData.GetCreateScheduleDTODetails()
            );

            _client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue(
                "Bearer",
                TestAuthorization.GetToken("Admin", "User", new Guid())
            );

            var response = await _client.DeleteAsync($"/api/Schedule/{scheduleId}");
            Assert.Equal(HttpStatusCode.NoContent, response.StatusCode);

            response = await _client.GetAsync($"/api/Schedule/{scheduleId}");
            Assert.Equal(HttpStatusCode.NotFound, response.StatusCode);
        }

        [Fact]
        public async Task ScheduleIsCreated()
        {
            var scheduleRequest = MockData.GetCreateScheduleDTODetails();

            _client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue(
                "Bearer",
                TestAuthorization.GetToken("Admin", "User", userId)
            );

            var response = await _client.PostAsJsonAsync("/api/Schedule", scheduleRequest);
            var jsonScheduleId = await response.Content.ReadFromJsonAsync<ApiGetResponse<Guid>>();
            Assert.Equal(HttpStatusCode.OK, response.StatusCode);

            response = await _client.GetAsync($"/api/Schedule/{jsonScheduleId!.Content}");
            Assert.Equal(HttpStatusCode.OK, response.StatusCode);
        }

        [Fact]
        public async Task UpdateScheduleReturnsUpdatedSchedule()
        {
            var scheduleUpdateRequest = MockData.GetUpdateScheduleDTODetails();

            var scheduleId = await DbHelper.SetupSchedule(
                _client,
                userId,
                MockData.GetCreateScheduleDTODetails()
            );

            _client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue(
                "Bearer",
                TestAuthorization.GetToken("Admin", "User", userId)
            );
            var response = await _client.PatchAsJsonAsync(
                $"/api/Schedule/{scheduleId}",
                scheduleUpdateRequest
            );

            var jsonResponse = await response.Content.ReadFromJsonAsync<ApiGetResponse<Schedule>>();

            Assert.Equal(scheduleUpdateRequest.Name, jsonResponse!.Content.Name);
        }

        [Fact]
        public async Task UpdateScheduleThrowsNotFoundErrorWhenWrongIdIsGiven()
        {
            var scheduleUpdateRequest = MockData.GetUpdateScheduleDTODetails();

            var scheduleId = await DbHelper.SetupSchedule(
                _client,
                userId,
                MockData.GetCreateScheduleDTODetails()
            );

            _client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue(
                "Bearer",
                TestAuthorization.GetToken("Admin", "User", userId)
            );
            var response = await _client.PatchAsJsonAsync(
                $"/api/Schedule/{new Guid()}",
                scheduleUpdateRequest
            );

            Assert.Equal(HttpStatusCode.NotFound, response.StatusCode);
        }

        [Fact]
        public async Task UpdateScheduleThrowsNotFoundErrorWhenWrongUserAttemptsUpdate()
        {
            var scheduleUpdateRequest = MockData.GetUpdateScheduleDTODetails();

            var scheduleId = await DbHelper.SetupSchedule(
                _client,
                userId,
                MockData.GetCreateScheduleDTODetails()
            );

            _client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue(
                "Bearer",
                TestAuthorization.GetToken("User", "User", new Guid())
            );
            var response = await _client.PatchAsJsonAsync(
                $"/api/Schedule/{scheduleId}",
                scheduleUpdateRequest
            );

            Assert.Equal(HttpStatusCode.NotFound, response.StatusCode);
        }

        [Fact]
        public async Task GetScheduleByNameReturnsSchedule()
        {
            var scheduleRequest = MockData.GetCreateScheduleDTODetails();

            await DbHelper.SetupSchedule(_client, userId, scheduleRequest);

            _client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue(
                "Bearer",
                TestAuthorization.GetToken("Admin", "User", userId)
            );
            var response = await _client.GetAsync($"/api/Schedule/byName/{scheduleRequest.Name}");
            var jsonResponse = await response.Content.ReadFromJsonAsync<ApiGetResponse<Schedule>>();

            Assert.Equal(scheduleRequest.Name, jsonResponse!.Content.Name);
        }

        [Fact]
        public async Task GetScheduleByNameThrowsNotFoundErrorWhenWrongUserTokenIsGiven()
        {
            var scheduleRequest = MockData.GetCreateScheduleDTODetails();

            await DbHelper.SetupSchedule(_client, userId, scheduleRequest);

            _client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue(
                "Bearer",
                TestAuthorization.GetToken("User", "User", new Guid())
            );
            var response = await _client.GetAsync($"/api/Schedule/byName/{scheduleRequest.Name}");

            Assert.Equal(HttpStatusCode.NotFound, response.StatusCode);
        }

        [Fact]
        public async Task GetScheduleByNameThrowsNotFoundErrorWhenWrongNameIsGiven()
        {
            var scheduleRequest = MockData.GetCreateScheduleDTODetails();

            await DbHelper.SetupSchedule(_client, userId, scheduleRequest);

            _client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue(
                "Bearer",
                TestAuthorization.GetToken("Admin", "User", userId)
            );
            var response = await _client.GetAsync("/api/Schedule/byName/WrongName");

            Assert.Equal(HttpStatusCode.NotFound, response.StatusCode);
        }

        [Fact]
        public async Task GetScheduleReturnsSchedule()
        {
            var scheduleRequest = MockData.GetCreateScheduleDTODetails();

            var scheduleId = await DbHelper.SetupSchedule(_client, userId, scheduleRequest);

            _client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue(
                "Bearer",
                TestAuthorization.GetToken("Admin", "User", userId)
            );
            var response = await _client.GetAsync($"/api/Schedule/{scheduleId}");
            var jsonResponse = await response.Content.ReadFromJsonAsync<ApiGetResponse<Schedule>>();

            Assert.Equal(scheduleRequest.Name, jsonResponse!.Content.Name);
        }

        [Fact]
        public async Task GetScheduleThrowsNotFoundErrorWhenWrongUserTokenIsGivent()
        {
            var scheduleId = await DbHelper.SetupSchedule(
                _client,
                userId,
                MockData.GetCreateScheduleDTODetails()
            );

            _client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue(
                "Bearer",
                TestAuthorization.GetToken("User", "User", new Guid())
            );
            var response = await _client.GetAsync($"/api/Schedule/{scheduleId}");

            Assert.Equal(HttpStatusCode.NotFound, response.StatusCode);
        }

        [Fact]
        public async Task GetScheduleThrowsNotFoundErrorWhenWrongIdIsGiven()
        {
            await DbHelper.SetupSchedule(_client, userId, MockData.GetCreateScheduleDTODetails());

            _client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue(
                "Bearer",
                TestAuthorization.GetToken("Admin", "User", userId)
            );
            var response = await _client.GetAsync($"/api/Schedule/{new Guid()}");

            Assert.Equal(HttpStatusCode.NotFound, response.StatusCode);
        }

        [Fact]
        public async Task GetAllScheduleReturnsSchedules()
        {
            var scheduleRequest1 = MockData.GetCreateScheduleDTODetails();
            var scheduleRequest2 = MockData.GetCreateSecondScheduleDTODetails();

            var userId2 = await DbHelper.SetupUser(
                _client,
                MockData.GetCreateSecondUserDTODetails()
            );

            await DbHelper.SetupSchedule(_client, userId, scheduleRequest1);
            await DbHelper.SetupSchedule(_client, userId2, scheduleRequest2);

            _client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue(
                "Bearer",
                TestAuthorization.GetToken("Admin", "User", userId)
            );
            var response = await _client.GetAsync("/api/Schedule");
            var stringResponse = await response.Content.ReadAsStringAsync();

            Assert.Contains(scheduleRequest1.Name, stringResponse);
            Assert.Contains(scheduleRequest2.Name, stringResponse);
        }

        [Fact]
        public async Task GetAllScheduleReturnsEmptyContentWhenWrongUserAttemptsGet()
        {
            var scheduleRequest1 = MockData.GetCreateScheduleDTODetails();
            var scheduleRequest2 = MockData.GetCreateSecondScheduleDTODetails();

            var userId2 = await DbHelper.SetupUser(
                _client,
                MockData.GetCreateSecondUserDTODetails()
            );

            await DbHelper.SetupSchedule(_client, userId, scheduleRequest1);
            await DbHelper.SetupSchedule(_client, userId2, scheduleRequest2);

            _client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue(
                "Bearer",
                TestAuthorization.GetToken("User", "User", new Guid())
            );
            var response = await _client.GetAsync("/api/Schedule");
            var stringResponse = await response.Content.ReadAsStringAsync();

            Assert.DoesNotContain(scheduleRequest1.Name, stringResponse);
            Assert.DoesNotContain(scheduleRequest2.Name, stringResponse);
        }

        [Fact]
        public async Task GetAllScheduleTReturnsOnlyUserMadeSchedules()
        {
            var scheduleRequest1 = MockData.GetCreateScheduleDTODetails();
            var scheduleRequest2 = MockData.GetCreateSecondScheduleDTODetails();

            var userId2 = await DbHelper.SetupUser(
                _client,
                MockData.GetCreateSecondUserDTODetails()
            );

            await DbHelper.SetupSchedule(_client, userId, scheduleRequest1);
            await DbHelper.SetupSchedule(_client, userId2, scheduleRequest2);

            _client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue(
                "Bearer",
                TestAuthorization.GetToken("User", "User", userId)
            );
            var response = await _client.GetAsync("/api/Schedule");
            var stringResponse = await response.Content.ReadAsStringAsync();

            Assert.Contains(scheduleRequest1.Name, stringResponse);
            Assert.DoesNotContain(scheduleRequest2.Name, stringResponse);
        }

        [Fact]
        public async Task ScheduleControllerThrowsUnauthorized()
        {
            _client.DefaultRequestHeaders.Authorization = null;
            var response = await _client.GetAsync("/api/Schedule");
            Assert.Equal(HttpStatusCode.Unauthorized, response.StatusCode);

            response = await _client.GetAsync($"/api/Schedule/{new Guid()}");
            Assert.Equal(HttpStatusCode.Unauthorized, response.StatusCode);

            response = await _client.GetAsync("/api/Schedule/byName/AnyName");
            Assert.Equal(HttpStatusCode.Unauthorized, response.StatusCode);

            response = await _client.PatchAsJsonAsync(
                $"/api/Schedule/{new Guid()}",
                MockData.GetUpdateScheduleDTODetails()
            );
            Assert.Equal(HttpStatusCode.Unauthorized, response.StatusCode);

            response = await _client.PostAsJsonAsync(
                "/api/Schedule",
                MockData.GetCreateScheduleDTODetails()
            );
            Assert.Equal(HttpStatusCode.Unauthorized, response.StatusCode);

            response = await _client.DeleteAsync(
                "/api/Schedule/b70eda99-ed0a-4c06-bc65-44166ce58bb0"
            );
            Assert.Equal(HttpStatusCode.Unauthorized, response.StatusCode);
        }

        [Fact]
        public async Task ScheduleControllerThrowsTooManyRequests()
        {
            for (int i = 0; i != RateLimiterSettings.permitLimit; i++)
            {
                await _client.GetAsync("/api/Schedule");
            }

            var response = await _client.GetAsync("/api/Schedule");
            Assert.Equal(HttpStatusCode.TooManyRequests, response.StatusCode);

            response = await _client.GetAsync("/api/Schedule");
            Assert.Equal(HttpStatusCode.TooManyRequests, response.StatusCode);

            response = await _client.GetAsync($"/api/Schedule/{new Guid()}");
            Assert.Equal(HttpStatusCode.TooManyRequests, response.StatusCode);

            response = await _client.GetAsync("/api/Schedule/byName/AnyName");
            Assert.Equal(HttpStatusCode.TooManyRequests, response.StatusCode);

            response = await _client.PatchAsJsonAsync(
                $"/api/Schedule/{new Guid()}",
                MockData.GetUpdateScheduleDTODetails()
            );
            Assert.Equal(HttpStatusCode.TooManyRequests, response.StatusCode);

            response = await _client.PostAsJsonAsync(
                "/api/Schedule",
                MockData.GetCreateScheduleDTODetails()
            );
            Assert.Equal(HttpStatusCode.TooManyRequests, response.StatusCode);

            response = await _client.DeleteAsync(
                "/api/Schedule/b70eda99-ed0a-4c06-bc65-44166ce58bb0"
            );
            Assert.Equal(HttpStatusCode.TooManyRequests, response.StatusCode);
        }
    }
}
