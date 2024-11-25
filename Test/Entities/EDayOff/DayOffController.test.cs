using System.Net;
using System.Net.Http.Headers;
using AlpimiAPI.Entities.EDayOff;
using AlpimiAPI.Entities.ESchedule;
using AlpimiAPI.Responses;
using AlpimiAPI.Settings;
using AlpimiTest.TestUtilities;
using Xunit;

namespace AlpimiTest.Entities.EDayOff
{
    [Collection("Sequential Tests")]
    public class DayOffControllerTest : IAsyncLifetime
    {
        CustomWebApplicationFactory<Program> _factory;
        HttpClient _client;
        Guid userId;
        Guid scheduleId;

        public DayOffControllerTest()
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
        public async Task DayOffIsDeleted()
        {
            var dayOffRequest = MockData.GetCreateDayOffDTODetails(scheduleId);
            var dayOffId = await DbHelper.SetupDayOff(_client, dayOffRequest);

            _client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue(
                "Bearer",
                TestAuthorization.GetToken("Admin", "User", new Guid())
            );

            var response = await _client.DeleteAsync($"/api/DayOff/{dayOffId}");
            Assert.Equal(HttpStatusCode.NoContent, response.StatusCode);

            var query = $"?scheduleId={scheduleId}";
            response = await _client.GetAsync($"/api/DayOff");
            var stringResponse = await response.Content.ReadAsStringAsync();
            Assert.DoesNotContain(dayOffRequest.Name, stringResponse);
        }

        [Fact]
        public async Task DayOffIsCreated()
        {
            var dayOffRequest = MockData.GetCreateDayOffDTODetails(scheduleId);

            _client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue(
                "Bearer",
                TestAuthorization.GetToken("Admin", "User", userId)
            );

            var response = await _client.PostAsJsonAsync("/api/DayOff", dayOffRequest);
            Assert.Equal(HttpStatusCode.OK, response.StatusCode);

            var query = $"?scheduleId={scheduleId}";
            response = await _client.GetAsync($"/api/DayOff{query}");
            var stringResponse = await response.Content.ReadAsStringAsync();
            Assert.Contains(dayOffRequest.Name, stringResponse);
        }

        [Fact]
        public async Task UpdateDayOffReturnsUpdatedDayOff()
        {
            var dayOffUpdateRequest = MockData.GetUpdateDayOffDTODetails();
            var dayOffId = await DbHelper.SetupDayOff(
                _client,
                MockData.GetCreateDayOffDTODetails(scheduleId)
            );

            _client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue(
                "Bearer",
                TestAuthorization.GetToken("Admin", "Bob", userId)
            );

            var response = await _client.PatchAsJsonAsync(
                $"/api/DayOff/{dayOffId}",
                dayOffUpdateRequest
            );
            var jsonResponse = await response.Content.ReadFromJsonAsync<ApiGetResponse<DayOff>>();

            Assert.Equal(dayOffUpdateRequest.Name, jsonResponse!.Content.Name);
            Assert.Equal(dayOffUpdateRequest.From, jsonResponse!.Content.From);
            Assert.Equal(dayOffUpdateRequest.To, jsonResponse!.Content.To);
        }

        [Fact]
        public async Task UpdateDayOffThrowsNotFoundErrorWhenWrongIdIsGiven()
        {
            var dayOffUpdateRequest = MockData.GetUpdateDayOffDTODetails();

            var dayOffId = await DbHelper.SetupDayOff(
                _client,
                MockData.GetCreateDayOffDTODetails(scheduleId)
            );

            _client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue(
                "Bearer",
                TestAuthorization.GetToken("Admin", "User", userId)
            );
            var response = await _client.PatchAsJsonAsync(
                $"/api/DayOff/{new Guid()}",
                dayOffUpdateRequest
            );

            Assert.Equal(HttpStatusCode.NotFound, response.StatusCode);
        }

        [Fact]
        public async Task UpdateDayOffThrowsNotFoundErrorWhenWrongUserAttemptsUpdate()
        {
            var dayOffUpdateRequest = MockData.GetUpdateDayOffDTODetails();

            var dayOffId = await DbHelper.SetupDayOff(
                _client,
                MockData.GetCreateDayOffDTODetails(scheduleId)
            );

            _client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue(
                "Bearer",
                TestAuthorization.GetToken("User", "User", new Guid())
            );
            var response = await _client.PatchAsJsonAsync(
                $"/api/DayOff/{dayOffId}",
                dayOffUpdateRequest
            );

            var jsonResponse = await response.Content.ReadFromJsonAsync<ApiGetResponse<Schedule>>();

            Assert.Equal(HttpStatusCode.NotFound, response.StatusCode);
        }

        [Fact]
        public async Task GetAllDayOffByScheduleReturnsDaysOff()
        {
            var dayOffRequest1 = MockData.GetCreateDayOffDTODetails(scheduleId);
            var dayOffRequest2 = MockData.GetCreateSecondDayOffDTODetails(scheduleId);

            await DbHelper.SetupDayOff(_client, dayOffRequest1);
            await DbHelper.SetupDayOff(_client, dayOffRequest2);

            _client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue(
                "Bearer",
                TestAuthorization.GetToken("Admin", "User", userId)
            );
            var query = $"?scheduleId={scheduleId}";
            var response = await _client.GetAsync($"/api/DayOff{query}");
            var stringResponse = await response.Content.ReadAsStringAsync();

            Assert.Contains(dayOffRequest1.Name, stringResponse);
            Assert.Contains(dayOffRequest2.Name, stringResponse);
        }

        [Fact]
        public async Task GetAllDayOffByScheduleReturnsEmptyContentWhenWrongUserAttemptsGet()
        {
            var dayOffRequest1 = MockData.GetCreateDayOffDTODetails(scheduleId);
            var dayOffRequest2 = MockData.GetCreateSecondDayOffDTODetails(scheduleId);

            await DbHelper.SetupDayOff(_client, dayOffRequest1);
            await DbHelper.SetupDayOff(_client, dayOffRequest2);

            _client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue(
                "Bearer",
                TestAuthorization.GetToken("User", "User", new Guid())
            );
            var query = $"?scheduleId={scheduleId}";
            var response = await _client.GetAsync($"/api/DayOff{query}");
            var stringResponse = await response.Content.ReadAsStringAsync();

            Assert.DoesNotContain(dayOffRequest1.Name, stringResponse);
            Assert.DoesNotContain(dayOffRequest2.Name, stringResponse);
        }

        [Fact]
        public async Task GetAllDayOffByScheduleReturnsEmptyContentWhenWrongIdIsGiven()
        {
            var dayOffRequest1 = MockData.GetCreateDayOffDTODetails(scheduleId);
            var dayOffRequest2 = MockData.GetCreateSecondDayOffDTODetails(scheduleId);

            await DbHelper.SetupDayOff(_client, dayOffRequest1);
            await DbHelper.SetupDayOff(_client, dayOffRequest2);

            _client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue(
                "Bearer",
                TestAuthorization.GetToken("Admin", "User", userId)
            );
            var query = $"?scheduleId={new Guid()}";
            var response = await _client.GetAsync($"/api/DayOff{query}");
            var stringResponse = await response.Content.ReadAsStringAsync();

            Assert.DoesNotContain(dayOffRequest1.Name, stringResponse);
            Assert.DoesNotContain(dayOffRequest2.Name, stringResponse);
        }

        [Fact]
        public async Task DayOffControllerThrowsUnauthorized()
        {
            _client.DefaultRequestHeaders.Authorization = null;

            var response = await _client.DeleteAsync($"/api/DayOff/{new Guid()}");
            Assert.Equal(HttpStatusCode.Unauthorized, response.StatusCode);

            response = await _client.PostAsJsonAsync(
                "/api/DayOff",
                MockData.GetCreateDayOffDTODetails(scheduleId)
            );
            Assert.Equal(HttpStatusCode.Unauthorized, response.StatusCode);

            var query = $"?scheduleId={new Guid()}";
            response = await _client.GetAsync($"/api/DayOff{query}");
            Assert.Equal(HttpStatusCode.Unauthorized, response.StatusCode);

            response = await _client.PatchAsJsonAsync(
                $"/api/DayOff/{new Guid()}",
                MockData.GetUpdateDayOffDTODetails()
            );
            Assert.Equal(HttpStatusCode.Unauthorized, response.StatusCode);
        }

        [Fact]
        public async Task DayOffControllerThrowsTooManyRequests()
        {
            for (int i = 0; i != RateLimiterSettings.permitLimit; i++)
            {
                await _client.GetAsync("/api/DayOff");
            }

            _client.DefaultRequestHeaders.Authorization = null;

            var response = await _client.DeleteAsync($"/api/DayOff/{new Guid()}");
            Assert.Equal(HttpStatusCode.TooManyRequests, response.StatusCode);

            response = await _client.PostAsJsonAsync(
                "/api/DayOff",
                MockData.GetCreateDayOffDTODetails(scheduleId)
            );
            Assert.Equal(HttpStatusCode.TooManyRequests, response.StatusCode);

            var query = $"?scheduleId={new Guid()}";
            response = await _client.GetAsync($"/api/DayOff{query}");
            Assert.Equal(HttpStatusCode.TooManyRequests, response.StatusCode);

            response = await _client.PatchAsJsonAsync(
                $"/api/DayOff/{new Guid()}",
                MockData.GetUpdateDayOffDTODetails()
            );
            Assert.Equal(HttpStatusCode.TooManyRequests, response.StatusCode);
        }
    }
}
