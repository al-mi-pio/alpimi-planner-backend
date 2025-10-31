using System.Diagnostics;
using System.Net;
using System.Net.Http.Headers;
using AlpimiAPI.Entities.EData.DTO;
using AlpimiAPI.Entities.EDayOff.DTO;
using AlpimiAPI.Responses;
using AlpimiAPI.Utilities;
using AlpimiTest.TestSetup;
using AlpimiTest.TestUtilities;
using Xunit;

namespace AlpimiTest.Entities.EData
{
    [Collection("Sequential Tests")]
    public class DataControllerTest : IAsyncLifetime
    {
        CustomWebApplicationFactory<Program> _factory;
        HttpClient _client;
        Guid userId;
        Guid scheduleId;

        public DataControllerTest()
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
        public async Task DayOffControllerThrowsUnauthorized()
        {
            _client.DefaultRequestHeaders.Authorization = null;

            var response = await _client.PostAsJsonAsync(
                "/api/Data/import",
                MockXMLs.GetCorrectXML()
            );
            Assert.Equal(HttpStatusCode.Unauthorized, response.StatusCode);

            response = await _client.GetAsync($"/api/Data/export/{new Guid()}");
            Assert.Equal(HttpStatusCode.Unauthorized, response.StatusCode);
        }

        [Fact]
        public async Task DayOffControllerThrowsTooManyRequests()
        {
            _client.DefaultRequestHeaders.Authorization = null;

            for (int i = 0; i != Configuration.GetLoosePermitLimit(); i++)
                await _client.PostAsJsonAsync("/api/Data/import", MockXMLs.GetCorrectXML());
            var response = await _client.PostAsJsonAsync(
                "/api/Data/import",
                MockXMLs.GetCorrectXML()
            );
            Assert.Equal(HttpStatusCode.TooManyRequests, response.StatusCode);

            for (int i = 0; i != Configuration.GetLoosePermitLimit(); i++)
                await _client.GetAsync($"/api/Data/export/{new Guid()}");
            response = await _client.GetAsync($"/api/Data/export/{new Guid()}");
            Assert.Equal(HttpStatusCode.TooManyRequests, response.StatusCode);
        }

        [Fact]
        public async Task ImportDataCorrectlyImportsData()
        {
            var dto = new ImportDataDTO()
            {
                Payload = MockXMLs.GetCorrectXML(),
                ScheduleId = scheduleId
            };
            _client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue(
                "Bearer",
                TestAuthorization.GetToken("Admin", "User", userId)
            );

            var response = await _client.PostAsJsonAsync("/api/Data/import", dto);
            Assert.Equal(HttpStatusCode.OK, response.StatusCode);

            var query = $"?scheduleId={scheduleId}";
            response = await _client.GetAsync($"/api/LessonPeriod{query}");
            Assert.Equal(HttpStatusCode.OK, response.StatusCode);
            var stringResponse = await response.Content.ReadAsStringAsync();
            Assert.Contains("Id", stringResponse);
            response = await _client.GetAsync($"/api/DayOff{query}");
            Assert.Equal(HttpStatusCode.OK, response.StatusCode);
            stringResponse = await response.Content.ReadAsStringAsync();
            Assert.Contains("Id", stringResponse);
            response = await _client.GetAsync($"/api/ClassroomType{query}");
            Assert.Equal(HttpStatusCode.OK, response.StatusCode);
            stringResponse = await response.Content.ReadAsStringAsync();
            Assert.Contains("Id", stringResponse);
            response = await _client.GetAsync($"/api/LessonType{query}");
            Assert.Equal(HttpStatusCode.OK, response.StatusCode);
            stringResponse = await response.Content.ReadAsStringAsync();
            Assert.Contains("Id", stringResponse);
            response = await _client.GetAsync($"/api/Teacher{query}");
            Assert.Equal(HttpStatusCode.OK, response.StatusCode);
            stringResponse = await response.Content.ReadAsStringAsync();
            Assert.Contains("Id", stringResponse);
            response = await _client.GetAsync($"/api/Group{query}");
            Assert.Equal(HttpStatusCode.OK, response.StatusCode);
            stringResponse = await response.Content.ReadAsStringAsync();
            Assert.Contains("Id", stringResponse);
            response = await _client.GetAsync($"/api/Subgroup{query}");
            Assert.Equal(HttpStatusCode.OK, response.StatusCode);
            stringResponse = await response.Content.ReadAsStringAsync();
            Assert.Contains("Id", stringResponse);
            response = await _client.GetAsync($"/api/Classroom{query}");
            Assert.Equal(HttpStatusCode.OK, response.StatusCode);
            stringResponse = await response.Content.ReadAsStringAsync();
            Assert.Contains("Id", stringResponse);
            response = await _client.GetAsync($"/api/Lesson{query}");
            Assert.Equal(HttpStatusCode.OK, response.StatusCode);
            stringResponse = await response.Content.ReadAsStringAsync();
            Assert.Contains("Id", stringResponse);
            response = await _client.GetAsync($"/api/Availability{query}");
            Assert.Equal(HttpStatusCode.OK, response.StatusCode);
            stringResponse = await response.Content.ReadAsStringAsync();
            Assert.Contains("Id", stringResponse);
            response = await _client.GetAsync($"/api/Student{query}");
            Assert.Equal(HttpStatusCode.OK, response.StatusCode);
            stringResponse = await response.Content.ReadAsStringAsync();
            Assert.Contains("Id", stringResponse);
        }

        [Fact]
        public async Task ExportDataCorrectlyExportsData()
        {
            var dto = new ImportDataDTO()
            {
                Payload = MockXMLs.GetCorrectXML(),
                ScheduleId = scheduleId
            };
            _client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue(
                "Bearer",
                TestAuthorization.GetToken("Admin", "User", userId)
            );
            var response = await _client.PostAsJsonAsync("/api/Data/import", dto);
            Assert.Equal(HttpStatusCode.OK, response.StatusCode);

            response = await _client.GetAsync($"/api/Data/export/{scheduleId}");
            Assert.Equal(HttpStatusCode.OK, response.StatusCode);

            var stringResponse = await response.Content.ReadAsStringAsync();
            Debug.WriteLine(stringResponse);
            Assert.Contains("Lesson period", stringResponse);
            Assert.Contains("Day off", stringResponse);
            Assert.Contains("Classroom type", stringResponse);
            Assert.Contains("Lesson type", stringResponse);
            Assert.Contains("Teacher", stringResponse);
            Assert.Contains("Group", stringResponse);
            Assert.Contains("Subgroup", stringResponse);
            Assert.Contains("Classroom", stringResponse);
            Assert.Contains("Lesson", stringResponse);
            Assert.Contains("Teacher's availability", stringResponse);
            Assert.Contains("Student", stringResponse);
        }
    }
}
