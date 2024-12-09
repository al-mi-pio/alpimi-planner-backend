using System.Net;
using System.Net.Http.Headers;
using AlpimiAPI.Entities.EClassroom;
using AlpimiAPI.Responses;
using AlpimiAPI.Settings;
using AlpimiTest.TestSetup;
using AlpimiTest.TestUtilities;
using Xunit;

namespace AlpimiTest.Entities.EClassroom
{
    [Collection("Sequential Tests")]
    public class ClassroomControllerTest : IAsyncLifetime
    {
        CustomWebApplicationFactory<Program> _factory;
        HttpClient _client;
        Guid userId;
        Guid scheduleId;

        public ClassroomControllerTest()
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
        public async Task ClassroomIsDeleted()
        {
            var classroomRequest = MockData.GetCreateClassroomDTODetails(scheduleId);
            var classroomId = await DbHelper.SetupClassroom(_client, classroomRequest);

            _client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue(
                "Bearer",
                TestAuthorization.GetToken("Admin", "User", new Guid())
            );

            var response = await _client.DeleteAsync($"/api/Classroom/{classroomId}");
            Assert.Equal(HttpStatusCode.NoContent, response.StatusCode);

            var query = $"?id={scheduleId}";
            response = await _client.GetAsync($"/api/Classroom");
            var stringResponse = await response.Content.ReadAsStringAsync();
            Assert.DoesNotContain(classroomRequest.Name, stringResponse);
        }

        [Fact]
        public async Task ClassroomIsCreated()
        {
            var classroomRequest = MockData.GetCreateClassroomDTODetails(scheduleId);

            _client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue(
                "Bearer",
                TestAuthorization.GetToken("Admin", "User", userId)
            );

            var response = await _client.PostAsJsonAsync("/api/Classroom", classroomRequest);
            Assert.Equal(HttpStatusCode.OK, response.StatusCode);

            var query = $"?id={scheduleId}";
            response = await _client.GetAsync($"/api/Classroom{query}");
            var stringResponse = await response.Content.ReadAsStringAsync();
            Assert.Contains(classroomRequest.Name, stringResponse);
        }

        [Fact]
        public async Task ClassroomIsCreatedWithClassroomTypes()
        {
            _client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue(
                "Bearer",
                TestAuthorization.GetToken("Admin", "User", userId)
            );

            var classroomTypeId = await DbHelper.SetupClassroomType(
                _client,
                MockData.GetCreateClassroomTypeDTODetails(scheduleId)
            );
            var classroomRequest = MockData.GetCreateClassroomDTODetails(scheduleId);
            classroomRequest.ClassroomTypeIds = [classroomTypeId];

            var response = await _client.PostAsJsonAsync("/api/Classroom", classroomRequest);
            Assert.Equal(HttpStatusCode.OK, response.StatusCode);

            var query = $"?id={classroomTypeId}";
            response = await _client.GetAsync($"/api/Classroom{query}");
            var stringResponse = await response.Content.ReadAsStringAsync();
            Assert.Contains(classroomRequest.Name, stringResponse);
        }

        [Fact]
        public async Task UpdateClassroomReturnsUpdatedClassroom()
        {
            var classroomUpdateRequest = MockData.GetUpdateClassroomDTODetails();
            var classroomId = await DbHelper.SetupClassroom(
                _client,
                MockData.GetCreateClassroomDTODetails(scheduleId)
            );

            _client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue(
                "Bearer",
                TestAuthorization.GetToken("Admin", "Bob", userId)
            );

            var response = await _client.PatchAsJsonAsync(
                $"/api/Classroom/{classroomId}",
                classroomUpdateRequest
            );
            var jsonResponse = await response.Content.ReadFromJsonAsync<
                ApiGetResponse<Classroom>
            >();

            Assert.Equal(classroomUpdateRequest.Name, jsonResponse!.Content.Name);
        }

        [Fact]
        public async Task UpdateClassroomUpdatesClassroomsClassroomTypes()
        {
            _client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue(
                "Bearer",
                TestAuthorization.GetToken("Admin", "Bob", userId)
            );
            var classroomUpdateRequest = MockData.GetUpdateClassroomDTODetails();
            var classroomId = await DbHelper.SetupClassroom(
                _client,
                MockData.GetCreateClassroomDTODetails(scheduleId)
            );

            var classroomTypeId = await DbHelper.SetupClassroomType(
                _client,
                MockData.GetCreateClassroomTypeDTODetails(scheduleId)
            );
            classroomUpdateRequest.ClassroomTypeIds = [classroomTypeId];

            await _client.PatchAsJsonAsync($"/api/Classroom/{classroomId}", classroomUpdateRequest);

            var query = $"?id={classroomTypeId}";
            var response = await _client.GetAsync($"/api/Classroom{query}");
            var stringResponse = await response.Content.ReadAsStringAsync();
            Assert.Contains(classroomUpdateRequest.Name!, stringResponse);
        }

        [Fact]
        public async Task UpdateClassroomThrowsNotFoundErrorWhenWrongIdIsGiven()
        {
            var classroomUpdateRequest = MockData.GetUpdateClassroomDTODetails();

            var classroomId = await DbHelper.SetupClassroom(
                _client,
                MockData.GetCreateClassroomDTODetails(scheduleId)
            );

            _client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue(
                "Bearer",
                TestAuthorization.GetToken("Admin", "User", userId)
            );
            var response = await _client.PatchAsJsonAsync(
                $"/api/Classroom/{new Guid()}",
                classroomUpdateRequest
            );

            Assert.Equal(HttpStatusCode.NotFound, response.StatusCode);
        }

        [Fact]
        public async Task UpdateClassroomThrowsNotFoundErrorWhenWrongUserAttemptsUpdate()
        {
            var classroomUpdateRequest = MockData.GetUpdateClassroomDTODetails();

            var classroomId = await DbHelper.SetupClassroom(
                _client,
                MockData.GetCreateClassroomDTODetails(scheduleId)
            );

            _client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue(
                "Bearer",
                TestAuthorization.GetToken("User", "User", new Guid())
            );
            var response = await _client.PatchAsJsonAsync(
                $"/api/Classroom/{classroomId}",
                classroomUpdateRequest
            );

            Assert.Equal(HttpStatusCode.NotFound, response.StatusCode);
        }

        [Fact]
        public async Task GetAllClassroomsReturnsClassroomsFromScheduleIfShceduleIdIsProvided()
        {
            var classroomRequest1 = MockData.GetCreateClassroomDTODetails(scheduleId);
            var classroomRequest2 = MockData.GetCreateSecondClassroomDTODetails(scheduleId);

            await DbHelper.SetupClassroom(_client, classroomRequest1);
            await DbHelper.SetupClassroom(_client, classroomRequest2);

            _client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue(
                "Bearer",
                TestAuthorization.GetToken("Admin", "User", userId)
            );
            var query = $"?id={scheduleId}";
            var response = await _client.GetAsync($"/api/Classroom{query}");
            var stringResponse = await response.Content.ReadAsStringAsync();

            Assert.Contains(classroomRequest1.Name, stringResponse);
            Assert.Contains(classroomRequest2.Name, stringResponse);
        }

        [Fact]
        public async Task GetAllClassroomsReturnsClassroomsFromClassroomTypeIfClassroomTypeIdIsProvided()
        {
            var classromTypeId = await DbHelper.SetupClassroomType(
                _client,
                MockData.GetCreateClassroomTypeDTODetails(scheduleId)
            );

            var classroomRequest1 = MockData.GetCreateClassroomDTODetails(scheduleId);
            classroomRequest1.ClassroomTypeIds = [classromTypeId];
            var classroomRequest2 = MockData.GetCreateSecondClassroomDTODetails(scheduleId);

            await DbHelper.SetupClassroom(_client, classroomRequest1);
            await DbHelper.SetupClassroom(_client, classroomRequest2);

            _client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue(
                "Bearer",
                TestAuthorization.GetToken("Admin", "User", userId)
            );
            var query = $"?id={classromTypeId}";
            var response = await _client.GetAsync($"/api/Classroom{query}");
            var stringResponse = await response.Content.ReadAsStringAsync();

            Assert.Contains(classroomRequest1.Name, stringResponse);
            Assert.DoesNotContain(classroomRequest2.Name, stringResponse);
        }

        [Fact]
        public async Task GetAllClassroomReturnsEmptyContentWhenWrongUserAttemptsGet()
        {
            var classroomRequest1 = MockData.GetCreateClassroomDTODetails(scheduleId);
            var classroomRequest2 = MockData.GetCreateSecondClassroomDTODetails(scheduleId);

            await DbHelper.SetupClassroom(_client, classroomRequest1);
            await DbHelper.SetupClassroom(_client, classroomRequest2);

            _client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue(
                "Bearer",
                TestAuthorization.GetToken("User", "User", new Guid())
            );
            var query = $"?id={scheduleId}";
            var response = await _client.GetAsync($"/api/Classroom{query}");
            var stringResponse = await response.Content.ReadAsStringAsync();

            Assert.DoesNotContain(classroomRequest1.Name, stringResponse);
            Assert.DoesNotContain(classroomRequest2.Name, stringResponse);
        }

        [Fact]
        public async Task GetAllClassroomReturnsEmptyContentWhenWrongIdIsGiven()
        {
            var classroomRequest1 = MockData.GetCreateClassroomDTODetails(scheduleId);
            var classroomRequest2 = MockData.GetCreateSecondClassroomDTODetails(scheduleId);

            await DbHelper.SetupClassroom(_client, classroomRequest1);
            await DbHelper.SetupClassroom(_client, classroomRequest2);

            _client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue(
                "Bearer",
                TestAuthorization.GetToken("Admin", "User", userId)
            );
            var query = $"?id={new Guid()}";
            var response = await _client.GetAsync($"/api/Classroom{query}");
            var stringResponse = await response.Content.ReadAsStringAsync();

            Assert.DoesNotContain(classroomRequest1.Name, stringResponse);
            Assert.DoesNotContain(classroomRequest2.Name, stringResponse);
        }

        [Fact]
        public async Task GetClassroomReturnsClassroom()
        {
            var classroomRequest = MockData.GetCreateClassroomDTODetails(scheduleId);

            var classroomId = await DbHelper.SetupClassroom(_client, classroomRequest);

            _client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue(
                "Bearer",
                TestAuthorization.GetToken("Admin", "User", userId)
            );

            var response = await _client.GetAsync($"/api/Classroom/{classroomId}");
            var jsonResponse = await response.Content.ReadFromJsonAsync<
                ApiGetResponse<Classroom>
            >();

            Assert.Equal(classroomRequest.Name, jsonResponse!.Content.Name);
        }

        [Fact]
        public async Task GetScheduleThrowsNotFoundWhenWrongIdIsGiven()
        {
            var classroomRequest = MockData.GetCreateClassroomDTODetails(scheduleId);

            await DbHelper.SetupClassroom(_client, classroomRequest);

            _client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue(
                "Bearer",
                TestAuthorization.GetToken("Admin", "User", userId)
            );

            var response = await _client.GetAsync($"/api/Classroom/{new Guid()}");

            Assert.Equal(HttpStatusCode.NotFound, response.StatusCode);
        }

        [Fact]
        public async Task GetScheduleThrowsNotFoundErrorWhenWrongUserTokenIsGiven()
        {
            var classroomRequest = MockData.GetCreateClassroomDTODetails(scheduleId);

            var classroomId = await DbHelper.SetupClassroom(_client, classroomRequest);

            _client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue(
                "Bearer",
                TestAuthorization.GetToken("User", "User", new Guid())
            );

            var response = await _client.GetAsync($"/api/Classroom/{classroomId}");

            Assert.Equal(HttpStatusCode.NotFound, response.StatusCode);
        }

        [Fact]
        public async Task ClassroomControllerThrowsUnauthorized()
        {
            _client.DefaultRequestHeaders.Authorization = null;

            var response = await _client.DeleteAsync($"/api/Classroom/{new Guid()}");
            Assert.Equal(HttpStatusCode.Unauthorized, response.StatusCode);

            response = await _client.PostAsJsonAsync(
                "/api/Classroom",
                MockData.GetCreateClassroomDTODetails(scheduleId)
            );
            Assert.Equal(HttpStatusCode.Unauthorized, response.StatusCode);

            var query = $"?id={new Guid()}";
            response = await _client.GetAsync($"/api/Classroom{query}");
            Assert.Equal(HttpStatusCode.Unauthorized, response.StatusCode);

            response = await _client.PatchAsJsonAsync(
                $"/api/Classroom/{new Guid()}",
                MockData.GetUpdateClassroomDTODetails()
            );
            Assert.Equal(HttpStatusCode.Unauthorized, response.StatusCode);

            response = await _client.GetAsync($"/api/Classroom/{new Guid()}");
            Assert.Equal(HttpStatusCode.Unauthorized, response.StatusCode);
        }

        [Fact]
        public async Task ClassroomControllerThrowsTooManyRequests()
        {
            for (int i = 0; i != RateLimiterSettings.permitLimit; i++)
            {
                await _client.GetAsync("/api/Classroom");
            }

            _client.DefaultRequestHeaders.Authorization = null;

            var response = await _client.DeleteAsync($"/api/Classroom/{new Guid()}");
            Assert.Equal(HttpStatusCode.TooManyRequests, response.StatusCode);

            response = await _client.PostAsJsonAsync(
                "/api/Classroom",
                MockData.GetCreateClassroomDTODetails(scheduleId)
            );
            Assert.Equal(HttpStatusCode.TooManyRequests, response.StatusCode);

            var query = $"?id={new Guid()}";
            response = await _client.GetAsync($"/api/Classroom{query}");
            Assert.Equal(HttpStatusCode.TooManyRequests, response.StatusCode);

            response = await _client.PatchAsJsonAsync(
                $"/api/Classroom/{new Guid()}",
                MockData.GetUpdateClassroomDTODetails()
            );
            Assert.Equal(HttpStatusCode.TooManyRequests, response.StatusCode);

            response = await _client.GetAsync($"/api/Classroom/{new Guid()}");
            Assert.Equal(HttpStatusCode.TooManyRequests, response.StatusCode);
        }
    }
}
