using System.Net;
using System.Net.Http.Headers;
using AlpimiAPI.Entities.EGroup.DTO;
using AlpimiAPI.Responses;
using AlpimiAPI.Utilities;
using AlpimiTest.TestSetup;
using AlpimiTest.TestUtilities;
using Xunit;

namespace AlpimiTest.Entities.EGroup
{
    [Collection("Sequential Tests")]
    public class GroupControllerTest : IAsyncLifetime
    {
        CustomWebApplicationFactory<Program> _factory;
        HttpClient _client;
        Guid userId;
        Guid scheduleId;

        public GroupControllerTest()
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
        public async Task GroupControllerThrowsUnauthorized()
        {
            _client.DefaultRequestHeaders.Authorization = null;

            var response = await _client.DeleteAsync($"/api/Group/{new Guid()}");
            Assert.Equal(HttpStatusCode.Unauthorized, response.StatusCode);

            response = await _client.PostAsJsonAsync(
                "/api/Group",
                MockData.GetCreateGroupDTODetails(scheduleId)
            );
            Assert.Equal(HttpStatusCode.Unauthorized, response.StatusCode);

            var query = $"?scheduleId={new Guid()}";
            response = await _client.GetAsync($"/api/Group{query}");
            Assert.Equal(HttpStatusCode.Unauthorized, response.StatusCode);

            response = await _client.PatchAsJsonAsync(
                $"/api/Group/{new Guid()}",
                MockData.GetUpdateGroupDTODetails()
            );
            Assert.Equal(HttpStatusCode.Unauthorized, response.StatusCode);

            response = await _client.GetAsync($"/api/Group/{new Guid()}");
            Assert.Equal(HttpStatusCode.Unauthorized, response.StatusCode);
        }

        [Fact]
        public async Task GroupControllerThrowsTooManyRequests()
        {
            for (int i = 0; i != Configuration.GetPermitLimit(); i++)
            {
                await _client.GetAsync("/api/Group");
            }
            _client.DefaultRequestHeaders.Authorization = null;

            var response = await _client.DeleteAsync($"/api/Group/{new Guid()}");
            Assert.Equal(HttpStatusCode.TooManyRequests, response.StatusCode);

            response = await _client.PostAsJsonAsync(
                "/api/Group",
                MockData.GetCreateGroupDTODetails(scheduleId)
            );
            Assert.Equal(HttpStatusCode.TooManyRequests, response.StatusCode);

            var query = $"?scheduleId={new Guid()}";
            response = await _client.GetAsync($"/api/Group{query}");
            Assert.Equal(HttpStatusCode.TooManyRequests, response.StatusCode);

            response = await _client.PatchAsJsonAsync(
                $"/api/Group/{new Guid()}",
                MockData.GetUpdateGroupDTODetails()
            );
            Assert.Equal(HttpStatusCode.TooManyRequests, response.StatusCode);

            response = await _client.GetAsync($"/api/Group/{new Guid()}");
            Assert.Equal(HttpStatusCode.TooManyRequests, response.StatusCode);
        }

        [Fact]
        public async Task GroupIsCreated()
        {
            var groupRequest = MockData.GetCreateGroupDTODetails(scheduleId);
            _client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue(
                "Bearer",
                TestAuthorization.GetToken("Admin", "User", userId)
            );

            var response = await _client.PostAsJsonAsync("/api/Group", groupRequest);
            Assert.Equal(HttpStatusCode.OK, response.StatusCode);

            var query = $"?scheduleId={scheduleId}";
            response = await _client.GetAsync($"/api/Group{query}");
            var stringResponse = await response.Content.ReadAsStringAsync();
            Assert.Contains(groupRequest.Name, stringResponse);
        }

        [Fact]
        public async Task GroupIsDeleted()
        {
            var groupRequest = MockData.GetCreateGroupDTODetails(scheduleId);
            var groupId = await DbHelper.SetupGroup(_client, groupRequest);
            _client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue(
                "Bearer",
                TestAuthorization.GetToken("Admin", "User", new Guid())
            );

            var response = await _client.DeleteAsync($"/api/Group/{groupId}");
            Assert.Equal(HttpStatusCode.NoContent, response.StatusCode);

            var query = $"?scheduleId={scheduleId}";
            response = await _client.GetAsync($"/api/Group");
            var stringResponse = await response.Content.ReadAsStringAsync();
            Assert.DoesNotContain(groupRequest.Name, stringResponse);
        }

        [Fact]
        public async Task UpdateGroupReturnsUpdatedGroup()
        {
            var groupUpdateRequest = MockData.GetUpdateGroupDTODetails();
            var groupId = await DbHelper.SetupGroup(
                _client,
                MockData.GetCreateGroupDTODetails(scheduleId)
            );
            _client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue(
                "Bearer",
                TestAuthorization.GetToken("Admin", "Bob", userId)
            );

            var response = await _client.PatchAsJsonAsync(
                $"/api/Group/{groupId}",
                groupUpdateRequest
            );
            var jsonResponse = await response.Content.ReadFromJsonAsync<ApiGetResponse<GroupDTO>>();

            Assert.Equal(groupUpdateRequest.Name, jsonResponse!.Content.Name);
            Assert.Equal(groupUpdateRequest.StudentCount, jsonResponse!.Content.StudentCount);
        }

        [Fact]
        public async Task UpdateGroupThrowsNotFoundErrorWhenWrongIdIsGiven()
        {
            var groupUpdateRequest = MockData.GetUpdateGroupDTODetails();
            var groupId = await DbHelper.SetupGroup(
                _client,
                MockData.GetCreateGroupDTODetails(scheduleId)
            );
            _client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue(
                "Bearer",
                TestAuthorization.GetToken("Admin", "User", userId)
            );

            var response = await _client.PatchAsJsonAsync(
                $"/api/Group/{new Guid()}",
                groupUpdateRequest
            );

            Assert.Equal(HttpStatusCode.NotFound, response.StatusCode);
        }

        [Fact]
        public async Task UpdateGroupThrowsNotFoundErrorWhenWrongUserAttemptsUpdate()
        {
            var groupUpdateRequest = MockData.GetUpdateGroupDTODetails();
            var groupId = await DbHelper.SetupGroup(
                _client,
                MockData.GetCreateGroupDTODetails(scheduleId)
            );
            _client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue(
                "Bearer",
                TestAuthorization.GetToken("User", "User", new Guid())
            );

            var response = await _client.PatchAsJsonAsync(
                $"/api/Group/{groupId}",
                groupUpdateRequest
            );

            Assert.Equal(HttpStatusCode.NotFound, response.StatusCode);
        }

        [Fact]
        public async Task GetAllGroupByScheduleReturnsGroups()
        {
            var groupRequest1 = MockData.GetCreateGroupDTODetails(scheduleId);
            var groupRequest2 = MockData.GetCreateSecondGroupDTODetails(scheduleId);
            await DbHelper.SetupGroup(_client, groupRequest1);
            await DbHelper.SetupGroup(_client, groupRequest2);
            _client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue(
                "Bearer",
                TestAuthorization.GetToken("Admin", "User", userId)
            );

            var query = $"?scheduleId={scheduleId}";
            var response = await _client.GetAsync($"/api/Group{query}");
            var stringResponse = await response.Content.ReadAsStringAsync();

            Assert.Contains(groupRequest1.Name, stringResponse);
            Assert.Contains(groupRequest2.Name, stringResponse);
        }

        [Fact]
        public async Task GetAllGroupByScheduleReturnsEmptyContentWhenWrongUserAttemptsGet()
        {
            var groupRequest1 = MockData.GetCreateGroupDTODetails(scheduleId);
            var groupRequest2 = MockData.GetCreateSecondGroupDTODetails(scheduleId);
            await DbHelper.SetupGroup(_client, groupRequest1);
            await DbHelper.SetupGroup(_client, groupRequest2);
            _client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue(
                "Bearer",
                TestAuthorization.GetToken("User", "User", new Guid())
            );

            var query = $"?scheduleId={scheduleId}";
            var response = await _client.GetAsync($"/api/Group{query}");
            var stringResponse = await response.Content.ReadAsStringAsync();

            Assert.DoesNotContain(groupRequest1.Name, stringResponse);
            Assert.DoesNotContain(groupRequest2.Name, stringResponse);
        }

        [Fact]
        public async Task GetAllGroupByScheduleReturnsEmptyContentWhenWrongIdIsGiven()
        {
            var groupRequest1 = MockData.GetCreateGroupDTODetails(scheduleId);
            var groupRequest2 = MockData.GetCreateSecondGroupDTODetails(scheduleId);
            await DbHelper.SetupGroup(_client, groupRequest1);
            await DbHelper.SetupGroup(_client, groupRequest2);
            _client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue(
                "Bearer",
                TestAuthorization.GetToken("Admin", "User", userId)
            );

            var query = $"?scheduleId={new Guid()}";
            var response = await _client.GetAsync($"/api/Group{query}");
            var stringResponse = await response.Content.ReadAsStringAsync();

            Assert.DoesNotContain(groupRequest1.Name, stringResponse);
            Assert.DoesNotContain(groupRequest2.Name, stringResponse);
        }

        [Fact]
        public async Task GetGroupReturnsGroup()
        {
            var groupRequest = MockData.GetCreateGroupDTODetails(scheduleId);
            var groupId = await DbHelper.SetupGroup(_client, groupRequest);
            _client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue(
                "Bearer",
                TestAuthorization.GetToken("Admin", "User", userId)
            );

            var response = await _client.GetAsync($"/api/Group/{groupId}");
            var jsonResponse = await response.Content.ReadFromJsonAsync<ApiGetResponse<GroupDTO>>();

            Assert.Equal(groupRequest.Name, jsonResponse!.Content.Name);
            Assert.Equal(groupRequest.StudentCount, jsonResponse!.Content.StudentCount);
        }

        [Fact]
        public async Task GetScheduleThrowsNotFoundErrorWhenWrongUserTokenIsGiven()
        {
            var groupRequest = MockData.GetCreateGroupDTODetails(scheduleId);
            var groupId = await DbHelper.SetupGroup(_client, groupRequest);
            _client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue(
                "Bearer",
                TestAuthorization.GetToken("User", "User", new Guid())
            );

            var response = await _client.GetAsync($"/api/Group/{groupId}");

            Assert.Equal(HttpStatusCode.NotFound, response.StatusCode);
        }

        [Fact]
        public async Task GetScheduleThrowsNotFoundWhenWrongIdIsGiven()
        {
            var groupRequest = MockData.GetCreateGroupDTODetails(scheduleId);
            await DbHelper.SetupGroup(_client, groupRequest);
            _client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue(
                "Bearer",
                TestAuthorization.GetToken("Admin", "User", userId)
            );

            var response = await _client.GetAsync($"/api/Group/{new Guid()}");

            Assert.Equal(HttpStatusCode.NotFound, response.StatusCode);
        }
    }
}
