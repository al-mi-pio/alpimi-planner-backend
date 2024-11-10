using System.Net.Http.Headers;
using AlpimiAPI.Entities.ESchedule;
using AlpimiAPI.Entities.ESchedule.DTO;
using AlpimiAPI.Entities.EUser;
using AlpimiAPI.Entities.EUser.DTO;
using AlpimiAPI.Responses;

namespace AlpimiTest.TestUtilities
{
    public static class DbHelper
    {
        public static async Task<bool> UserCleaner(HttpClient _client, Guid toDeleteUserId)
        {
            _client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue(
                "Bearer",
                TestAuthorization.GetToken("Admin", "Bob", new Guid())
            );
            await _client.DeleteAsync($"/api/User/{toDeleteUserId}");
            return true;
        }

        public static async Task<Guid> SetupUser(HttpClient _client, CreateUserDTO userRequest)
        {
            _client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue(
                "Bearer",
                TestAuthorization.GetToken("Admin", "Bob", new Guid())
            );

            var userId = await _client.PostAsJsonAsync("/api/User", userRequest);
            var jsonId = await userId.Content.ReadFromJsonAsync<ApiGetResponse<Guid>>();

            return jsonId!.Content;
        }

        public static async Task<Guid> SetupSchedule(
            HttpClient _client,
            Guid userId,
            CreateScheduleDTO scheduleRequest
        )
        {
            _client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue(
                "Bearer",
                TestAuthorization.GetToken("Admin", "Bob", userId)
            );

            var scheduleId = await _client.PostAsJsonAsync("/api/Schedule", scheduleRequest);
            var jsonScheduleId = await scheduleId.Content.ReadFromJsonAsync<ApiGetResponse<Guid>>();

            return jsonScheduleId!.Content;
        }
    }
}
