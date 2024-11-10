using System.Net;
using System.Net.Http.Headers;
using AlpimiAPI.Entities.ESchedule;
using AlpimiAPI.Responses;
using AlpimiTest.TestUtilities;
using Xunit;

namespace AlpimiTest.Entities.ESchedule
{
    [Collection("Sequential Tests")]
    public class ScheduleControllerTest
    {
        CustomWebApplicationFactory<Program> _factory;
        HttpClient _client;

        public ScheduleControllerTest()
        {
            _factory = new CustomWebApplicationFactory<Program>();
            _client = _factory.CreateClient();
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
            var response = await _client.DeleteAsync(
                "/api/Schedule/b70eda99-ed0a-4c06-bc65-44166ce58bb0"
            );
            Assert.Equal(HttpStatusCode.Unauthorized, response.StatusCode);
        }

        [Fact]
        public async Task CreateScheduleReturnsOkStatusCode()
        {
            var scheduleRequest = MockData.GetCreateScheduleDTODetails();

            var userId = await DbHelper.SetupUser(_client);

            _client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue(
                "Bearer",
                TestAuthorization.GetToken("Admin", "User", userId)
            );
            var response = await _client.PostAsJsonAsync("/api/Schedule", scheduleRequest);

            await DbHelper.UserCleaner(_client, userId);

            Assert.Equal(HttpStatusCode.OK, response.StatusCode);
        }

        [Fact]
        public async Task CreateScheduleThrowsUnothorizedErrorWhenNoTokenIsGiven()
        {
            var scheduleRequest = MockData.GetCreateScheduleDTODetails();

            var userId = await DbHelper.SetupUser(_client);

            _client.DefaultRequestHeaders.Authorization = null;
            var response = await _client.PostAsJsonAsync("/api/Schedule", scheduleRequest);

            await DbHelper.UserCleaner(_client, userId);

            Assert.Equal(HttpStatusCode.Unauthorized, response.StatusCode);
        }

        [Fact]
        public async Task UpdateScheduleReturnsUpdatedSchedule()
        {
            var scheduleUpdateRequest = MockData.GetUpdateScheduleDTODetails();

            var userId = await DbHelper.SetupUser(_client);
            var scheduleId = await DbHelper.SetupSchedule(_client, userId);

            _client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue(
                "Bearer",
                TestAuthorization.GetToken("Admin", "User", userId)
            );
            var response = await _client.PatchAsJsonAsync(
                $"/api/Schedule/{scheduleId}",
                scheduleUpdateRequest
            );

            var jsonResponse = await response.Content.ReadFromJsonAsync<ApiGetResponse<Schedule>>();

            await DbHelper.UserCleaner(_client, userId);

            Assert.Equal(scheduleUpdateRequest.Name, jsonResponse!.Content.Name);
            Assert.Equal(scheduleUpdateRequest.SchoolHour, jsonResponse!.Content.SchoolHour);
        }

        [Fact]
        public async Task UpdateScheduleThrowsNotFoundErrorWhenWrongIdIsGiven()
        {
            var scheduleUpdateRequest = MockData.GetUpdateScheduleDTODetails();

            var userId = await DbHelper.SetupUser(_client);
            var scheduleId = await DbHelper.SetupSchedule(_client, userId);

            _client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue(
                "Bearer",
                TestAuthorization.GetToken("Admin", "User", userId)
            );
            var response = await _client.PatchAsJsonAsync(
                $"/api/Schedule/{new Guid()}",
                scheduleUpdateRequest
            );

            var jsonResponse = await response.Content.ReadFromJsonAsync<ApiGetResponse<Schedule>>();

            await DbHelper.UserCleaner(_client, userId);

            Assert.Equal(HttpStatusCode.NotFound, response.StatusCode);
        }

        [Fact]
        public async Task UpdateScheduleThrowsNotFoundErrorWhenWrongUserAttemptsUpdate()
        {
            var scheduleUpdateRequest = MockData.GetUpdateScheduleDTODetails();

            var userId = await DbHelper.SetupUser(_client);
            var scheduleId = await DbHelper.SetupSchedule(_client, userId);

            _client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue(
                "Bearer",
                TestAuthorization.GetToken("User", "User", new Guid())
            );
            var response = await _client.PatchAsJsonAsync(
                $"/api/Schedule/{scheduleId}",
                scheduleUpdateRequest
            );

            var jsonResponse = await response.Content.ReadFromJsonAsync<ApiGetResponse<Schedule>>();

            await DbHelper.UserCleaner(_client, userId);

            Assert.Equal(HttpStatusCode.NotFound, response.StatusCode);
        }

        [Fact]
        public async Task UpdateScheduleThrowsUnothorizedErrorWhenNoTokenIsGiven()
        {
            var scheduleUpdateRequest = MockData.GetUpdateScheduleDTODetails();

            var userId = await DbHelper.SetupUser(_client);
            var scheduleId = await DbHelper.SetupSchedule(_client, userId);

            _client.DefaultRequestHeaders.Authorization = null;
            var response = await _client.PatchAsJsonAsync(
                $"/api/Schedule/{scheduleId}",
                scheduleUpdateRequest
            );

            var jsonResponse = await response.Content.ReadFromJsonAsync<ApiGetResponse<Schedule>>();

            await DbHelper.UserCleaner(_client, userId);

            Assert.Equal(HttpStatusCode.Unauthorized, response.StatusCode);
        }

        [Fact]
        public async Task GetScheduleByNameReturnsSchedule()
        {
            var scheduleRequest = MockData.GetCreateScheduleDTODetails();

            var userId = await DbHelper.SetupUser(_client);
            await DbHelper.SetupSchedule(_client, userId);

            _client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue(
                "Bearer",
                TestAuthorization.GetToken("Admin", "User", userId)
            );
            var response = await _client.GetAsync($"/api/Schedule/byName/{scheduleRequest.Name}");
            var jsonResponse = await response.Content.ReadFromJsonAsync<ApiGetResponse<Schedule>>();

            await DbHelper.UserCleaner(_client, userId);

            Assert.Equal(scheduleRequest.Name, jsonResponse!.Content.Name);
            Assert.Equal(scheduleRequest.SchoolHour, jsonResponse!.Content.SchoolHour);
        }

        [Fact]
        public async Task GetScheduleByNameThrowsNotFoundErrorWhenWrongUserAttemptsGet()
        {
            var scheduleRequest = MockData.GetCreateScheduleDTODetails();

            var userId = await DbHelper.SetupUser(_client);
            await DbHelper.SetupSchedule(_client, userId);

            _client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue(
                "Bearer",
                TestAuthorization.GetToken("User", "User", new Guid())
            );
            var response = await _client.GetAsync($"/api/Schedule/byName/{scheduleRequest.Name}");

            await DbHelper.UserCleaner(_client, userId);

            Assert.Equal(HttpStatusCode.NotFound, response.StatusCode);
        }

        [Fact]
        public async Task GetScheduleByNameThrowsNotFoundErrorWhenWrongNameIsGiven()
        {
            var scheduleRequest = MockData.GetCreateScheduleDTODetails();

            var userId = await DbHelper.SetupUser(_client);
            await DbHelper.SetupSchedule(_client, userId);

            _client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue(
                "Bearer",
                TestAuthorization.GetToken("Admin", "User", userId)
            );
            var response = await _client.GetAsync($"/api/Schedule/byName/WrongName");
            var jsonResponse = await response.Content.ReadFromJsonAsync<ApiGetResponse<Schedule>>();

            await DbHelper.UserCleaner(_client, userId);

            Assert.Equal(HttpStatusCode.NotFound, response.StatusCode);
        }

        [Fact]
        public async Task GetScheduleByNameThrowsUnothorizedErrorWhenNoTokenIsGiven()
        {
            var scheduleRequest = MockData.GetCreateScheduleDTODetails();

            var userId = await DbHelper.SetupUser(_client);
            await DbHelper.SetupSchedule(_client, userId);

            _client.DefaultRequestHeaders.Authorization = null;
            var response = await _client.GetAsync($"/api/Schedule/byName/{scheduleRequest.Name}");
            var jsonResponse = await response.Content.ReadFromJsonAsync<ApiGetResponse<Schedule>>();

            await DbHelper.UserCleaner(_client, userId);

            Assert.Equal(HttpStatusCode.Unauthorized, response.StatusCode);
        }

        [Fact]
        public async Task GetScheduleReturnsSchedule()
        {
            var scheduleRequest = MockData.GetCreateScheduleDTODetails();

            var userId = await DbHelper.SetupUser(_client);
            var scheduleId = await DbHelper.SetupSchedule(_client, userId);

            _client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue(
                "Bearer",
                TestAuthorization.GetToken("Admin", "User", userId)
            );
            var response = await _client.GetAsync($"/api/Schedule/{scheduleId}");
            var jsonResponse = await response.Content.ReadFromJsonAsync<ApiGetResponse<Schedule>>();

            await DbHelper.UserCleaner(_client, userId);

            Assert.Equal(scheduleRequest.Name, jsonResponse!.Content.Name);
            Assert.Equal(scheduleRequest.SchoolHour, jsonResponse!.Content.SchoolHour);
        }

        [Fact]
        public async Task GetScheduleThrowsUnothorizedErrorWhenNoTokenIsGiven()
        {
            var userId = await DbHelper.SetupUser(_client);
            var scheduleId = await DbHelper.SetupSchedule(_client, userId);

            _client.DefaultRequestHeaders.Authorization = null;
            var response = await _client.GetAsync($"/api/Schedule/{scheduleId}");

            await DbHelper.UserCleaner(_client, userId);

            Assert.Equal(HttpStatusCode.Unauthorized, response.StatusCode);
        }

        [Fact]
        public async Task GetScheduleThrowsNotFoundErrorWhenWrongUserAttemptsGet()
        {
            var userId = await DbHelper.SetupUser(_client);
            var scheduleId = await DbHelper.SetupSchedule(_client, userId);

            _client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue(
                "Bearer",
                TestAuthorization.GetToken("User", "User", new Guid())
            );
            var response = await _client.GetAsync($"/api/Schedule/{scheduleId}");

            await DbHelper.UserCleaner(_client, userId);

            Assert.Equal(HttpStatusCode.NotFound, response.StatusCode);
        }

        [Fact]
        public async Task GetScheduleThrowsNotFoundErrorWhenWrongIdIsGiven()
        {
            var userId = await DbHelper.SetupUser(_client);
            await DbHelper.SetupSchedule(_client, userId);

            _client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue(
                "Bearer",
                TestAuthorization.GetToken("Admin", "User", userId)
            );
            var response = await _client.GetAsync($"/api/Schedule/{new Guid()}");

            await DbHelper.UserCleaner(_client, userId);

            Assert.Equal(HttpStatusCode.NotFound, response.StatusCode);
        }

        [Fact]
        public async Task GetAllScheduleTReturnsSchedules()
        {
            var scheduleRequest1 = MockData.GetCreateScheduleDTODetails();
            var scheduleRequest2 = MockData.GetCreateSecondScheduleDTODetails();

            var userId1 = await DbHelper.SetupUser(_client);
            var userId2 = await DbHelper.SetupSecondUser(_client);

            await DbHelper.SetupSchedule(_client, userId1);
            await DbHelper.SetupSecondSchedule(_client, userId2);

            _client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue(
                "Bearer",
                TestAuthorization.GetToken("Admin", "User", userId1)
            );
            var response = await _client.GetAsync("/api/Schedule");
            var stringResponse = await response.Content.ReadAsStringAsync();

            await DbHelper.UserCleaner(_client, userId1);
            await DbHelper.UserCleaner(_client, userId2);

            Assert.Contains(scheduleRequest1.Name, stringResponse);
            Assert.Contains(scheduleRequest2.Name, stringResponse);
        }

        [Fact]
        public async Task GetAllScheduleTReturnsEmptyContentWhenWrongUserAttemptsGet()
        {
            var scheduleRequest1 = MockData.GetCreateScheduleDTODetails();
            var scheduleRequest2 = MockData.GetCreateSecondScheduleDTODetails();

            var userId1 = await DbHelper.SetupUser(_client);
            var userId2 = await DbHelper.SetupSecondUser(_client);

            await DbHelper.SetupSchedule(_client, userId1);
            await DbHelper.SetupSecondSchedule(_client, userId2);

            _client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue(
                "Bearer",
                TestAuthorization.GetToken("User", "User", new Guid())
            );
            var response = await _client.GetAsync("/api/Schedule");
            var stringResponse = await response.Content.ReadAsStringAsync();

            await DbHelper.UserCleaner(_client, userId1);
            await DbHelper.UserCleaner(_client, userId2);

            Assert.DoesNotContain(scheduleRequest1.Name, stringResponse);
            Assert.DoesNotContain(scheduleRequest2.Name, stringResponse);
        }

        [Fact]
        public async Task GetAllScheduleTReturnsOnlyUserMadeSchedules()
        {
            var scheduleRequest1 = MockData.GetCreateScheduleDTODetails();
            var scheduleRequest2 = MockData.GetCreateSecondScheduleDTODetails();

            var userId1 = await DbHelper.SetupUser(_client);
            var userId2 = await DbHelper.SetupSecondUser(_client);

            await DbHelper.SetupSchedule(_client, userId1);
            await DbHelper.SetupSecondSchedule(_client, userId2);

            _client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue(
                "Bearer",
                TestAuthorization.GetToken("User", "User", userId1)
            );
            var response = await _client.GetAsync("/api/Schedule");
            var stringResponse = await response.Content.ReadAsStringAsync();

            await DbHelper.UserCleaner(_client, userId1);
            await DbHelper.UserCleaner(_client, userId2);

            Assert.Contains(scheduleRequest1.Name, stringResponse);
            Assert.DoesNotContain(scheduleRequest2.Name, stringResponse);
        }

        [Fact]
        public async Task ThrowsUnothorizedErrorWhenNoTokenIsGiven()
        {
            var userId1 = await DbHelper.SetupUser(_client);
            var userId2 = await DbHelper.SetupSecondUser(_client);

            await DbHelper.SetupSchedule(_client, userId1);
            await DbHelper.SetupSecondSchedule(_client, userId2);

            _client.DefaultRequestHeaders.Authorization = null;
            var response = await _client.GetAsync("/api/Schedule");

            await DbHelper.UserCleaner(_client, userId1);
            await DbHelper.UserCleaner(_client, userId2);

            Assert.Equal(HttpStatusCode.Unauthorized, response.StatusCode);
        }
    }
}
