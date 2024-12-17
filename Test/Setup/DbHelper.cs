using System.Net.Http.Headers;
using AlpimiAPI.Database;
using AlpimiAPI.Entities.EClassroom.DTO;
using AlpimiAPI.Entities.EClassroomType.DTO;
using AlpimiAPI.Entities.EDayOff.DTO;
using AlpimiAPI.Entities.EGroup.DTO;
using AlpimiAPI.Entities.ELesson.DTO;
using AlpimiAPI.Entities.ELessonBlock.DTO;
using AlpimiAPI.Entities.ELessonPeriod.DTO;
using AlpimiAPI.Entities.ELessonType.DTO;
using AlpimiAPI.Entities.ESchedule.DTO;
using AlpimiAPI.Entities.EStudent.DTO;
using AlpimiAPI.Entities.ESubgroup.DTO;
using AlpimiAPI.Entities.ETeacher.DTO;
using AlpimiAPI.Entities.EUser.DTO;
using AlpimiAPI.Responses;
using AlpimiAPI.Utilities;
using AlpimiTest.TestUtilities;
using Microsoft.Data.SqlClient;

namespace AlpimiTest.TestSetup
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

        public static async Task<Guid> SetupSubgroup(
            HttpClient _client,
            CreateSubgroupDTO subgroupRequest
        )
        {
            _client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue(
                "Bearer",
                TestAuthorization.GetToken("Admin", "Bob", new Guid())
            );

            var subgroup = await _client.PostAsJsonAsync("/api/Subgroup", subgroupRequest);
            var jsonSubgroupId = await subgroup.Content.ReadFromJsonAsync<ApiGetResponse<Guid>>();

            return jsonSubgroupId!.Content;
        }

        public static async Task<Guid> SetupStudent(
            HttpClient _client,
            CreateStudentDTO studentRequest
        )
        {
            _client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue(
                "Bearer",
                TestAuthorization.GetToken("Admin", "Bob", new Guid())
            );

            var student = await _client.PostAsJsonAsync("/api/Student", studentRequest);
            var jsonStudentId = await student.Content.ReadFromJsonAsync<ApiGetResponse<Guid>>();

            return jsonStudentId!.Content;
        }

        public static async Task<Guid> SetupClassroomType(
            HttpClient _client,
            CreateClassroomTypeDTO classroomTypeRequest
        )
        {
            _client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue(
                "Bearer",
                TestAuthorization.GetToken("Admin", "Bob", new Guid())
            );

            var classroomType = await _client.PostAsJsonAsync(
                "/api/ClassroomType",
                classroomTypeRequest
            );
            var jsonClassroomTypeId = await classroomType.Content.ReadFromJsonAsync<
                ApiGetResponse<Guid>
            >();

            return jsonClassroomTypeId!.Content;
        }

        public static async Task<Guid> SetupClassroom(
            HttpClient _client,
            CreateClassroomDTO classroomRequest
        )
        {
            _client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue(
                "Bearer",
                TestAuthorization.GetToken("Admin", "Bob", new Guid())
            );

            var classroom = await _client.PostAsJsonAsync("/api/Classroom", classroomRequest);
            var jsonClassroomId = await classroom.Content.ReadFromJsonAsync<ApiGetResponse<Guid>>();

            return jsonClassroomId!.Content;
        }

        public static async Task<Guid> SetupLessonType(
            HttpClient _client,
            CreateLessonTypeDTO lessonTypeRequest
        )
        {
            _client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue(
                "Bearer",
                TestAuthorization.GetToken("Admin", "Bob", new Guid())
            );

            var lessonType = await _client.PostAsJsonAsync("/api/LessonType", lessonTypeRequest);
            var jsonLessonTypeId = await lessonType.Content.ReadFromJsonAsync<
                ApiGetResponse<Guid>
            >();

            return jsonLessonTypeId!.Content;
        }

        public static async Task<Guid> SetupLesson(
            HttpClient _client,
            CreateLessonDTO lessonRequest
        )
        {
            _client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue(
                "Bearer",
                TestAuthorization.GetToken("Admin", "Bob", new Guid())
            );

            var lesson = await _client.PostAsJsonAsync("/api/Lesson", lessonRequest);
            var jsonLessonId = await lesson.Content.ReadFromJsonAsync<ApiGetResponse<Guid>>();

            return jsonLessonId!.Content;
        }

        public static async Task<Guid> SetupLessonBlock(
            HttpClient _client,
            CreateLessonBlockDTO lessonBlockRequest
        )
        {
            _client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue(
                "Bearer",
                TestAuthorization.GetToken("Admin", "Bob", new Guid())
            );

            var lessonBlock = await _client.PostAsJsonAsync("/api/LessonBlock", lessonBlockRequest);
            var jsonLessonBlockId = await lessonBlock.Content.ReadFromJsonAsync<
                ApiGetResponse<Guid>
            >();

            return jsonLessonBlockId!.Content;
        }
    }
}
