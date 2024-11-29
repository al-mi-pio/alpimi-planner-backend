using System.Net;
using System.Net.Http.Headers;
using AlpimiAPI.Entities.EStudent;
using AlpimiAPI.Responses;
using AlpimiAPI.Settings;
using AlpimiTest.TestSetup;
using AlpimiTest.TestUtilities;
using Xunit;

namespace AlpimiTest.Entities.EStudent
{
    [Collection("Sequential Tests")]
    public class StudentControllerTest : IAsyncLifetime
    {
        CustomWebApplicationFactory<Program> _factory;
        HttpClient _client;
        Guid userId;
        Guid scheduleId;
        Guid groupId1;
        Guid groupId2;

        public StudentControllerTest()
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
            groupId1 = await DbHelper.SetupGroup(
                _client,
                MockData.GetCreateGroupDTODetails(scheduleId)
            );
            groupId2 = await DbHelper.SetupGroup(
                _client,
                MockData.GetCreateSecondGroupDTODetails(scheduleId)
            );
        }

        public async Task DisposeAsync()
        {
            await DbHelper.UserCleaner(_client);
        }

        [Fact]
        public async Task StudentIsDeleted()
        {
            var studentRequest = MockData.GetCreateStudentDTODetails(groupId1);
            var studentId = await DbHelper.SetupStudent(_client, studentRequest);

            _client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue(
                "Bearer",
                TestAuthorization.GetToken("Admin", "User", new Guid())
            );

            var response = await _client.DeleteAsync($"/api/Student/{studentId}");
            Assert.Equal(HttpStatusCode.NoContent, response.StatusCode);

            var query = $"?groupId1={groupId1}";
            response = await _client.GetAsync($"/api/Student");
            var stringResponse = await response.Content.ReadAsStringAsync();
            Assert.DoesNotContain(studentRequest.AlbumNumber, stringResponse);
        }

        [Fact]
        public async Task StudentIsCreated()
        {
            var studentRequest = MockData.GetCreateStudentDTODetails(groupId1);

            _client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue(
                "Bearer",
                TestAuthorization.GetToken("Admin", "User", userId)
            );

            var response = await _client.PostAsJsonAsync("/api/Student", studentRequest);
            Assert.Equal(HttpStatusCode.OK, response.StatusCode);

            var query = $"?Id={groupId1}";
            response = await _client.GetAsync($"/api/Student{query}");
            var stringResponse = await response.Content.ReadAsStringAsync();
            Assert.Contains(studentRequest.AlbumNumber, stringResponse);
        }

        [Fact]
        public async Task UpdateStudentReturnsUpdatedStudent()
        {
            var studentUpdateRequest = MockData.GetUpdateStudentDTODetails();
            var studentId = await DbHelper.SetupStudent(
                _client,
                MockData.GetCreateStudentDTODetails(groupId1)
            );

            _client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue(
                "Bearer",
                TestAuthorization.GetToken("Admin", "Bob", userId)
            );

            var response = await _client.PatchAsJsonAsync(
                $"/api/Student/{studentId}",
                studentUpdateRequest
            );
            var jsonResponse = await response.Content.ReadFromJsonAsync<ApiGetResponse<Student>>();

            Assert.Equal(studentUpdateRequest.AlbumNumber, jsonResponse!.Content.AlbumNumber);
        }

        [Fact]
        public async Task UpdateStudentThrowsNotFoundErrorWhenWrongIdIsGiven()
        {
            var studentUpdateRequest = MockData.GetUpdateStudentDTODetails();

            var studentId = await DbHelper.SetupStudent(
                _client,
                MockData.GetCreateStudentDTODetails(groupId1)
            );

            _client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue(
                "Bearer",
                TestAuthorization.GetToken("Admin", "User", userId)
            );
            var response = await _client.PatchAsJsonAsync(
                $"/api/Student/{new Guid()}",
                studentUpdateRequest
            );

            Assert.Equal(HttpStatusCode.NotFound, response.StatusCode);
        }

        [Fact]
        public async Task UpdateStudentThrowsNotFoundErrorWhenWrongUserAttemptsUpdate()
        {
            var studentUpdateRequest = MockData.GetUpdateStudentDTODetails();

            var studentId = await DbHelper.SetupStudent(
                _client,
                MockData.GetCreateStudentDTODetails(groupId1)
            );

            _client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue(
                "Bearer",
                TestAuthorization.GetToken("User", "User", new Guid())
            );
            var response = await _client.PatchAsJsonAsync(
                $"/api/Student/{studentId}",
                studentUpdateRequest
            );

            Assert.Equal(HttpStatusCode.NotFound, response.StatusCode);
        }

        [Fact]
        public async Task GetAllStudentsReturnsStudentsFromGroupIfGroupIdIsProvided()
        {
            var studentRequest1 = MockData.GetCreateStudentDTODetails(groupId1);
            var studentRequest2 = MockData.GetCreateSecondStudentDTODetails(groupId2);

            await DbHelper.SetupStudent(_client, studentRequest1);
            await DbHelper.SetupStudent(_client, studentRequest2);

            _client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue(
                "Bearer",
                TestAuthorization.GetToken("Admin", "User", userId)
            );
            var query = $"?Id={groupId1}";
            var response = await _client.GetAsync($"/api/Student{query}");
            var stringResponse = await response.Content.ReadAsStringAsync();

            Assert.Contains(studentRequest1.AlbumNumber, stringResponse);
            Assert.DoesNotContain(studentRequest2.AlbumNumber, stringResponse);
        }

        [Fact]
        public async Task GetAllStudentsReturnsStudentsFromScheduleIfScheduleIsProvided()
        {
            var studentRequest1 = MockData.GetCreateStudentDTODetails(groupId1);
            var studentRequest2 = MockData.GetCreateSecondStudentDTODetails(groupId2);

            await DbHelper.SetupStudent(_client, studentRequest1);
            await DbHelper.SetupStudent(_client, studentRequest2);

            _client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue(
                "Bearer",
                TestAuthorization.GetToken("Admin", "User", userId)
            );
            var query = $"?Id={scheduleId}";
            var response = await _client.GetAsync($"/api/Student{query}");
            var stringResponse = await response.Content.ReadAsStringAsync();

            Assert.Contains(studentRequest1.AlbumNumber, stringResponse);
            Assert.Contains(studentRequest2.AlbumNumber, stringResponse);
        }

        [Fact]
        public async Task GetAllStudentReturnsEmptyContentWhenWrongUserAttemptsGet()
        {
            var studentRequest1 = MockData.GetCreateStudentDTODetails(groupId1);
            var studentRequest2 = MockData.GetCreateSecondStudentDTODetails(groupId2);

            await DbHelper.SetupStudent(_client, studentRequest1);
            await DbHelper.SetupStudent(_client, studentRequest2);

            _client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue(
                "Bearer",
                TestAuthorization.GetToken("User", "User", new Guid())
            );
            var query = $"?Id={groupId1}";
            var response = await _client.GetAsync($"/api/Student{query}");
            var stringResponse = await response.Content.ReadAsStringAsync();

            Assert.DoesNotContain(studentRequest1.AlbumNumber, stringResponse);
            Assert.DoesNotContain(studentRequest2.AlbumNumber, stringResponse);
        }

        [Fact]
        public async Task GetAllStudentReturnsEmptyContentWhenWrongIdIsGiven()
        {
            var studentRequest1 = MockData.GetCreateStudentDTODetails(groupId1);
            var studentRequest2 = MockData.GetCreateSecondStudentDTODetails(groupId2);

            await DbHelper.SetupStudent(_client, studentRequest1);
            await DbHelper.SetupStudent(_client, studentRequest2);

            _client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue(
                "Bearer",
                TestAuthorization.GetToken("Admin", "User", userId)
            );
            var query = $"?Id={new Guid()}";
            var response = await _client.GetAsync($"/api/Student{query}");
            var stringResponse = await response.Content.ReadAsStringAsync();

            Assert.DoesNotContain(studentRequest1.AlbumNumber, stringResponse);
            Assert.DoesNotContain(studentRequest2.AlbumNumber, stringResponse);
        }

        [Fact]
        public async Task GetStudentReturnsStudent()
        {
            var studentRequest = MockData.GetCreateStudentDTODetails(groupId1);

            var studentId = await DbHelper.SetupStudent(_client, studentRequest);

            _client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue(
                "Bearer",
                TestAuthorization.GetToken("Admin", "User", userId)
            );

            var response = await _client.GetAsync($"/api/Student/{studentId}");
            var jsonResponse = await response.Content.ReadFromJsonAsync<ApiGetResponse<Student>>();

            Assert.Equal(studentRequest.AlbumNumber, jsonResponse!.Content.AlbumNumber);
        }

        [Fact]
        public async Task GetStudentThrowsNotFoundWhenWrongIdIsGiven()
        {
            var studentRequest = MockData.GetCreateStudentDTODetails(groupId1);

            await DbHelper.SetupStudent(_client, studentRequest);

            _client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue(
                "Bearer",
                TestAuthorization.GetToken("Admin", "User", userId)
            );

            var response = await _client.GetAsync($"/api/Student/{new Guid()}");

            Assert.Equal(HttpStatusCode.NotFound, response.StatusCode);
        }

        [Fact]
        public async Task GetStudentThrowsNotFoundErrorWhenWrongUserTokenIsGiven()
        {
            var studentRequest = MockData.GetCreateStudentDTODetails(groupId1);

            var studentId = await DbHelper.SetupStudent(_client, studentRequest);

            _client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue(
                "Bearer",
                TestAuthorization.GetToken("User", "User", new Guid())
            );

            var response = await _client.GetAsync($"/api/Student/{studentId}");

            Assert.Equal(HttpStatusCode.NotFound, response.StatusCode);
        }

        [Fact]
        public async Task StudentControllerThrowsUnauthorized()
        {
            _client.DefaultRequestHeaders.Authorization = null;

            var response = await _client.DeleteAsync($"/api/Student/{new Guid()}");
            Assert.Equal(HttpStatusCode.Unauthorized, response.StatusCode);

            response = await _client.PostAsJsonAsync(
                "/api/Student",
                MockData.GetCreateStudentDTODetails(groupId1)
            );
            Assert.Equal(HttpStatusCode.Unauthorized, response.StatusCode);

            var query = $"?Id={new Guid()}";
            response = await _client.GetAsync($"/api/Student{query}");
            Assert.Equal(HttpStatusCode.Unauthorized, response.StatusCode);

            response = await _client.PatchAsJsonAsync(
                $"/api/Student/{new Guid()}",
                MockData.GetUpdateStudentDTODetails()
            );
            Assert.Equal(HttpStatusCode.Unauthorized, response.StatusCode);

            response = await _client.GetAsync($"/api/Student/{new Guid()}");
            Assert.Equal(HttpStatusCode.Unauthorized, response.StatusCode);
        }

        [Fact]
        public async Task StudentControllerThrowsTooManyRequests()
        {
            for (int i = 0; i != RateLimiterSettings.permitLimit; i++)
            {
                await _client.GetAsync("/api/Student");
            }

            _client.DefaultRequestHeaders.Authorization = null;

            var response = await _client.DeleteAsync($"/api/Student/{new Guid()}");
            Assert.Equal(HttpStatusCode.TooManyRequests, response.StatusCode);

            response = await _client.PostAsJsonAsync(
                "/api/Student",
                MockData.GetCreateStudentDTODetails(groupId1)
            );
            Assert.Equal(HttpStatusCode.TooManyRequests, response.StatusCode);

            var query = $"?Id={new Guid()}";
            response = await _client.GetAsync($"/api/Student{query}");
            Assert.Equal(HttpStatusCode.TooManyRequests, response.StatusCode);

            response = await _client.PatchAsJsonAsync(
                $"/api/Student/{new Guid()}",
                MockData.GetUpdateStudentDTODetails()
            );
            Assert.Equal(HttpStatusCode.TooManyRequests, response.StatusCode);

            response = await _client.GetAsync($"/api/Student/{new Guid()}");
            Assert.Equal(HttpStatusCode.TooManyRequests, response.StatusCode);
        }
    }
}
