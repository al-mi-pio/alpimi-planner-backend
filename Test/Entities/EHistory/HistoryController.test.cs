using System.Net;
using System.Net.Http.Headers;
using AlpimiAPI.Utilities;
using AlpimiTest.TestSetup;
using AlpimiTest.TestUtilities;
using Xunit;

namespace AlpimiTest.Entities.EHistory
{
    [Collection("Sequential Tests")]
    public class HistoryControllerTest : IAsyncLifetime
    {
        CustomWebApplicationFactory<Program> _factory;
        HttpClient _client;
        Guid userId;
        Guid scheduleId;
        Guid groupId;

        public HistoryControllerTest()
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
        public async Task HistoryControllerThrowsUnauthorized()
        {
            _client.DefaultRequestHeaders.Authorization = null;

            var response = await _client.PatchAsJsonAsync($"/api/History/undo/{new Guid()}", "");
            Assert.Equal(HttpStatusCode.Unauthorized, response.StatusCode);

            response = await _client.PatchAsJsonAsync($"/api/History/redo/{new Guid()}", "");
            Assert.Equal(HttpStatusCode.Unauthorized, response.StatusCode);
        }

        [Fact]
        public async Task HistoryControllerThrowsTooManyRequests()
        {
            for (int i = 0; i != Configuration.GetPermitLimit(); i++)
            {
                await _client.PatchAsJsonAsync($"/api/History/undo/{new Guid()}", "");
            }
            _client.DefaultRequestHeaders.Authorization = null;

            var response = await _client.PatchAsJsonAsync($"/api/History/undo/{new Guid()}", "");
            Assert.Equal(HttpStatusCode.TooManyRequests, response.StatusCode);

            response = await _client.PatchAsJsonAsync($"/api/History/redo/{new Guid()}", "");
            Assert.Equal(HttpStatusCode.TooManyRequests, response.StatusCode);
        }

        [Fact]
        public async Task ReplacesGuidsIfANewEntityIsCreated()
        {
            var groupUpdateRequest = MockData.GetUpdateGroupDTODetails();
            _client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue(
                "Bearer",
                TestAuthorization.GetToken("Admin", "Bob", userId)
            );

            await _client.PatchAsJsonAsync($"/api/Group/{groupId}", groupUpdateRequest);
            await _client.DeleteAsync($"/api/Group/{groupId}");

            var response = await _client.PatchAsJsonAsync($"/api/History/undo/{scheduleId}", "");
            Assert.Equal(HttpStatusCode.OK, response.StatusCode);
            response = await _client.PatchAsJsonAsync($"/api/History/undo/{scheduleId}", "");
            Assert.Equal(HttpStatusCode.OK, response.StatusCode);
        }

        [Fact]
        public async Task UndoThrowsErrorWhenBadScheduleIdIsGiven()
        {
            _client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue(
                "Bearer",
                TestAuthorization.GetToken("Admin", "Bob", userId)
            );

            var response = await _client.PatchAsJsonAsync($"/api/History/undo/{new Guid()}", "");

            Assert.Equal(HttpStatusCode.BadRequest, response.StatusCode);
        }

        [Fact]
        public async Task UndoThrowsErrorWhenWrongUserAttemptsToUndo()
        {
            _client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue(
                "Bearer",
                TestAuthorization.GetToken("User", "Bob", new Guid())
            );

            var response = await _client.PatchAsJsonAsync($"/api/History/undo/{new Guid()}", "");

            Assert.Equal(HttpStatusCode.BadRequest, response.StatusCode);
        }

        [Fact]
        public async Task RedoThrowsErrorWhenBadScheduleIdIsGiven()
        {
            _client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue(
                "Bearer",
                TestAuthorization.GetToken("Admin", "Bob", userId)
            );

            var response = await _client.PatchAsJsonAsync($"/api/History/redo/{new Guid()}", "");

            Assert.Equal(HttpStatusCode.BadRequest, response.StatusCode);
        }

        [Fact]
        public async Task RedoThrowsErrorWhenWrongUserAttemptsToRedo()
        {
            _client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue(
                "Bearer",
                TestAuthorization.GetToken("User", "User", new Guid())
            );

            var response = await _client.PatchAsJsonAsync($"/api/History/redo/{new Guid()}", "");

            Assert.Equal(HttpStatusCode.BadRequest, response.StatusCode);
        }
    }
}
