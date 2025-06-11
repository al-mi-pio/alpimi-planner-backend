using System.Net;
using System.Net.Http.Headers;
using AlpimiAPI.Entities.EAvailability.DTO;
using AlpimiAPI.Responses;
using AlpimiAPI.Utilities;
using AlpimiTest.TestSetup;
using AlpimiTest.TestUtilities;
using Xunit;

namespace AlpimiTest.Entities.EAvailability
{
    [Collection("Sequential Tests")]
    public class AvailabilityControllerTest : IAsyncLifetime
    {
        CustomWebApplicationFactory<Program> _factory;
        HttpClient _client;
        Guid userId;
        Guid scheduleId;
        Guid teacherId1;
        Guid teacherId2;

        public AvailabilityControllerTest()
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
            teacherId1 = await DbHelper.SetupTeacher(
                _client,
                MockData.GetCreateTeacherDTODetails(scheduleId)
            );
            teacherId2 = await DbHelper.SetupTeacher(
                _client,
                MockData.GetCreateSecondTeacherDTODetails(scheduleId)
            );
            for (int i = 0; i != 5; i++)
            {
                var dto = MockData.GetCreateLessonPeriodDTODetails(scheduleId);
                dto.Start = dto.Start.AddMinutes(60 * i);
                await DbHelper.SetupLessonPeriod(_client, dto);
            }
        }

        public async Task DisposeAsync()
        {
            await DbHelper.UserCleaner(_client);
        }

        [Fact]
        public async Task AvailabilitySettingsControllerThrowsUnauthorized()
        {
            _client.DefaultRequestHeaders.Authorization = null;

            var response = await _client.DeleteAsync($"/api/Availability/{new Guid()}");
            Assert.Equal(HttpStatusCode.Unauthorized, response.StatusCode);

            response = await _client.PostAsJsonAsync(
                "/api/Availability",
                MockData.GetCreateAvailabilityDTODetails(teacherId1)
            );
            Assert.Equal(HttpStatusCode.Unauthorized, response.StatusCode);

            response = await _client.PatchAsJsonAsync(
                $"/api/Availability/{new Guid()}",
                MockData.GetUpdateLessonDTODetails()
            );
            Assert.Equal(HttpStatusCode.Unauthorized, response.StatusCode);
        }

        [Fact]
        public async Task AvailabilityControllerThrowsTooManyRequests()
        {
            for (int i = 0; i != Configuration.GetPermitLimit(); i++)
            {
                await _client.GetAsync("/api/Availability");
            }
            _client.DefaultRequestHeaders.Authorization = null;

            var response = await _client.DeleteAsync($"/api/Availability/{new Guid()}");
            Assert.Equal(HttpStatusCode.TooManyRequests, response.StatusCode);

            response = await _client.PostAsJsonAsync(
                "/api/Availability",
                MockData.GetCreateAvailabilityDTODetails(teacherId1)
            );
            Assert.Equal(HttpStatusCode.TooManyRequests, response.StatusCode);

            var query = $"?groupId={new Guid()}";
            response = await _client.GetAsync($"/api/Availability{query}");
            Assert.Equal(HttpStatusCode.TooManyRequests, response.StatusCode);

            response = await _client.PatchAsJsonAsync(
                $"/api/Availability/{new Guid()}",
                MockData.GetUpdateLessonDTODetails()
            );
            Assert.Equal(HttpStatusCode.TooManyRequests, response.StatusCode);
        }

        [Fact]
        public async Task AvailabilityIsCreated()
        {
            var availabilityRequest = MockData.GetCreateAvailabilityDTODetails(teacherId1);
            _client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue(
                "Bearer",
                TestAuthorization.GetToken("Admin", "User", userId)
            );

            var response = await _client.PostAsJsonAsync("/api/Availability", availabilityRequest);
            Assert.Equal(HttpStatusCode.OK, response.StatusCode);

            var query = $"?id={teacherId1}";
            response = await _client.GetAsync($"/api/Availability{query}");
            var stringResponse = await response.Content.ReadAsStringAsync();
            Assert.Contains(teacherId1.ToString(), stringResponse);
        }

        [Fact]
        public async Task CreateAvailabilityIsUndone()
        {
            var availabilityRequest = MockData.GetCreateAvailabilityDTODetails(teacherId1);
            _client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue(
                "Bearer",
                TestAuthorization.GetToken("Admin", "User", userId)
            );

            await _client.PostAsJsonAsync("/api/Availability", availabilityRequest);
            await _client.PatchAsJsonAsync($"/api/History/undo/{scheduleId}", "");

            var query = $"?id={teacherId1}";
            var response = await _client.GetAsync($"/api/Availability{query}");
            var stringResponse = await response.Content.ReadAsStringAsync();
            Assert.DoesNotContain(teacherId1.ToString(), stringResponse);
        }

        [Fact]
        public async Task CreateAvailabilityIsRedone()
        {
            var availabilityRequest = MockData.GetCreateAvailabilityDTODetails(teacherId1);
            _client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue(
                "Bearer",
                TestAuthorization.GetToken("Admin", "User", userId)
            );

            await _client.PostAsJsonAsync("/api/Availability", availabilityRequest);
            await _client.PatchAsJsonAsync($"/api/History/undo/{scheduleId}", "");
            await _client.PatchAsJsonAsync($"/api/History/redo/{scheduleId}", "");

            var query = $"?id={teacherId1}";
            var response = await _client.GetAsync($"/api/Availability{query}");
            var stringResponse = await response.Content.ReadAsStringAsync();
            Assert.Contains(teacherId1.ToString(), stringResponse);
        }

        [Fact]
        public async Task AvailabilityIsDeleted()
        {
            var availabilityRequest = MockData.GetCreateAvailabilityDTODetails(teacherId1);
            var availabilityId = await DbHelper.SetupAvailability(_client, availabilityRequest);
            _client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue(
                "Bearer",
                TestAuthorization.GetToken("Admin", "User", new Guid())
            );

            var response = await _client.DeleteAsync($"/api/Availability/{availabilityId}");
            Assert.Equal(HttpStatusCode.NoContent, response.StatusCode);

            var query = $"?id={teacherId1}";
            response = await _client.GetAsync($"/api/Availability{query}");
            var stringResponse = await response.Content.ReadAsStringAsync();
            Assert.DoesNotContain(Convert.ToString(availabilityRequest.TeacherId)!, stringResponse);
        }

        [Fact]
        public async Task DeleteAvailabilityIsUndone()
        {
            var availabilityRequest = MockData.GetCreateAvailabilityDTODetails(teacherId1);
            var availabilityId = await DbHelper.SetupAvailability(_client, availabilityRequest);
            _client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue(
                "Bearer",
                TestAuthorization.GetToken("Admin", "User", new Guid())
            );

            await _client.DeleteAsync($"/api/Availability/{availabilityId}");
            await _client.PatchAsJsonAsync($"/api/History/undo/{scheduleId}", "");

            var query = $"?id={teacherId1}";
            var response = await _client.GetAsync($"/api/Availability{query}");
            var stringResponse = await response.Content.ReadAsStringAsync();
            Assert.Contains(Convert.ToString(availabilityRequest.TeacherId)!, stringResponse);
        }

        [Fact]
        public async Task DeleteAvailabilityIsRedone()
        {
            var availabilityRequest = MockData.GetCreateAvailabilityDTODetails(teacherId1);
            var availabilityId = await DbHelper.SetupAvailability(_client, availabilityRequest);
            _client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue(
                "Bearer",
                TestAuthorization.GetToken("Admin", "User", new Guid())
            );

            await _client.DeleteAsync($"/api/Availability/{availabilityId}");
            await _client.PatchAsJsonAsync($"/api/History/undo/{scheduleId}", "");
            await _client.PatchAsJsonAsync($"/api/History/redo/{scheduleId}", "");

            var query = $"?id={teacherId1}";
            var response = await _client.GetAsync($"/api/Availability{query}");
            var stringResponse = await response.Content.ReadAsStringAsync();
            Assert.DoesNotContain(Convert.ToString(availabilityRequest.TeacherId)!, stringResponse);
        }

        [Fact]
        public async Task UpdateAvailabilityReturnsUpdatedAvailability()
        {
            var availabilityUpdateRequest = MockData.GetUpdateAvailabilityDTODetails();

            _client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue(
                "Bearer",
                TestAuthorization.GetToken("Admin", "Bob", userId)
            );
            var availabilityId = await DbHelper.SetupAvailability(
                _client,
                MockData.GetCreateAvailabilityDTODetails(teacherId1)
            );

            var response = await _client.PatchAsJsonAsync(
                $"/api/Availability/{availabilityId}",
                availabilityUpdateRequest
            );
            var jsonResponse = await response.Content.ReadFromJsonAsync<
                ApiGetResponse<AvailabilityDTO>
            >();

            Assert.Equal(availabilityUpdateRequest.WeekDay, jsonResponse!.Content.WeekDay);
        }

        [Fact]
        public async Task UpdateAvailabilityThrowsNotFoundErrorWhenWrongIdIsGiven()
        {
            var availabilityUpdateRequest = MockData.GetUpdateAvailabilityDTODetails();
            _client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue(
                "Bearer",
                TestAuthorization.GetToken("Admin", "Bob", userId)
            );
            await DbHelper.SetupAvailability(
                _client,
                MockData.GetCreateAvailabilityDTODetails(teacherId1)
            );

            var response = await _client.PatchAsJsonAsync(
                $"/api/Availability/{new Guid()}",
                availabilityUpdateRequest
            );

            Assert.Equal(HttpStatusCode.NotFound, response.StatusCode);
        }

        [Fact]
        public async Task UpdateAvailabilityThrowsNotFoundErrorWhenWrongUserAttemptsUpdate()
        {
            var availabilityUpdateRequest = MockData.GetUpdateAvailabilityDTODetails();
            var availabilityId = await DbHelper.SetupAvailability(
                _client,
                MockData.GetCreateAvailabilityDTODetails(teacherId1)
            );
            _client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue(
                "Bearer",
                TestAuthorization.GetToken("User", "Bob", new Guid())
            );

            var response = await _client.PatchAsJsonAsync(
                $"/api/Availability/{availabilityId}",
                availabilityUpdateRequest
            );

            Assert.Equal(HttpStatusCode.NotFound, response.StatusCode);
        }

        [Fact]
        public async Task UpdateAvailabilityIsUndone()
        {
            var availabilityUpdateRequest = MockData.GetUpdateAvailabilityDTODetails();
            var dto = MockData.GetCreateAvailabilityDTODetails(teacherId1);

            _client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue(
                "Bearer",
                TestAuthorization.GetToken("Admin", "Bob", userId)
            );
            var availabilityId = await DbHelper.SetupAvailability(_client, dto);

            await _client.PatchAsJsonAsync(
                $"/api/Availability/{availabilityId}",
                availabilityUpdateRequest
            );
            await _client.PatchAsJsonAsync($"/api/History/undo/{scheduleId}", "");

            var query = $"?id={teacherId1}";
            var response = await _client.GetAsync($"/api/Availability{query}");
            var stringResponse = await response.Content.ReadAsStringAsync();

            Assert.Contains("\"weekDay\":" + dto.WeekDay.ToString(), stringResponse);
            Assert.DoesNotContain(
                "\"weekDay\":" + availabilityUpdateRequest.WeekDay.ToString()!,
                stringResponse
            );
        }

        [Fact]
        public async Task UpdateAvailabilityIsRedone()
        {
            var availabilityUpdateRequest = MockData.GetUpdateAvailabilityDTODetails();
            _client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue(
                "Bearer",
                TestAuthorization.GetToken("Admin", "Bob", userId)
            );
            var availabilityId = await DbHelper.SetupAvailability(
                _client,
                MockData.GetCreateAvailabilityDTODetails(teacherId1)
            );

            await _client.PatchAsJsonAsync(
                $"/api/Availability/{availabilityId}",
                availabilityUpdateRequest
            );
            await _client.PatchAsJsonAsync($"/api/History/undo/{scheduleId}", "");
            await _client.PatchAsJsonAsync($"/api/History/redo/{scheduleId}", "");

            var query = $"?id={teacherId1}";
            var response = await _client.GetAsync($"/api/Availability{query}");
            var stringResponse = await response.Content.ReadAsStringAsync();

            Assert.Contains(
                "\"weekDay\":" + availabilityUpdateRequest.WeekDay.ToString()!,
                stringResponse
            );
        }

        [Fact]
        public async Task GetAllAvailabilitysReturnsAvailabilitysFromTeacherIfTeacherIdIsProvided()
        {
            _client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue(
                "Bearer",
                TestAuthorization.GetToken("Admin", "Bob", userId)
            );
            await DbHelper.SetupAvailability(
                _client,
                MockData.GetCreateAvailabilityDTODetails(teacherId1)
            );
            await DbHelper.SetupAvailability(
                _client,
                MockData.GetCreateSecondAvailabilityDTODetails(teacherId2)
            );

            var query = $"?id={teacherId1}";
            var response = await _client.GetAsync($"/api/Availability{query}");
            var stringResponse = await response.Content.ReadAsStringAsync();

            Assert.Contains(teacherId1.ToString(), stringResponse);
            Assert.DoesNotContain(teacherId2.ToString(), stringResponse);
        }

        [Fact]
        public async Task GetAllAvailabilitysReturnsEmptyContentWhenWrongIdIsGiven()
        {
            _client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue(
                "Bearer",
                TestAuthorization.GetToken("Admin", "Bob", userId)
            );
            await DbHelper.SetupAvailability(
                _client,
                MockData.GetCreateAvailabilityDTODetails(teacherId1)
            );
            await DbHelper.SetupAvailability(
                _client,
                MockData.GetCreateSecondAvailabilityDTODetails(teacherId2)
            );

            var query = $"?id={new Guid()}";
            var response = await _client.GetAsync($"/api/Availability{query}");
            var stringResponse = await response.Content.ReadAsStringAsync();

            Assert.DoesNotContain(teacherId1.ToString(), stringResponse);
            Assert.DoesNotContain(teacherId2.ToString(), stringResponse);
        }

        [Fact]
        public async Task GetAllAvailabilitysReturnsEmptyContentWhenWrongUserAttemptsGet()
        {
            await DbHelper.SetupAvailability(
                _client,
                MockData.GetCreateAvailabilityDTODetails(teacherId1)
            );
            await DbHelper.SetupAvailability(
                _client,
                MockData.GetCreateSecondAvailabilityDTODetails(teacherId2)
            );

            _client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue(
                "Bearer",
                TestAuthorization.GetToken("User", "Bob", new Guid())
            );

            var query = $"?id={teacherId1}";
            var response = await _client.GetAsync($"/api/Availability{query}");
            var stringResponse = await response.Content.ReadAsStringAsync();

            Assert.DoesNotContain(teacherId1.ToString(), stringResponse);
            Assert.DoesNotContain(teacherId2.ToString(), stringResponse);
        }
    }
}
