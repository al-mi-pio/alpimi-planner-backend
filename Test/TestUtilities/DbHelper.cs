using System.Net.Http.Headers;
using AlpimiAPI.Database;
using AlpimiAPI.Entities.EDayOff.DTO;
using AlpimiAPI.Entities.EGroup.DTO;
using AlpimiAPI.Entities.ELessonPeriod.DTO;
using AlpimiAPI.Entities.ESchedule.DTO;
using AlpimiAPI.Entities.ETeacher.DTO;
using AlpimiAPI.Entities.EUser.DTO;
using AlpimiAPI.Responses;
using AlpimiAPI.Utilities;
using Microsoft.Data.SqlClient;

namespace AlpimiTest.TestUtilities
{
    public static class DbHelper
    {
        public static async Task UserCleaner(HttpClient _client)
        {
            _client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue(
                "Bearer",
                TestAuthorization.GetToken("Admin", "Bob", new Guid())
            );
            var query = "SELECT [Id] FROM [User]";
            var _dbService = new DbService(
                new SqlConnection(Configuration.GetTestConnectionString())
            );
            var userIds = await _dbService.GetAll<Guid>(query, "");
            foreach (var userId in userIds!)
            {
                await _client.DeleteAsync($"/api/User/{userId}");
            }
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

        public static async Task<Guid> SetupDayOff(
            HttpClient _client,
            CreateDayOffDTO dayOffRequest
        )
        {
            _client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue(
                "Bearer",
                TestAuthorization.GetToken("Admin", "Bob", new Guid())
            );

            var dayOffId = await _client.PostAsJsonAsync("/api/DayOff", dayOffRequest);
            var jsonDayOffId = await dayOffId.Content.ReadFromJsonAsync<ApiGetResponse<Guid>>();

            return jsonDayOffId!.Content;
        }

        public static async Task<Guid> SetupLessonPeriod(
            HttpClient _client,
            CreateLessonPeriodDTO lessonPeriodRequest
        )
        {
            _client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue(
                "Bearer",
                TestAuthorization.GetToken("Admin", "Bob", new Guid())
            );

            var lessonPeriodId = await _client.PostAsJsonAsync(
                "/api/LessonPeriod",
                lessonPeriodRequest
            );
            var jsonLessonPeriodId = await lessonPeriodId.Content.ReadFromJsonAsync<
                ApiGetResponse<Guid>
            >();

            return jsonLessonPeriodId!.Content;
        }

        public static async Task<Guid> SetupTeacher(
            HttpClient _client,
            CreateTeacherDTO teacherRequest
        )
        {
            _client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue(
                "Bearer",
                TestAuthorization.GetToken("Admin", "Bob", new Guid())
            );

            var teacher = await _client.PostAsJsonAsync("/api/Teacher", teacherRequest);

            var jsonTeacherId = await teacher.Content.ReadFromJsonAsync<ApiGetResponse<Guid>>();

            return jsonTeacherId!.Content;
        }

        public static async Task<Guid> SetupGroup(HttpClient _client, CreateGroupDTO groupRequest)
        {
            _client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue(
                "Bearer",
                TestAuthorization.GetToken("Admin", "Bob", new Guid())
            );

            var group = await _client.PostAsJsonAsync("/api/Group", groupRequest);

            var jsonGroupId = await group.Content.ReadFromJsonAsync<ApiGetResponse<Guid>>();

            return jsonGroupId!.Content;
        }
    }
}
