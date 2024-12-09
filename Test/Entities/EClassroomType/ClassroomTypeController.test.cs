using System.Net;
using System.Net.Http.Headers;
using AlpimiAPI.Entities.EClassroomType;
using AlpimiAPI.Responses;
using AlpimiAPI.Settings;
using AlpimiTest.TestSetup;
using AlpimiTest.TestUtilities;
using Xunit;

namespace AlpimiTest.Entities.EClassroomType
{
    [Collection("Sequential Tests")]
    public class ClassroomTypeControllerTest : IAsyncLifetime
    {
        CustomWebApplicationFactory<Program> _factory;
        HttpClient _client;
        Guid userId;
        Guid scheduleId;

        public ClassroomTypeControllerTest()
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
        public async Task ClassroomTypeIsDeleted()
        {
            var classroomTypeRequest = MockData.GetCreateClassroomTypeDTODetails(scheduleId);
            var classroomTypeId = await DbHelper.SetupClassroomType(_client, classroomTypeRequest);

            _client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue(
                "Bearer",
                TestAuthorization.GetToken("Admin", "User", new Guid())
            );

            var response = await _client.DeleteAsync($"/api/ClassroomType/{classroomTypeId}");
            Assert.Equal(HttpStatusCode.NoContent, response.StatusCode);

            var query = $"id={scheduleId}";
            response = await _client.GetAsync($"/api/ClassroomType");
            var stringResponse = await response.Content.ReadAsStringAsync();
            Assert.DoesNotContain(classroomTypeRequest.Name, stringResponse);
        }

        [Fact]
        public async Task ClassroomTypeIsCreated()
        {
            var classroomTypeRequest = MockData.GetCreateClassroomTypeDTODetails(scheduleId);

            _client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue(
                "Bearer",
                TestAuthorization.GetToken("Admin", "User", userId)
            );

            var response = await _client.PostAsJsonAsync(
                "/api/ClassroomType",
                classroomTypeRequest
            );
            Assert.Equal(HttpStatusCode.OK, response.StatusCode);

            var query = $"?id={scheduleId}";
            response = await _client.GetAsync($"/api/ClassroomType{query}");
            var stringResponse = await response.Content.ReadAsStringAsync();
            Assert.Contains(classroomTypeRequest.Name, stringResponse);
        }

        [Fact]
        public async Task UpdateClassroomTypeReturnsUpdatedClassroomType()
        {
            var classroomTypeUpdateRequest = MockData.GetUpdateClassroomTypeDTODetails();
            var classroomTypeId = await DbHelper.SetupClassroomType(
                _client,
                MockData.GetCreateClassroomTypeDTODetails(scheduleId)
            );

            _client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue(
                "Bearer",
                TestAuthorization.GetToken("Admin", "Bob", userId)
            );

            var response = await _client.PatchAsJsonAsync(
                $"/api/ClassroomType/{classroomTypeId}",
                classroomTypeUpdateRequest
            );
            var jsonResponse = await response.Content.ReadFromJsonAsync<
                ApiGetResponse<ClassroomType>
            >();

            Assert.Equal(classroomTypeUpdateRequest.Name, jsonResponse!.Content.Name);
        }

        [Fact]
        public async Task UpdateClassroomTypeThrowsNotFoundErrorWhenWrongIdIsGiven()
        {
            var classroomTypeUpdateRequest = MockData.GetUpdateClassroomTypeDTODetails();

            var classroomTypeId = await DbHelper.SetupClassroomType(
                _client,
                MockData.GetCreateClassroomTypeDTODetails(scheduleId)
            );

            _client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue(
                "Bearer",
                TestAuthorization.GetToken("Admin", "User", userId)
            );
            var response = await _client.PatchAsJsonAsync(
                $"/api/ClassroomType/{new Guid()}",
                classroomTypeUpdateRequest
            );

            Assert.Equal(HttpStatusCode.NotFound, response.StatusCode);
        }

        [Fact]
        public async Task UpdateClassroomTypeThrowsNotFoundErrorWhenWrongUserAttemptsUpdate()
        {
            var classroomTypeUpdateRequest = MockData.GetUpdateClassroomTypeDTODetails();

            var classroomTypeId = await DbHelper.SetupClassroomType(
                _client,
                MockData.GetCreateClassroomTypeDTODetails(scheduleId)
            );

            _client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue(
                "Bearer",
                TestAuthorization.GetToken("User", "User", new Guid())
            );
            var response = await _client.PatchAsJsonAsync(
                $"/api/ClassroomType/{classroomTypeId}",
                classroomTypeUpdateRequest
            );

            Assert.Equal(HttpStatusCode.NotFound, response.StatusCode);
        }

        [Fact]
        public async Task GetAllClassroomTypesReturnsClassroomTypesFromScheduleIfScheduleIdIsProvided()
        {
            var classroomTypeRequest1 = MockData.GetCreateClassroomTypeDTODetails(scheduleId);
            var classroomTypeRequest2 = MockData.GetCreateSecondClassroomTypeDTODetails(scheduleId);
            _client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue(
                "Bearer",
                TestAuthorization.GetToken("Admin", "User", userId)
            );

            await DbHelper.SetupClassroomType(_client, classroomTypeRequest1);
            await DbHelper.SetupClassroomType(_client, classroomTypeRequest2);

            var query = $"?id={scheduleId}";
            var response = await _client.GetAsync($"/api/ClassroomType{query}");
            var stringResponse = await response.Content.ReadAsStringAsync();

            Assert.Contains(classroomTypeRequest1.Name, stringResponse);
            Assert.Contains(classroomTypeRequest2.Name, stringResponse);
        }

        [Fact]
        public async Task GetAllClassroomTypesReturnsClassroomTypesFromClassroomIfClassroomIdIsProvided()
        {
            var classroomTypeRequest1 = MockData.GetCreateClassroomTypeDTODetails(scheduleId);
            var classroomTypeRequest2 = MockData.GetCreateSecondClassroomTypeDTODetails(scheduleId);
            _client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue(
                "Bearer",
                TestAuthorization.GetToken("Admin", "User", userId)
            );

            var classroomTypeId = await DbHelper.SetupClassroomType(_client, classroomTypeRequest1);
            await DbHelper.SetupClassroomType(_client, classroomTypeRequest2);

            var classroomRequest = MockData.GetCreateClassroomDTODetails(scheduleId);
            classroomRequest.ClassroomTypeIds = [classroomTypeId];

            var classroomId = await DbHelper.SetupClassroom(_client, classroomRequest);
            await DbHelper.SetupClassroom(
                _client,
                MockData.GetCreateClassroomDTODetails(scheduleId)
            );

            var query = $"?id={classroomId}";
            var response = await _client.GetAsync($"/api/ClassroomType{query}");
            var stringResponse = await response.Content.ReadAsStringAsync();

            Assert.Contains(classroomTypeRequest1.Name, stringResponse);
            Assert.DoesNotContain(classroomTypeRequest2.Name, stringResponse);
        }

        [Fact]
        public async Task GetAllClassroomTypeReturnsEmptyContentWhenWrongUserAttemptsGet()
        {
            var classroomTypeRequest1 = MockData.GetCreateClassroomTypeDTODetails(scheduleId);
            var classroomTypeRequest2 = MockData.GetCreateSecondClassroomTypeDTODetails(scheduleId);

            await DbHelper.SetupClassroomType(_client, classroomTypeRequest1);
            await DbHelper.SetupClassroomType(_client, classroomTypeRequest2);

            _client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue(
                "Bearer",
                TestAuthorization.GetToken("User", "User", new Guid())
            );
            var query = $"?id={scheduleId}";
            var response = await _client.GetAsync($"/api/ClassroomType{query}");
            var stringResponse = await response.Content.ReadAsStringAsync();

            Assert.DoesNotContain(classroomTypeRequest1.Name, stringResponse);
            Assert.DoesNotContain(classroomTypeRequest2.Name, stringResponse);
        }

        [Fact]
        public async Task GetAllClassroomTypeReturnsEmptyContentWhenWrongIdIsGiven()
        {
            var classroomTypeRequest1 = MockData.GetCreateClassroomTypeDTODetails(scheduleId);
            var classroomTypeRequest2 = MockData.GetCreateSecondClassroomTypeDTODetails(scheduleId);

            await DbHelper.SetupClassroomType(_client, classroomTypeRequest1);
            await DbHelper.SetupClassroomType(_client, classroomTypeRequest2);

            _client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue(
                "Bearer",
                TestAuthorization.GetToken("Admin", "User", userId)
            );
            var query = $"?id={new Guid()}";
            var response = await _client.GetAsync($"/api/ClassroomType{query}");
            var stringResponse = await response.Content.ReadAsStringAsync();

            Assert.DoesNotContain(classroomTypeRequest1.Name, stringResponse);
            Assert.DoesNotContain(classroomTypeRequest2.Name, stringResponse);
        }

        [Fact]
        public async Task GetClassroomTypeReturnsClassroomType()
        {
            var classroomTypeRequest = MockData.GetCreateClassroomTypeDTODetails(scheduleId);

            var classroomTypeId = await DbHelper.SetupClassroomType(_client, classroomTypeRequest);

            _client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue(
                "Bearer",
                TestAuthorization.GetToken("Admin", "User", userId)
            );

            var response = await _client.GetAsync($"/api/ClassroomType/{classroomTypeId}");
            var jsonResponse = await response.Content.ReadFromJsonAsync<
                ApiGetResponse<ClassroomType>
            >();

            Assert.Equal(classroomTypeRequest.Name, jsonResponse!.Content.Name);
        }

        [Fact]
        public async Task GetScheduleThrowsNotFoundWhenWrongIdIsGiven()
        {
            var classroomTypeRequest = MockData.GetCreateClassroomTypeDTODetails(scheduleId);

            await DbHelper.SetupClassroomType(_client, classroomTypeRequest);

            _client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue(
                "Bearer",
                TestAuthorization.GetToken("Admin", "User", userId)
            );

            var response = await _client.GetAsync($"/api/ClassroomType/{new Guid()}");

            Assert.Equal(HttpStatusCode.NotFound, response.StatusCode);
        }

        [Fact]
        public async Task GetScheduleThrowsNotFoundErrorWhenWrongUserTokenIsGiven()
        {
            var classroomTypeRequest = MockData.GetCreateClassroomTypeDTODetails(scheduleId);

            var classroomTypeId = await DbHelper.SetupClassroomType(_client, classroomTypeRequest);

            _client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue(
                "Bearer",
                TestAuthorization.GetToken("User", "User", new Guid())
            );

            var response = await _client.GetAsync($"/api/ClassroomType/{classroomTypeId}");

            Assert.Equal(HttpStatusCode.NotFound, response.StatusCode);
        }

        [Fact]
        public async Task ClassroomTypeControllerThrowsUnauthorized()
        {
            _client.DefaultRequestHeaders.Authorization = null;

            var response = await _client.DeleteAsync($"/api/ClassroomType/{new Guid()}");
            Assert.Equal(HttpStatusCode.Unauthorized, response.StatusCode);

            response = await _client.PostAsJsonAsync(
                "/api/ClassroomType",
                MockData.GetCreateClassroomTypeDTODetails(scheduleId)
            );
            Assert.Equal(HttpStatusCode.Unauthorized, response.StatusCode);

            var query = $"?id={new Guid()}";
            response = await _client.GetAsync($"/api/ClassroomType{query}");
            Assert.Equal(HttpStatusCode.Unauthorized, response.StatusCode);

            response = await _client.PatchAsJsonAsync(
                $"/api/ClassroomType/{new Guid()}",
                MockData.GetUpdateClassroomTypeDTODetails()
            );
            Assert.Equal(HttpStatusCode.Unauthorized, response.StatusCode);

            response = await _client.GetAsync($"/api/ClassroomType/{new Guid()}");
            Assert.Equal(HttpStatusCode.Unauthorized, response.StatusCode);
        }

        [Fact]
        public async Task ClassroomTypeControllerThrowsTooManyRequests()
        {
            for (int i = 0; i != RateLimiterSettings.permitLimit; i++)
            {
                await _client.GetAsync("/api/ClassroomType");
            }

            _client.DefaultRequestHeaders.Authorization = null;

            var response = await _client.DeleteAsync($"/api/ClassroomType/{new Guid()}");
            Assert.Equal(HttpStatusCode.TooManyRequests, response.StatusCode);

            response = await _client.PostAsJsonAsync(
                "/api/ClassroomType",
                MockData.GetCreateClassroomTypeDTODetails(scheduleId)
            );
            Assert.Equal(HttpStatusCode.TooManyRequests, response.StatusCode);

            var query = $"?id={new Guid()}";
            response = await _client.GetAsync($"/api/ClassroomType{query}");
            Assert.Equal(HttpStatusCode.TooManyRequests, response.StatusCode);

            response = await _client.PatchAsJsonAsync(
                $"/api/ClassroomType/{new Guid()}",
                MockData.GetUpdateClassroomTypeDTODetails()
            );
            Assert.Equal(HttpStatusCode.TooManyRequests, response.StatusCode);

            response = await _client.GetAsync($"/api/ClassroomType/{new Guid()}");
            Assert.Equal(HttpStatusCode.TooManyRequests, response.StatusCode);
        }
    }
}
