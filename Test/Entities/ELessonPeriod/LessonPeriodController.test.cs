using System.Net;
using System.Net.Http.Headers;
using AlpimiAPI.Entities.ELessonPeriod.DTO;
using AlpimiAPI.Entities.ESchedule;
using AlpimiAPI.Responses;
using AlpimiAPI.Utilities;
using AlpimiTest.TestSetup;
using AlpimiTest.TestUtilities;
using Xunit;

namespace AlpimiTest.Entities.ELessonPeriod
{
    [Collection("Sequential Tests")]
    public class LessonPeriodControllerTest : IAsyncLifetime
    {
        CustomWebApplicationFactory<Program> _factory;
        HttpClient _client;
        Guid userId;
        Guid scheduleId;

        public LessonPeriodControllerTest()
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
        public async Task LessonPeriodControllerThrowsUnauthorized()
        {
            _client.DefaultRequestHeaders.Authorization = null;

            var response = await _client.DeleteAsync($"/api/LessonPeriod/{new Guid()}");
            Assert.Equal(HttpStatusCode.Unauthorized, response.StatusCode);

            response = await _client.PostAsJsonAsync(
                "/api/LessonPeriod",
                MockData.GetCreateLessonPeriodDTODetails(scheduleId)
            );
            Assert.Equal(HttpStatusCode.Unauthorized, response.StatusCode);

            var query = $"?scheduleId={new Guid()}";
            response = await _client.GetAsync($"/api/LessonPeriod{query}");
            Assert.Equal(HttpStatusCode.Unauthorized, response.StatusCode);

            response = await _client.PatchAsJsonAsync(
                $"/api/LessonPeriod/{new Guid()}",
                MockData.GetUpdateLessonPeriodDTODetails()
            );
            Assert.Equal(HttpStatusCode.Unauthorized, response.StatusCode);
        }

        [Fact]
        public async Task LessonPeriodControllerThrowsTooManyRequests()
        {
            for (int i = 0; i != Configuration.GetPermitLimit(); i++)
            {
                await _client.GetAsync("/api/LessonPeriod");
            }

            _client.DefaultRequestHeaders.Authorization = null;

            var response = await _client.DeleteAsync($"/api/LessonPeriod/{new Guid()}");
            Assert.Equal(HttpStatusCode.TooManyRequests, response.StatusCode);

            response = await _client.PostAsJsonAsync(
                "/api/LessonPeriod",
                MockData.GetCreateLessonPeriodDTODetails(scheduleId)
            );
            Assert.Equal(HttpStatusCode.TooManyRequests, response.StatusCode);

            var query = $"?scheduleId={new Guid()}";
            response = await _client.GetAsync($"/api/LessonPeriod{query}");
            Assert.Equal(HttpStatusCode.TooManyRequests, response.StatusCode);

            response = await _client.PatchAsJsonAsync(
                $"/api/LessonPeriod/{new Guid()}",
                MockData.GetUpdateLessonPeriodDTODetails()
            );
            Assert.Equal(HttpStatusCode.TooManyRequests, response.StatusCode);
        }

        [Fact]
        public async Task LessonPeriodIsCreated()
        {
            var lessonPeriodRequest = MockData.GetCreateLessonPeriodDTODetails(scheduleId);

            _client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue(
                "Bearer",
                TestAuthorization.GetToken("Admin", "User", userId)
            );

            var response = await _client.PostAsJsonAsync("/api/LessonPeriod", lessonPeriodRequest);
            Assert.Equal(HttpStatusCode.OK, response.StatusCode);

            var query = $"?scheduleId={scheduleId}";
            response = await _client.GetAsync($"/api/LessonPeriod{query}");
            var stringResponse = await response.Content.ReadAsStringAsync();
            Assert.Contains(lessonPeriodRequest.Start.ToString(), stringResponse);
        }

        [Fact]
        public async Task LessonPeriodIsDeleted()
        {
            var lessonPeriodRequest = MockData.GetCreateLessonPeriodDTODetails(scheduleId);
            var lessonPeriodId = await DbHelper.SetupLessonPeriod(_client, lessonPeriodRequest);

            _client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue(
                "Bearer",
                TestAuthorization.GetToken("Admin", "User", new Guid())
            );

            var response = await _client.DeleteAsync($"/api/LessonPeriod/{lessonPeriodId}");
            Assert.Equal(HttpStatusCode.NoContent, response.StatusCode);

            var query = $"?scheduleId={scheduleId}";
            response = await _client.GetAsync($"/api/LessonPeriod");
            var stringResponse = await response.Content.ReadAsStringAsync();
            Assert.DoesNotContain(lessonPeriodRequest.Start.ToString(), stringResponse);
        }

        [Fact]
        public async Task UpdateLessonPeriodReturnsUpdatedLessonPeriod()
        {
            var lessonPeriodUpdateRequest = MockData.GetUpdateLessonPeriodDTODetails();
            var lessonPeriodId = await DbHelper.SetupLessonPeriod(
                _client,
                MockData.GetCreateLessonPeriodDTODetails(scheduleId)
            );

            _client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue(
                "Bearer",
                TestAuthorization.GetToken("Admin", "Bob", userId)
            );

            var response = await _client.PatchAsJsonAsync(
                $"/api/LessonPeriod/{lessonPeriodId}",
                lessonPeriodUpdateRequest
            );
            Assert.Equal(HttpStatusCode.OK, response.StatusCode);
            var jsonResponse = await response.Content.ReadFromJsonAsync<
                ApiGetResponse<LessonPeriodDTO>
            >();

            Assert.Equal(lessonPeriodUpdateRequest.Start, jsonResponse!.Content.Start);
        }

        [Fact]
        public async Task UpdateLessonPeriodThrowsNotFoundErrorWhenWrongIdIsGiven()
        {
            var lessonPeriodUpdateRequest = MockData.GetUpdateLessonPeriodDTODetails();

            var lessonPeriodId = await DbHelper.SetupLessonPeriod(
                _client,
                MockData.GetCreateLessonPeriodDTODetails(scheduleId)
            );

            _client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue(
                "Bearer",
                TestAuthorization.GetToken("Admin", "User", userId)
            );
            var response = await _client.PatchAsJsonAsync(
                $"/api/LessonPeriod/{new Guid()}",
                lessonPeriodUpdateRequest
            );

            Assert.Equal(HttpStatusCode.NotFound, response.StatusCode);
        }

        [Fact]
        public async Task UpdateLessonPeriodThrowsNotFoundErrorWhenWrongUserAttemptsUpdate()
        {
            var lessonPeriodUpdateRequest = MockData.GetUpdateLessonPeriodDTODetails();

            var lessonPeriodId = await DbHelper.SetupLessonPeriod(
                _client,
                MockData.GetCreateLessonPeriodDTODetails(scheduleId)
            );

            _client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue(
                "Bearer",
                TestAuthorization.GetToken("User", "User", new Guid())
            );
            var response = await _client.PatchAsJsonAsync(
                $"/api/LessonPeriod/{lessonPeriodId}",
                lessonPeriodUpdateRequest
            );

            var jsonResponse = await response.Content.ReadFromJsonAsync<ApiGetResponse<Schedule>>();

            Assert.Equal(HttpStatusCode.NotFound, response.StatusCode);
        }

        [Fact]
        public async Task GetAllLessonPeriodByScheduleReturnsLessonPeriods()
        {
            var lessonPeriodRequest1 = MockData.GetCreateLessonPeriodDTODetails(scheduleId);
            var lessonPeriodRequest2 = MockData.GetCreateSecondLessonPeriodDTODetails(scheduleId);

            await DbHelper.SetupLessonPeriod(_client, lessonPeriodRequest1);
            var lessonPeriodId = await DbHelper.SetupLessonPeriod(_client, lessonPeriodRequest2);

            _client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue(
                "Bearer",
                TestAuthorization.GetToken("Admin", "User", userId)
            );
            var query = $"?scheduleId={scheduleId}";
            var response = await _client.GetAsync($"/api/LessonPeriod{query}");

            var stringResponse = await response.Content.ReadAsStringAsync();

            Assert.Contains(lessonPeriodRequest1.Start.ToString(), stringResponse);
            Assert.Contains(lessonPeriodRequest2.Start.ToString(), stringResponse);
        }

        [Fact]
        public async Task GetAllLessonPeriodByScheduleReturnsEmptyContentWhenWrongUserAttemptsGet()
        {
            var lessonPeriodRequest1 = MockData.GetCreateLessonPeriodDTODetails(scheduleId);
            var lessonPeriodRequest2 = MockData.GetCreateSecondLessonPeriodDTODetails(scheduleId);

            await DbHelper.SetupLessonPeriod(_client, lessonPeriodRequest1);
            await DbHelper.SetupLessonPeriod(_client, lessonPeriodRequest2);

            _client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue(
                "Bearer",
                TestAuthorization.GetToken("User", "User", new Guid())
            );
            var query = $"?scheduleId={scheduleId}";
            var response = await _client.GetAsync($"/api/LessonPeriod{query}");
            var stringResponse = await response.Content.ReadAsStringAsync();

            Assert.DoesNotContain(lessonPeriodRequest1.Start.ToString(), stringResponse);
            Assert.DoesNotContain(lessonPeriodRequest2.Start.ToString(), stringResponse);
        }

        [Fact]
        public async Task GetAllLessonPeriodByScheduleReturnsEmptyContentWhenWrongIdIsGiven()
        {
            var lessonPeriodRequest1 = MockData.GetCreateLessonPeriodDTODetails(scheduleId);
            var lessonPeriodRequest2 = MockData.GetCreateSecondLessonPeriodDTODetails(scheduleId);

            await DbHelper.SetupLessonPeriod(_client, lessonPeriodRequest1);
            await DbHelper.SetupLessonPeriod(_client, lessonPeriodRequest2);

            _client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue(
                "Bearer",
                TestAuthorization.GetToken("Admin", "User", userId)
            );
            var query = $"?scheduleId={new Guid()}";
            var response = await _client.GetAsync($"/api/LessonPeriod{query}");
            var stringResponse = await response.Content.ReadAsStringAsync();

            Assert.DoesNotContain(lessonPeriodRequest1.Start.ToString(), stringResponse);
            Assert.DoesNotContain(lessonPeriodRequest2.Start.ToString(), stringResponse);
        }
    }
}
