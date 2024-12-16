using System.Net;
using System.Net.Http.Headers;
using AlpimiAPI.Entities.ETeacher;
using AlpimiAPI.Entities.ETeacher.DTO;
using AlpimiAPI.Responses;
using AlpimiAPI.Settings;
using AlpimiAPI.Utilities;
using AlpimiTest.TestSetup;
using AlpimiTest.TestUtilities;
using Xunit;

namespace AlpimiTest.Entities.ETeacher
{
    [Collection("Sequential Tests")]
    public class TeacherControllerTest : IAsyncLifetime
    {
        CustomWebApplicationFactory<Program> _factory;
        HttpClient _client;
        Guid userId;
        Guid scheduleId;

        public TeacherControllerTest()
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
        public async Task TeacherIsDeleted()
        {
            var teacherRequest = MockData.GetCreateTeacherDTODetails(scheduleId);
            var teacherId = await DbHelper.SetupTeacher(_client, teacherRequest);

            _client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue(
                "Bearer",
                TestAuthorization.GetToken("Admin", "User", new Guid())
            );

            var response = await _client.DeleteAsync($"/api/Teacher/{teacherId}");
            Assert.Equal(HttpStatusCode.NoContent, response.StatusCode);

            var query = $"?scheduleId={scheduleId}";
            response = await _client.GetAsync($"/api/Teacher");
            var stringResponse = await response.Content.ReadAsStringAsync();
            Assert.DoesNotContain(teacherRequest.Name, stringResponse);
        }

        [Fact]
        public async Task TeacherIsCreated()
        {
            var teacherRequest = MockData.GetCreateTeacherDTODetails(scheduleId);

            _client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue(
                "Bearer",
                TestAuthorization.GetToken("Admin", "User", userId)
            );

            var response = await _client.PostAsJsonAsync("/api/Teacher", teacherRequest);
            Assert.Equal(HttpStatusCode.OK, response.StatusCode);

            var query = $"?scheduleId={scheduleId}";
            response = await _client.GetAsync($"/api/Teacher{query}");
            var stringResponse = await response.Content.ReadAsStringAsync();
            Assert.Contains(teacherRequest.Name, stringResponse);
        }

        [Fact]
        public async Task UpdateTeacherReturnsUpdatedTeacher()
        {
            var teacherUpdateRequest = MockData.GetUpdateTeacherDTODetails();
            var teacherId = await DbHelper.SetupTeacher(
                _client,
                MockData.GetCreateTeacherDTODetails(scheduleId)
            );

            _client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue(
                "Bearer",
                TestAuthorization.GetToken("Admin", "Bob", userId)
            );

            var response = await _client.PatchAsJsonAsync(
                $"/api/Teacher/{teacherId}",
                teacherUpdateRequest
            );
            var jsonResponse = await response.Content.ReadFromJsonAsync<
                ApiGetResponse<TeacherDTO>
            >();

            Assert.Equal(teacherUpdateRequest.Name, jsonResponse!.Content.Name);
            Assert.Equal(teacherUpdateRequest.Surname, jsonResponse!.Content.Surname);
        }

        [Fact]
        public async Task UpdateTeacherThrowsNotFoundErrorWhenWrongIdIsGiven()
        {
            var teacherUpdateRequest = MockData.GetUpdateTeacherDTODetails();

            var teacherId = await DbHelper.SetupTeacher(
                _client,
                MockData.GetCreateTeacherDTODetails(scheduleId)
            );

            _client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue(
                "Bearer",
                TestAuthorization.GetToken("Admin", "User", userId)
            );
            var response = await _client.PatchAsJsonAsync(
                $"/api/Teacher/{new Guid()}",
                teacherUpdateRequest
            );

            Assert.Equal(HttpStatusCode.NotFound, response.StatusCode);
        }

        [Fact]
        public async Task UpdateTeacherThrowsNotFoundErrorWhenWrongUserAttemptsUpdate()
        {
            var teacherUpdateRequest = MockData.GetUpdateTeacherDTODetails();

            var teacherId = await DbHelper.SetupTeacher(
                _client,
                MockData.GetCreateTeacherDTODetails(scheduleId)
            );

            _client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue(
                "Bearer",
                TestAuthorization.GetToken("User", "User", new Guid())
            );
            var response = await _client.PatchAsJsonAsync(
                $"/api/Teacher/{teacherId}",
                teacherUpdateRequest
            );

            Assert.Equal(HttpStatusCode.NotFound, response.StatusCode);
        }

        [Fact]
        public async Task GetAllTeacherByScheduleReturnsDaysOff()
        {
            var teacherRequest1 = MockData.GetCreateTeacherDTODetails(scheduleId);
            var teacherRequest2 = MockData.GetCreateSecondTeacherDTODetails(scheduleId);

            await DbHelper.SetupTeacher(_client, teacherRequest1);
            await DbHelper.SetupTeacher(_client, teacherRequest2);

            _client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue(
                "Bearer",
                TestAuthorization.GetToken("Admin", "User", userId)
            );
            var query = $"?scheduleId={scheduleId}";
            var response = await _client.GetAsync($"/api/Teacher{query}");
            var stringResponse = await response.Content.ReadAsStringAsync();

            Assert.Contains(teacherRequest1.Name, stringResponse);
            Assert.Contains(teacherRequest2.Name, stringResponse);
        }

        [Fact]
        public async Task GetAllTeacherByScheduleReturnsEmptyContentWhenWrongUserAttemptsGet()
        {
            var teacherRequest1 = MockData.GetCreateTeacherDTODetails(scheduleId);
            var teacherRequest2 = MockData.GetCreateSecondTeacherDTODetails(scheduleId);

            await DbHelper.SetupTeacher(_client, teacherRequest1);
            await DbHelper.SetupTeacher(_client, teacherRequest2);

            _client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue(
                "Bearer",
                TestAuthorization.GetToken("User", "User", new Guid())
            );
            var query = $"?scheduleId={scheduleId}";
            var response = await _client.GetAsync($"/api/Teacher{query}");
            var stringResponse = await response.Content.ReadAsStringAsync();

            Assert.DoesNotContain(teacherRequest1.Name, stringResponse);
            Assert.DoesNotContain(teacherRequest2.Name, stringResponse);
        }

        [Fact]
        public async Task GetAllTeacherByScheduleReturnsEmptyContentWhenWrongIdIsGiven()
        {
            var teacherRequest1 = MockData.GetCreateTeacherDTODetails(scheduleId);
            var teacherRequest2 = MockData.GetCreateSecondTeacherDTODetails(scheduleId);

            await DbHelper.SetupTeacher(_client, teacherRequest1);
            await DbHelper.SetupTeacher(_client, teacherRequest2);

            _client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue(
                "Bearer",
                TestAuthorization.GetToken("Admin", "User", userId)
            );
            var query = $"?scheduleId={new Guid()}";
            var response = await _client.GetAsync($"/api/Teacher{query}");
            var stringResponse = await response.Content.ReadAsStringAsync();

            Assert.DoesNotContain(teacherRequest1.Name, stringResponse);
            Assert.DoesNotContain(teacherRequest2.Name, stringResponse);
        }

        [Fact]
        public async Task GetTeacherReturnsTeacher()
        {
            var teacherRequest = MockData.GetCreateTeacherDTODetails(scheduleId);

            var teacherId = await DbHelper.SetupTeacher(_client, teacherRequest);

            _client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue(
                "Bearer",
                TestAuthorization.GetToken("Admin", "User", userId)
            );

            var response = await _client.GetAsync($"/api/Teacher/{teacherId}");
            var jsonResponse = await response.Content.ReadFromJsonAsync<
                ApiGetResponse<TeacherDTO>
            >();

            Assert.Equal(teacherRequest.Name, jsonResponse!.Content.Name);
            Assert.Equal(teacherRequest.Surname, jsonResponse!.Content.Surname);
        }

        [Fact]
        public async Task GetScheduleThrowsNotFoundWhenWrongIdIsGiven()
        {
            var teacherRequest = MockData.GetCreateTeacherDTODetails(scheduleId);

            await DbHelper.SetupTeacher(_client, teacherRequest);

            _client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue(
                "Bearer",
                TestAuthorization.GetToken("Admin", "User", userId)
            );

            var response = await _client.GetAsync($"/api/Teacher/{new Guid()}");

            Assert.Equal(HttpStatusCode.NotFound, response.StatusCode);
        }

        [Fact]
        public async Task GetScheduleThrowsNotFoundErrorWhenWrongUserTokenIsGiven()
        {
            var teacherRequest = MockData.GetCreateTeacherDTODetails(scheduleId);

            var teacherId = await DbHelper.SetupTeacher(_client, teacherRequest);

            _client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue(
                "Bearer",
                TestAuthorization.GetToken("User", "User", new Guid())
            );

            var response = await _client.GetAsync($"/api/Teacher/{teacherId}");

            Assert.Equal(HttpStatusCode.NotFound, response.StatusCode);
        }

        [Fact]
        public async Task TeacherControllerThrowsUnauthorized()
        {
            _client.DefaultRequestHeaders.Authorization = null;

            var response = await _client.DeleteAsync($"/api/Teacher/{new Guid()}");
            Assert.Equal(HttpStatusCode.Unauthorized, response.StatusCode);

            response = await _client.PostAsJsonAsync(
                "/api/Teacher",
                MockData.GetCreateTeacherDTODetails(scheduleId)
            );
            Assert.Equal(HttpStatusCode.Unauthorized, response.StatusCode);

            var query = $"?scheduleId={new Guid()}";
            response = await _client.GetAsync($"/api/Teacher{query}");
            Assert.Equal(HttpStatusCode.Unauthorized, response.StatusCode);

            response = await _client.PatchAsJsonAsync(
                $"/api/Teacher/{new Guid()}",
                MockData.GetUpdateTeacherDTODetails()
            );
            Assert.Equal(HttpStatusCode.Unauthorized, response.StatusCode);

            response = await _client.GetAsync($"/api/Teacher/{new Guid()}");
            Assert.Equal(HttpStatusCode.Unauthorized, response.StatusCode);
        }

        [Fact]
        public async Task TeacherControllerThrowsTooManyRequests()
        {
            for (int i = 0; i != Configuration.GetPermitLimit(); i++)
            {
                await _client.GetAsync("/api/Teacher");
            }

            _client.DefaultRequestHeaders.Authorization = null;

            var response = await _client.DeleteAsync($"/api/Teacher/{new Guid()}");
            Assert.Equal(HttpStatusCode.TooManyRequests, response.StatusCode);

            response = await _client.PostAsJsonAsync(
                "/api/Teacher",
                MockData.GetCreateTeacherDTODetails(scheduleId)
            );
            Assert.Equal(HttpStatusCode.TooManyRequests, response.StatusCode);

            var query = $"?scheduleId={new Guid()}";
            response = await _client.GetAsync($"/api/Teacher{query}");
            Assert.Equal(HttpStatusCode.TooManyRequests, response.StatusCode);

            response = await _client.PatchAsJsonAsync(
                $"/api/Teacher/{new Guid()}",
                MockData.GetUpdateTeacherDTODetails()
            );
            Assert.Equal(HttpStatusCode.TooManyRequests, response.StatusCode);

            response = await _client.GetAsync($"/api/Teacher/{new Guid()}");
            Assert.Equal(HttpStatusCode.TooManyRequests, response.StatusCode);
        }
    }
}
