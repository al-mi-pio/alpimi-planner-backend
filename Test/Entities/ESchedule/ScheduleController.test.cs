using System.Net;
using System.Net.Http.Headers;
using AlpimiAPI.Entities.ESchedule;
using AlpimiAPI.Responses;
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
            DotNetEnv.Env.Load(
                Path.Combine(Directory.GetCurrentDirectory(), "..", "..", "..", ".env")
            );
            await DbHelper.UserCleaner(_client);
            userId = await DbHelper.SetupUser(_client, MockData.GetCreateUserDTODetails());
        }

        public async Task DisposeAsync()
        {
            await DbHelper.UserCleaner(_client);
        }

        [Fact]
        public async Task DeleteScheduleReturnsNoContentStatusCode()
        {
            _client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue(
                "Bearer",
                TestAuthorization.GetToken("Admin", "User", new Guid())
            );
            var response = await _client.DeleteAsync(
                "/api/Schedule/b70eda99-ed0a-4c06-bc65-44166ce58bb0"
            );
            Assert.Equal(HttpStatusCode.NoContent, response.StatusCode);
        }

        [Fact]
        public async Task DeleteScheduleThrowsUnothorizedErrorWhenNoTokenIsGiven()
        {
            _client.DefaultRequestHeaders.Authorization = null;
            var response = await _client.DeleteAsync(
                "/api/Schedule/b70eda99-ed0a-4c06-bc65-44166ce58bb0"
            );
            Assert.Equal(HttpStatusCode.Unauthorized, response.StatusCode);
        }

        [Fact]
        public async Task CreateScheduleReturnsOkStatusCode()
        {
            var scheduleRequest = MockData.GetCreateScheduleDTODetails();

            _client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue(
                "Bearer",
                TestAuthorization.GetToken("Admin", "User", userId)
            );
            var response = await _client.PostAsJsonAsync("/api/Schedule", scheduleRequest);

            Assert.Equal(HttpStatusCode.OK, response.StatusCode);
        }

        [Fact]
        public async Task CreateScheduleThrowsUnothorizedErrorWhenNoTokenIsGiven()
        {
            var scheduleRequest = MockData.GetCreateScheduleDTODetails();

            _client.DefaultRequestHeaders.Authorization = null;
            var response = await _client.PostAsJsonAsync("/api/Schedule", scheduleRequest);

            Assert.Equal(HttpStatusCode.Unauthorized, response.StatusCode);
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
            Assert.Equal(scheduleUpdateRequest.SchoolHour, jsonResponse!.Content.SchoolHour);
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
        public async Task UpdateScheduleThrowsUnothorizedErrorWhenNoTokenIsGiven()
        {
            var scheduleUpdateRequest = MockData.GetUpdateScheduleDTODetails();

            var scheduleId = await DbHelper.SetupSchedule(
                _client,
                userId,
                MockData.GetCreateScheduleDTODetails()
            );

            _client.DefaultRequestHeaders.Authorization = null;
            var response = await _client.PatchAsJsonAsync(
                $"/api/Schedule/{scheduleId}",
                scheduleUpdateRequest
            );

            Assert.Equal(HttpStatusCode.Unauthorized, response.StatusCode);
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
            Assert.Equal(scheduleRequest.SchoolHour, jsonResponse!.Content.SchoolHour);
        }

        [Fact]
        public async Task GetScheduleByNameThrowsNotFoundErrorWhenWrongUserAttemptsGet()
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
            var response = await _client.GetAsync($"/api/Schedule/byName/WrongName");

            Assert.Equal(HttpStatusCode.NotFound, response.StatusCode);
        }

        [Fact]
        public async Task GetScheduleByNameThrowsUnothorizedErrorWhenNoTokenIsGiven()
        {
            var scheduleRequest = MockData.GetCreateScheduleDTODetails();

            await DbHelper.SetupSchedule(_client, userId, scheduleRequest);

            _client.DefaultRequestHeaders.Authorization = null;
            var response = await _client.GetAsync($"/api/Schedule/byName/{scheduleRequest.Name}");

            Assert.Equal(HttpStatusCode.Unauthorized, response.StatusCode);
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
            Assert.Equal(scheduleRequest.SchoolHour, jsonResponse!.Content.SchoolHour);
        }

        [Fact]
        public async Task GetScheduleThrowsUnothorizedErrorWhenNoTokenIsGiven()
        {
            var scheduleId = await DbHelper.SetupSchedule(
                _client,
                userId,
                MockData.GetCreateScheduleDTODetails()
            );

            _client.DefaultRequestHeaders.Authorization = null;
            var response = await _client.GetAsync($"/api/Schedule/{scheduleId}");

            Assert.Equal(HttpStatusCode.Unauthorized, response.StatusCode);
        }

        [Fact]
        public async Task GetScheduleThrowsNotFoundErrorWhenWrongUserAttemptsGet()
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
        public async Task GetAllScheduleTReturnsSchedules()
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
        public async Task GetAllScheduleTReturnsEmptyContentWhenWrongUserAttemptsGet()
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
        public async Task ThrowsUnothorizedErrorWhenNoTokenIsGiven()
        {
            var userId2 = await DbHelper.SetupUser(
                _client,
                MockData.GetCreateSecondUserDTODetails()
            );

            await DbHelper.SetupSchedule(_client, userId, MockData.GetCreateScheduleDTODetails());
            await DbHelper.SetupSchedule(
                _client,
                userId2,
                MockData.GetCreateSecondScheduleDTODetails()
            );

            _client.DefaultRequestHeaders.Authorization = null;
            var response = await _client.GetAsync("/api/Schedule");

            Assert.Equal(HttpStatusCode.Unauthorized, response.StatusCode);
        }
    }
}
