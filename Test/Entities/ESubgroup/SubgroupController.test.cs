using System.Net;
using System.Net.Http.Headers;
using AlpimiAPI.Entities.ESchedule;
using AlpimiAPI.Entities.ESubgroup;
using AlpimiAPI.Responses;
using AlpimiAPI.Settings;
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

            var query = $"?groupId={groupId}";
            response = await _client.GetAsync($"/api/Subgroup");
            var stringResponse = await response.Content.ReadAsStringAsync();
            Assert.DoesNotContain(subgroupRequest.Name, stringResponse);
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

            var query = $"?groupId={groupId}";
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
            var jsonResponse = await response.Content.ReadFromJsonAsync<ApiGetResponse<Subgroup>>();

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
        public async Task GetAllSubgroupByScheduleReturnsSubgroups()
        {
            var subgroupRequest1 = MockData.GetCreateSubgroupDTODetails(groupId);
            var subgroupRequest2 = MockData.GetCreateSecondSubgroupDTODetails(groupId);

            await DbHelper.SetupSubgroup(_client, subgroupRequest1);
            await DbHelper.SetupSubgroup(_client, subgroupRequest2);

            _client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue(
                "Bearer",
                TestAuthorization.GetToken("Admin", "User", userId)
            );
            var query = $"?groupId={groupId}";
            var response = await _client.GetAsync($"/api/Subgroup{query}");
            var stringResponse = await response.Content.ReadAsStringAsync();

            Assert.Contains(subgroupRequest1.Name, stringResponse);
            Assert.Contains(subgroupRequest2.Name, stringResponse);
        }

        [Fact]
        public async Task GetAllSubgroupByScheduleReturnsEmptyContentWhenWrongUserAttemptsGet()
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
        public async Task GetAllSubgroupByScheduleReturnsEmptyContentWhenWrongIdIsGiven()
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
            var jsonResponse = await response.Content.ReadFromJsonAsync<ApiGetResponse<Subgroup>>();

            Assert.Equal(subgroupRequest.Name, jsonResponse!.Content.Name);
            Assert.Equal(subgroupRequest.StudentCount, jsonResponse!.Content.StudentCount);
        }

        [Fact]
        public async Task GetScheduleThrowsNotFoundWhenWrongIdIsGiven()
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
        public async Task GetScheduleThrowsNotFoundErrorWhenWrongUserTokenIsGiven()
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
