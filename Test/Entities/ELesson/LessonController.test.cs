using System.Net;
using System.Net.Http.Headers;
using AlpimiAPI.Entities.ELesson;
using AlpimiAPI.Entities.ELesson.DTO;
using AlpimiAPI.Entities.ELessonType;
using AlpimiAPI.Locales;
using AlpimiAPI.Responses;
using AlpimiAPI.Settings;
using AlpimiTest.TestSetup;
using AlpimiTest.TestUtilities;
using Azure;
using Microsoft.VisualStudio.TestPlatform.CommunicationUtilities;
using Xunit;

namespace AlpimiTest.Entities.ELesson
{
    [Collection("Sequential Tests")]
    public class LessonControllerTest : IAsyncLifetime
    {
        CustomWebApplicationFactory<Program> _factory;
        HttpClient _client;
        Guid userId;
        Guid scheduleId;
        Guid groupId;
        Guid subgroupId1;
        Guid subgroupId2;
        Guid lessonTypeId;

        public LessonControllerTest()
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
            groupId = await DbHelper.SetupGroup(
                _client,
                MockData.GetCreateGroupDTODetails(scheduleId)
            );
            subgroupId1 = await DbHelper.SetupSubgroup(
                _client,
                MockData.GetCreateSubgroupDTODetails(groupId)
            );
            subgroupId2 = await DbHelper.SetupSubgroup(
                _client,
                MockData.GetCreateSecondSubgroupDTODetails(groupId)
            );
            lessonTypeId = await DbHelper.SetupLessonType(
                _client,
                MockData.GetCreateLessonTypeDTODetails(scheduleId)
            );
        }

        public async Task DisposeAsync()
        {
            await DbHelper.UserCleaner(_client);
        }

        [Fact]
        public async Task LessonIsDeleted()
        {
            var lessonRequest = MockData.GetCreateLessonDTODetails(subgroupId1, lessonTypeId);
            var lessonId = await DbHelper.SetupLesson(_client, lessonRequest);

            _client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue(
                "Bearer",
                TestAuthorization.GetToken("Admin", "User", new Guid())
            );

            var response = await _client.DeleteAsync($"/api/Lesson/{lessonId}");
            Assert.Equal(HttpStatusCode.NoContent, response.StatusCode);

            var query = $"?id={groupId}";
            response = await _client.GetAsync($"/api/Lesson");
            var stringResponse = await response.Content.ReadAsStringAsync();
            Assert.DoesNotContain(lessonRequest.Name, stringResponse);
        }

        [Fact]
        public async Task LessonIsCreated()
        {
            var lessonRequest = MockData.GetCreateLessonDTODetails(subgroupId1, lessonTypeId);

            _client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue(
                "Bearer",
                TestAuthorization.GetToken("Admin", "User", userId)
            );

            var response = await _client.PostAsJsonAsync("/api/Lesson", lessonRequest);
            Assert.Equal(HttpStatusCode.OK, response.StatusCode);

            var query = $"?id={groupId}";
            response = await _client.GetAsync($"/api/Lesson{query}");
            var stringResponse = await response.Content.ReadAsStringAsync();
            Assert.Contains(lessonRequest.Name, stringResponse);
        }

        [Fact]
        public async Task LessonIsCreatedWithClassroomTypes()
        {
            var lessonRequest = MockData.GetCreateLessonDTODetails(subgroupId1, lessonTypeId);

            _client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue(
                "Bearer",
                TestAuthorization.GetToken("Admin", "User", userId)
            );

            var classroomTypeRequest = MockData.GetCreateClassroomTypeDTODetails(scheduleId);
            var classroomTypeId = await DbHelper.SetupClassroomType(_client, classroomTypeRequest);
            lessonRequest.ClassroomTypeIds = [classroomTypeId];

            var response = await _client.PostAsJsonAsync("/api/Lesson", lessonRequest);
            Assert.Equal(HttpStatusCode.OK, response.StatusCode);

            var jsonLessonId = await response.Content.ReadFromJsonAsync<ApiGetResponse<Guid>>();

            var query = $"?id={jsonLessonId!.Content}";
            response = await _client.GetAsync($"/api/ClassroomType{query}");
            var stringResponse = await response.Content.ReadAsStringAsync();

            Assert.Contains(classroomTypeRequest.Name, stringResponse);
        }

        [Fact]
        public async Task UpdateLessonReturnsUpdatedLesson()
        {
            var lessonUpdateRequest = MockData.GetUpdateLessonDTODetails();

            _client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue(
                "Bearer",
                TestAuthorization.GetToken("Admin", "Bob", userId)
            );

            var lessonId = await DbHelper.SetupLesson(
                _client,
                MockData.GetCreateLessonDTODetails(subgroupId1, lessonTypeId)
            );

            var response = await _client.PatchAsJsonAsync(
                $"/api/Lesson/{lessonId}",
                lessonUpdateRequest
            );
            var jsonResponse = await response.Content.ReadFromJsonAsync<
                ApiGetResponse<LessonDTO>
            >();

            Assert.Equal(lessonUpdateRequest.Name, jsonResponse!.Content.Name);
            Assert.Equal(lessonUpdateRequest.AmountOfHours, jsonResponse!.Content.AmountOfHours);
        }

        [Fact]
        public async Task UpdateLessonUpdatesClassroomsClassroomTypes()
        {
            var lessonUpdateRequest = MockData.GetUpdateLessonDTODetails();
            var lessonId = await DbHelper.SetupLesson(
                _client,
                MockData.GetCreateLessonDTODetails(subgroupId1, lessonTypeId)
            );

            _client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue(
                "Bearer",
                TestAuthorization.GetToken("Admin", "Bob", userId)
            );

            var classromTypeRequest = MockData.GetCreateClassroomTypeDTODetails(scheduleId);
            var classroomTypeId = await DbHelper.SetupClassroomType(_client, classromTypeRequest);
            lessonUpdateRequest.ClassroomTypeIds = [classroomTypeId];

            await _client.PatchAsJsonAsync($"/api/Lesson/{lessonId}", lessonUpdateRequest);

            var query = $"?id={lessonId}";
            var response = await _client.GetAsync($"/api/ClassroomType{query}");
            var stringResponse = await response.Content.ReadAsStringAsync();
            Assert.Contains(classromTypeRequest.Name!, stringResponse);
        }

        [Fact]
        public async Task UpdateLessonThrowsNotFoundErrorWhenWrongIdIsGiven()
        {
            var lessonUpdateRequest = MockData.GetUpdateLessonDTODetails();

            var lessonId = await DbHelper.SetupLesson(
                _client,
                MockData.GetCreateLessonDTODetails(subgroupId1, lessonTypeId)
            );

            _client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue(
                "Bearer",
                TestAuthorization.GetToken("Admin", "User", userId)
            );
            var response = await _client.PatchAsJsonAsync(
                $"/api/Lesson/{new Guid()}",
                lessonUpdateRequest
            );

            Assert.Equal(HttpStatusCode.NotFound, response.StatusCode);
        }

        [Fact]
        public async Task UpdateLessonThrowsNotFoundErrorWhenWrongUserAttemptsUpdate()
        {
            var lessonUpdateRequest = MockData.GetUpdateLessonDTODetails();

            var lessonId = await DbHelper.SetupLesson(
                _client,
                MockData.GetCreateLessonDTODetails(subgroupId1, lessonTypeId)
            );

            _client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue(
                "Bearer",
                TestAuthorization.GetToken("User", "User", new Guid())
            );
            var response = await _client.PatchAsJsonAsync(
                $"/api/Lesson/{lessonId}",
                lessonUpdateRequest
            );

            Assert.Equal(HttpStatusCode.NotFound, response.StatusCode);
        }

        [Fact]
        public async Task GetAllLessonsReturnsLessonsFromGroupIfGroupIdIsProvided()
        {
            var lessonRequest1 = MockData.GetCreateLessonDTODetails(subgroupId1, lessonTypeId);
            var lessonRequest2 = MockData.GetCreateSecondLessonDTODetails(
                subgroupId2,
                lessonTypeId
            );

            await DbHelper.SetupLesson(_client, lessonRequest1);
            await DbHelper.SetupLesson(_client, lessonRequest2);

            _client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue(
                "Bearer",
                TestAuthorization.GetToken("Admin", "User", userId)
            );
            var query = $"?id={groupId}";
            var response = await _client.GetAsync($"/api/Lesson{query}");
            var stringResponse = await response.Content.ReadAsStringAsync();

            Assert.Contains(lessonRequest1.Name, stringResponse);
            Assert.Contains(lessonRequest2.Name, stringResponse);
        }

        [Fact]
        public async Task GetAllLessonsReturnsLessonsFromSubgroupIfSubgroupIdIsProvided()
        {
            var lessonRequest1 = MockData.GetCreateLessonDTODetails(subgroupId1, lessonTypeId);
            var lessonRequest2 = MockData.GetCreateSecondLessonDTODetails(
                subgroupId2,
                lessonTypeId
            );

            var lessonId = await DbHelper.SetupLesson(_client, lessonRequest1);
            await DbHelper.SetupLesson(_client, lessonRequest2);

            _client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue(
                "Bearer",
                TestAuthorization.GetToken("Admin", "User", userId)
            );
            var query = $"?id={subgroupId1}";
            var response = await _client.GetAsync($"/api/Lesson{query}");
            var stringResponse = await response.Content.ReadAsStringAsync();

            Assert.Contains(lessonRequest1.Name, stringResponse);
            Assert.DoesNotContain(lessonRequest2.Name, stringResponse);
        }

        [Fact]
        public async Task GetAllLessonsReturnsEmptyContentWhenWrongUserAttemptsGet()
        {
            var lessonRequest1 = MockData.GetCreateLessonDTODetails(subgroupId1, lessonTypeId);
            var lessonRequest2 = MockData.GetCreateSecondLessonDTODetails(
                subgroupId2,
                lessonTypeId
            );

            await DbHelper.SetupLesson(_client, lessonRequest1);
            await DbHelper.SetupLesson(_client, lessonRequest2);

            _client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue(
                "Bearer",
                TestAuthorization.GetToken("User", "User", new Guid())
            );
            var query = $"?groupId={groupId}";
            var response = await _client.GetAsync($"/api/Lesson{query}");
            var stringResponse = await response.Content.ReadAsStringAsync();

            Assert.DoesNotContain(lessonRequest1.Name, stringResponse);
            Assert.DoesNotContain(lessonRequest2.Name, stringResponse);
        }

        [Fact]
        public async Task GetAllLessonsReturnsEmptyContentWhenWrongIdIsGiven()
        {
            var lessonRequest1 = MockData.GetCreateLessonDTODetails(subgroupId1, lessonTypeId);
            var lessonRequest2 = MockData.GetCreateSecondLessonDTODetails(
                subgroupId2,
                lessonTypeId
            );

            await DbHelper.SetupLesson(_client, lessonRequest1);
            await DbHelper.SetupLesson(_client, lessonRequest2);

            _client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue(
                "Bearer",
                TestAuthorization.GetToken("Admin", "User", userId)
            );
            var query = $"?groupId={new Guid()}";
            var response = await _client.GetAsync($"/api/Lesson{query}");
            var stringResponse = await response.Content.ReadAsStringAsync();

            Assert.DoesNotContain(lessonRequest1.Name, stringResponse);
            Assert.DoesNotContain(lessonRequest2.Name, stringResponse);
        }

        [Fact]
        public async Task GetLessonReturnsLesson()
        {
            var lessonRequest = MockData.GetCreateLessonDTODetails(subgroupId1, lessonTypeId);

            var lessonId = await DbHelper.SetupLesson(_client, lessonRequest);

            _client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue(
                "Bearer",
                TestAuthorization.GetToken("Admin", "User", userId)
            );

            var response = await _client.GetAsync($"/api/Lesson/{lessonId}");
            var jsonResponse = await response.Content.ReadFromJsonAsync<
                ApiGetResponse<LessonDTO>
            >();

            Assert.Equal(lessonRequest.Name, jsonResponse!.Content.Name);
            Assert.Equal(lessonRequest.AmountOfHours, jsonResponse!.Content.AmountOfHours);
        }

        [Fact]
        public async Task GetLessonThrowsNotFoundWhenWrongIdIsGiven()
        {
            var lessonRequest = MockData.GetCreateLessonDTODetails(subgroupId1, lessonTypeId);

            await DbHelper.SetupLesson(_client, lessonRequest);

            _client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue(
                "Bearer",
                TestAuthorization.GetToken("Admin", "User", userId)
            );

            var response = await _client.GetAsync($"/api/Lesson/{new Guid()}");

            Assert.Equal(HttpStatusCode.NotFound, response.StatusCode);
        }

        [Fact]
        public async Task GetLessonThrowsNotFoundErrorWhenWrongUserTokenIsGiven()
        {
            var lessonRequest = MockData.GetCreateLessonDTODetails(subgroupId1, lessonTypeId);

            var lessonId = await DbHelper.SetupLesson(_client, lessonRequest);

            _client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue(
                "Bearer",
                TestAuthorization.GetToken("User", "User", new Guid())
            );

            var response = await _client.GetAsync($"/api/Lesson/{lessonId}");

            Assert.Equal(HttpStatusCode.NotFound, response.StatusCode);
        }

        [Fact]
        public async Task LessonControllerThrowsUnauthorized()
        {
            _client.DefaultRequestHeaders.Authorization = null;

            var response = await _client.DeleteAsync($"/api/Lesson/{new Guid()}");
            Assert.Equal(HttpStatusCode.Unauthorized, response.StatusCode);

            response = await _client.PostAsJsonAsync(
                "/api/Lesson",
                MockData.GetCreateLessonDTODetails(subgroupId1, lessonTypeId)
            );
            Assert.Equal(HttpStatusCode.Unauthorized, response.StatusCode);

            var query = $"?groupId={new Guid()}";
            response = await _client.GetAsync($"/api/Lesson{query}");
            Assert.Equal(HttpStatusCode.Unauthorized, response.StatusCode);

            response = await _client.PatchAsJsonAsync(
                $"/api/Lesson/{new Guid()}",
                MockData.GetUpdateLessonDTODetails()
            );
            Assert.Equal(HttpStatusCode.Unauthorized, response.StatusCode);

            response = await _client.GetAsync($"/api/Lesson/{new Guid()}");
            Assert.Equal(HttpStatusCode.Unauthorized, response.StatusCode);
        }

        [Fact]
        public async Task LessonControllerThrowsTooManyRequests()
        {
            for (int i = 0; i != RateLimiterSettings.permitLimit; i++)
            {
                await _client.GetAsync("/api/Lesson");
            }

            _client.DefaultRequestHeaders.Authorization = null;

            var response = await _client.DeleteAsync($"/api/Lesson/{new Guid()}");
            Assert.Equal(HttpStatusCode.TooManyRequests, response.StatusCode);

            response = await _client.PostAsJsonAsync(
                "/api/Lesson",
                MockData.GetCreateLessonDTODetails(subgroupId1, lessonTypeId)
            );
            Assert.Equal(HttpStatusCode.TooManyRequests, response.StatusCode);

            var query = $"?groupId={new Guid()}";
            response = await _client.GetAsync($"/api/Lesson{query}");
            Assert.Equal(HttpStatusCode.TooManyRequests, response.StatusCode);

            response = await _client.PatchAsJsonAsync(
                $"/api/Lesson/{new Guid()}",
                MockData.GetUpdateLessonDTODetails()
            );
            Assert.Equal(HttpStatusCode.TooManyRequests, response.StatusCode);

            response = await _client.GetAsync($"/api/Lesson/{new Guid()}");
            Assert.Equal(HttpStatusCode.TooManyRequests, response.StatusCode);
        }
    }
}
