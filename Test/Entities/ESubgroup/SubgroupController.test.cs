using System.Net;
using System.Net.Http.Headers;
using AlpimiAPI.Entities.ESubgroup;
using AlpimiAPI.Entities.ESubgroup.DTO;
using AlpimiAPI.Responses;
using AlpimiAPI.Settings;
using AlpimiTest.TestSetup;
using AlpimiTest.TestUtilities;
using Xunit;

namespace AlpimiTest.Entities.ESubgroup
{
    [Collection("Sequential Tests")]
    public class SubgroupControllerTest : IAsyncLifetime
    {
        CustomWebApplicationFactory<Program> _factory;
        HttpClient _client;
        Guid userId;
        Guid scheduleId;
        Guid groupId;

        public SubgroupControllerTest()
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
        }

        public async Task DisposeAsync()
        {
            await DbHelper.UserCleaner(_client);
        }

        [Fact]
        public async Task SubgroupIsDeleted()
        {
            var subgroupRequest = MockData.GetCreateSubgroupDTODetails(groupId);
            var subgroupId = await DbHelper.SetupSubgroup(_client, subgroupRequest);

            _client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue(
                "Bearer",
                TestAuthorization.GetToken("Admin", "User", new Guid())
            );

            var response = await _client.DeleteAsync($"/api/Subgroup/{subgroupId}");
            Assert.Equal(HttpStatusCode.NoContent, response.StatusCode);

            var query = $"?id={groupId}";
            response = await _client.GetAsync($"/api/Subgroup");
            var stringResponse = await response.Content.ReadAsStringAsync();
            Assert.DoesNotContain(subgroupRequest.Name, stringResponse);
        }

        [Fact]
        public async Task SubgroupsLessonsAreDeleted()
        {
            var subgroupRequest = MockData.GetCreateSubgroupDTODetails(groupId);
            var subgroupId = await DbHelper.SetupSubgroup(_client, subgroupRequest);
            var lessonTypeId = await DbHelper.SetupLessonType(
                _client,
                MockData.GetCreateLessonTypeDTODetails(scheduleId)
            );
            var lessonRequest = MockData.GetCreateLessonDTODetails(subgroupId, lessonTypeId);

            _client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue(
                "Bearer",
                TestAuthorization.GetToken("Admin", "User", new Guid())
            );

            var response = await _client.DeleteAsync($"/api/Subgroup/{subgroupId}");
            Assert.Equal(HttpStatusCode.NoContent, response.StatusCode);

            var query = $"?id={groupId}";
            response = await _client.GetAsync($"/api/Lesson");
            var stringResponse = await response.Content.ReadAsStringAsync();
            Assert.DoesNotContain(lessonRequest.Name, stringResponse);
        }

        [Fact]
        public async Task SubgroupIsCreated()
        {
            var subgroupRequest = MockData.GetCreateSubgroupDTODetails(groupId);

            _client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue(
                "Bearer",
                TestAuthorization.GetToken("Admin", "User", userId)
            );

            var response = await _client.PostAsJsonAsync("/api/Subgroup", subgroupRequest);
            Assert.Equal(HttpStatusCode.OK, response.StatusCode);

            var query = $"?id={groupId}";
            response = await _client.GetAsync($"/api/Subgroup{query}");
            var stringResponse = await response.Content.ReadAsStringAsync();
            Assert.Contains(subgroupRequest.Name, stringResponse);
        }

        [Fact]
        public async Task UpdateSubgroupReturnsUpdatedSubgroup()
        {
            var subgroupUpdateRequest = MockData.GetUpdateSubgroupDTODetails();
            var subgroupId = await DbHelper.SetupSubgroup(
                _client,
                MockData.GetCreateSubgroupDTODetails(groupId)
            );

            _client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue(
                "Bearer",
                TestAuthorization.GetToken("Admin", "Bob", userId)
            );

            var response = await _client.PatchAsJsonAsync(
                $"/api/Subgroup/{subgroupId}",
                subgroupUpdateRequest
            );
            var jsonResponse = await response.Content.ReadFromJsonAsync<
                ApiGetResponse<SubgroupDTO>
            >();

            Assert.Equal(subgroupUpdateRequest.Name, jsonResponse!.Content.Name);
            Assert.Equal(subgroupUpdateRequest.StudentCount, jsonResponse!.Content.StudentCount);
        }

        [Fact]
        public async Task UpdateSubgroupThrowsNotFoundErrorWhenWrongIdIsGiven()
        {
            var subgroupUpdateRequest = MockData.GetUpdateSubgroupDTODetails();

            var subgroupId = await DbHelper.SetupSubgroup(
                _client,
                MockData.GetCreateSubgroupDTODetails(groupId)
            );

            _client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue(
                "Bearer",
                TestAuthorization.GetToken("Admin", "User", userId)
            );
            var response = await _client.PatchAsJsonAsync(
                $"/api/Subgroup/{new Guid()}",
                subgroupUpdateRequest
            );

            Assert.Equal(HttpStatusCode.NotFound, response.StatusCode);
        }

        [Fact]
        public async Task UpdateSubgroupThrowsNotFoundErrorWhenWrongUserAttemptsUpdate()
        {
            var subgroupUpdateRequest = MockData.GetUpdateSubgroupDTODetails();

            var subgroupId = await DbHelper.SetupSubgroup(
                _client,
                MockData.GetCreateSubgroupDTODetails(groupId)
            );

            _client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue(
                "Bearer",
                TestAuthorization.GetToken("User", "User", new Guid())
            );
            var response = await _client.PatchAsJsonAsync(
                $"/api/Subgroup/{subgroupId}",
                subgroupUpdateRequest
            );

            Assert.Equal(HttpStatusCode.NotFound, response.StatusCode);
        }

        [Fact]
        public async Task GetAllSubgroupsReturnsSubgroupsFromGroupIfGroupIdIsProvided()
        {
            var subgroupRequest1 = MockData.GetCreateSubgroupDTODetails(groupId);
            var subgroupRequest2 = MockData.GetCreateSecondSubgroupDTODetails(groupId);

            await DbHelper.SetupSubgroup(_client, subgroupRequest1);
            await DbHelper.SetupSubgroup(_client, subgroupRequest2);

            _client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue(
                "Bearer",
                TestAuthorization.GetToken("Admin", "User", userId)
            );
            var query = $"?id={groupId}";
            var response = await _client.GetAsync($"/api/Subgroup{query}");
            var stringResponse = await response.Content.ReadAsStringAsync();

            Assert.Contains(subgroupRequest1.Name, stringResponse);
            Assert.Contains(subgroupRequest2.Name, stringResponse);
        }

        [Fact]
        public async Task GetAllSubgroupsReturnsSubgroupsFromStudentIfStudentIdIsProvided()
        {
            var subgroupRequest1 = MockData.GetCreateSubgroupDTODetails(groupId);
            var subgroupRequest2 = MockData.GetCreateSecondSubgroupDTODetails(groupId);

            var subgroupId = await DbHelper.SetupSubgroup(_client, subgroupRequest1);
            await DbHelper.SetupSubgroup(_client, subgroupRequest2);

            var studentRequest = MockData.GetCreateStudentDTODetails(groupId);
            studentRequest.SubgroupIds = [subgroupId];

            var studentId = await DbHelper.SetupStudent(_client, studentRequest);

            _client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue(
                "Bearer",
                TestAuthorization.GetToken("Admin", "User", userId)
            );
            var query = $"?id={studentId}";
            var response = await _client.GetAsync($"/api/Subgroup{query}");
            var stringResponse = await response.Content.ReadAsStringAsync();

            Assert.Contains(subgroupRequest1.Name, stringResponse);
            Assert.DoesNotContain(subgroupRequest2.Name, stringResponse);
        }

        [Fact]
        public async Task GetAllSubgroupsReturnsEmptyContentWhenWrongUserAttemptsGet()
        {
            var subgroupRequest1 = MockData.GetCreateSubgroupDTODetails(groupId);
            var subgroupRequest2 = MockData.GetCreateSecondSubgroupDTODetails(groupId);

            await DbHelper.SetupSubgroup(_client, subgroupRequest1);
            await DbHelper.SetupSubgroup(_client, subgroupRequest2);

            _client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue(
                "Bearer",
                TestAuthorization.GetToken("User", "User", new Guid())
            );
            var query = $"?groupId={groupId}";
            var response = await _client.GetAsync($"/api/Subgroup{query}");
            var stringResponse = await response.Content.ReadAsStringAsync();

            Assert.DoesNotContain(subgroupRequest1.Name, stringResponse);
            Assert.DoesNotContain(subgroupRequest2.Name, stringResponse);
        }

        [Fact]
        public async Task GetAllSubgroupsReturnsEmptyContentWhenWrongIdIsGiven()
        {
            var subgroupRequest1 = MockData.GetCreateSubgroupDTODetails(groupId);
            var subgroupRequest2 = MockData.GetCreateSecondSubgroupDTODetails(groupId);

            await DbHelper.SetupSubgroup(_client, subgroupRequest1);
            await DbHelper.SetupSubgroup(_client, subgroupRequest2);

            _client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue(
                "Bearer",
                TestAuthorization.GetToken("Admin", "User", userId)
            );
            var query = $"?groupId={new Guid()}";
            var response = await _client.GetAsync($"/api/Subgroup{query}");
            var stringResponse = await response.Content.ReadAsStringAsync();

            Assert.DoesNotContain(subgroupRequest1.Name, stringResponse);
            Assert.DoesNotContain(subgroupRequest2.Name, stringResponse);
        }

        [Fact]
        public async Task GetSubgroupReturnsSubgroup()
        {
            var subgroupRequest = MockData.GetCreateSubgroupDTODetails(groupId);

            var subgroupId = await DbHelper.SetupSubgroup(_client, subgroupRequest);

            _client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue(
                "Bearer",
                TestAuthorization.GetToken("Admin", "User", userId)
            );

            var response = await _client.GetAsync($"/api/Subgroup/{subgroupId}");
            var jsonResponse = await response.Content.ReadFromJsonAsync<
                ApiGetResponse<SubgroupDTO>
            >();

            Assert.Equal(subgroupRequest.Name, jsonResponse!.Content.Name);
            Assert.Equal(subgroupRequest.StudentCount, jsonResponse!.Content.StudentCount);
        }

        [Fact]
        public async Task GetSubgroupThrowsNotFoundWhenWrongIdIsGiven()
        {
            var subgroupRequest = MockData.GetCreateSubgroupDTODetails(groupId);

            await DbHelper.SetupSubgroup(_client, subgroupRequest);

            _client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue(
                "Bearer",
                TestAuthorization.GetToken("Admin", "User", userId)
            );

            var response = await _client.GetAsync($"/api/Subgroup/{new Guid()}");

            Assert.Equal(HttpStatusCode.NotFound, response.StatusCode);
        }

        [Fact]
        public async Task GetSubgroupThrowsNotFoundErrorWhenWrongUserTokenIsGiven()
        {
            var subgroupRequest = MockData.GetCreateSubgroupDTODetails(groupId);

            var subgroupId = await DbHelper.SetupSubgroup(_client, subgroupRequest);

            _client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue(
                "Bearer",
                TestAuthorization.GetToken("User", "User", new Guid())
            );

            var response = await _client.GetAsync($"/api/Subgroup/{subgroupId}");

            Assert.Equal(HttpStatusCode.NotFound, response.StatusCode);
        }

        [Fact]
        public async Task SubgroupControllerThrowsUnauthorized()
        {
            _client.DefaultRequestHeaders.Authorization = null;

            var response = await _client.DeleteAsync($"/api/Subgroup/{new Guid()}");
            Assert.Equal(HttpStatusCode.Unauthorized, response.StatusCode);

            response = await _client.PostAsJsonAsync(
                "/api/Subgroup",
                MockData.GetCreateSubgroupDTODetails(groupId)
            );
            Assert.Equal(HttpStatusCode.Unauthorized, response.StatusCode);

            var query = $"?groupId={new Guid()}";
            response = await _client.GetAsync($"/api/Subgroup{query}");
            Assert.Equal(HttpStatusCode.Unauthorized, response.StatusCode);

            response = await _client.PatchAsJsonAsync(
                $"/api/Subgroup/{new Guid()}",
                MockData.GetUpdateSubgroupDTODetails()
            );
            Assert.Equal(HttpStatusCode.Unauthorized, response.StatusCode);

            response = await _client.GetAsync($"/api/Subgroup/{new Guid()}");
            Assert.Equal(HttpStatusCode.Unauthorized, response.StatusCode);
        }

        [Fact]
        public async Task SubgroupControllerThrowsTooManyRequests()
        {
            for (int i = 0; i != RateLimiterSettings.permitLimit; i++)
            {
                await _client.GetAsync("/api/Subgroup");
            }

            _client.DefaultRequestHeaders.Authorization = null;

            var response = await _client.DeleteAsync($"/api/Subgroup/{new Guid()}");
            Assert.Equal(HttpStatusCode.TooManyRequests, response.StatusCode);

            response = await _client.PostAsJsonAsync(
                "/api/Subgroup",
                MockData.GetCreateSubgroupDTODetails(groupId)
            );
            Assert.Equal(HttpStatusCode.TooManyRequests, response.StatusCode);

            var query = $"?groupId={new Guid()}";
            response = await _client.GetAsync($"/api/Subgroup{query}");
            Assert.Equal(HttpStatusCode.TooManyRequests, response.StatusCode);

            response = await _client.PatchAsJsonAsync(
                $"/api/Subgroup/{new Guid()}",
                MockData.GetUpdateSubgroupDTODetails()
            );
            Assert.Equal(HttpStatusCode.TooManyRequests, response.StatusCode);

            response = await _client.GetAsync($"/api/Subgroup/{new Guid()}");
            Assert.Equal(HttpStatusCode.TooManyRequests, response.StatusCode);
        }
    }
}
