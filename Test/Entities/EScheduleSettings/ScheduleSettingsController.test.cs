﻿using System.Net;
using System.Net.Http.Headers;
using AlpimiAPI.Entities.EScheduleSettings;
using AlpimiAPI.Responses;
using AlpimiAPI.Settings;
using AlpimiTest.TestSetup;
using AlpimiTest.TestUtilities;
using Xunit;

namespace AlpimiTest.Entities.EScheduleSettings
{
    [Collection("Sequential Tests")]
    public class ScheduleSettingsControllerTest : IAsyncLifetime
    {
        CustomWebApplicationFactory<Program> _factory;
        HttpClient _client;
        Guid userId;
        Guid scheduleId;

        public ScheduleSettingsControllerTest()
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
        public async Task UpdateScheduleSettingsReturnsUpdatedSchedule()
        {
            var updateScheduleSettings = MockData.GetUpdateScheduleSettingsDTO();

            _client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue(
                "Bearer",
                TestAuthorization.GetToken("Admin", "User", new Guid())
            );

            var response = await _client.PatchAsJsonAsync(
                $"/api/ScheduleSettings/{scheduleId}",
                updateScheduleSettings
            );
            var jsonResponse = await response.Content.ReadFromJsonAsync<
                ApiGetResponse<ScheduleSettings>
            >();

            Assert.Equal(
                jsonResponse!.Content.SchoolYearStart,
                updateScheduleSettings.SchoolYearStart
            );
            Assert.Equal(jsonResponse.Content.SchoolYearEnd, updateScheduleSettings.SchoolYearEnd);
            Assert.Equal(jsonResponse.Content.SchoolHour, updateScheduleSettings.SchoolHour);
        }

        [Fact]
        public async Task UpdateScheduleSettingsThrowsNotFoundErrorWhenWrongIdIsGiven()
        {
            _client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue(
                "Bearer",
                TestAuthorization.GetToken("Admin", "User", new Guid())
            );

            var response = await _client.PatchAsJsonAsync(
                $"/api/ScheduleSettings/{new Guid()}",
                MockData.GetUpdateScheduleSettingsDTO()
            );
            Assert.Equal(HttpStatusCode.NotFound, response.StatusCode);
        }

        [Fact]
        public async Task UpdateScheduleSettingsThrowsNotFoundErrorWhenWrongUserAttemptsUpdate()
        {
            _client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue(
                "Bearer",
                TestAuthorization.GetToken("User", "User", new Guid())
            );

            var response = await _client.PatchAsJsonAsync(
                $"/api/ScheduleSettings/{scheduleId}",
                MockData.GetUpdateScheduleSettingsDTO()
            );
            Assert.Equal(HttpStatusCode.NotFound, response.StatusCode);
        }

        [Fact]
        public async Task GetScheduleSettingsByScheduleIdReturnsScheduleSettings()
        {
            var scheduleSettings = MockData.GetCreateScheduleDTODetails();

            _client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue(
                "Bearer",
                TestAuthorization.GetToken("Admin", "User", new Guid())
            );

            var response = await _client.GetAsync($"/api/ScheduleSettings/bySchedule/{scheduleId}");
            var jsonResponse = await response.Content.ReadFromJsonAsync<
                ApiGetResponse<ScheduleSettings>
            >();

            Assert.Equal(jsonResponse!.Content.SchoolYearStart, scheduleSettings.SchoolYearStart);
            Assert.Equal(jsonResponse.Content.SchoolYearEnd, scheduleSettings.SchoolYearEnd);
            Assert.Equal(jsonResponse.Content.SchoolHour, scheduleSettings.SchoolHour);
        }

        [Fact]
        public async Task GetScheduleSettingsByScheduleIdThrowsNotFoundErrorWhenWrongUserAttemptsUpdate()
        {
            _client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue(
                "Bearer",
                TestAuthorization.GetToken("User", "User", new Guid())
            );

            var response = await _client.GetAsync($"/api/ScheduleSettings/bySchedule/{scheduleId}");

            Assert.Equal(HttpStatusCode.NotFound, response.StatusCode);
        }

        [Fact]
        public async Task GetScheduleSettingsByScheduleIdThrowsNotFoundErrorWhenWrongIdIsGiven()
        {
            _client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue(
                "Bearer",
                TestAuthorization.GetToken("Admin", "User", new Guid())
            );

            var response = await _client.GetAsync($"/api/ScheduleSettings/bySchedule/{new Guid()}");

            Assert.Equal(HttpStatusCode.NotFound, response.StatusCode);
        }

        [Fact]
        public async Task GetScheduleSettingsReturnsScheduleSettings()
        {
            var scheduleSettings = MockData.GetCreateScheduleDTODetails();
            var scheduleSettingsId = await _client.GetAsync(
                $"/api/ScheduleSettings/bySchedule/{scheduleId}"
            );
            var jsonScheduleSettingsId = await scheduleSettingsId.Content.ReadFromJsonAsync<
                ApiGetResponse<ScheduleSettings>
            >();

            _client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue(
                "Bearer",
                TestAuthorization.GetToken("Admin", "User", new Guid())
            );

            var response = await _client.GetAsync(
                $"/api/ScheduleSettings/{jsonScheduleSettingsId!.Content.Id}"
            );
            var jsonResponse = await response.Content.ReadFromJsonAsync<
                ApiGetResponse<ScheduleSettings>
            >();

            Assert.Equal(jsonResponse!.Content.SchoolYearStart, scheduleSettings.SchoolYearStart);
            Assert.Equal(jsonResponse.Content.SchoolYearEnd, scheduleSettings.SchoolYearEnd);
            Assert.Equal(jsonResponse.Content.SchoolHour, scheduleSettings.SchoolHour);
        }

        [Fact]
        public async Task GetScheduleSettingsThrowsNotFoundErrorWhenWrongUserAttemptsUpdate()
        {
            var scheduleSettingsId = await _client.GetAsync(
                $"/api/ScheduleSettings/bySchedule/{scheduleId}"
            );
            var jsonScheduleSettingsId = await scheduleSettingsId.Content.ReadFromJsonAsync<
                ApiGetResponse<ScheduleSettings>
            >();

            _client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue(
                "Bearer",
                TestAuthorization.GetToken("User", "User", new Guid())
            );

            var response = await _client.GetAsync(
                $"/api/ScheduleSettings/{jsonScheduleSettingsId!.Content.Id}"
            );

            Assert.Equal(HttpStatusCode.NotFound, response.StatusCode);
        }

        [Fact]
        public async Task GetScheduleSettingsThrowsNotFoundErrorWhenWrongIdIsGiven()
        {
            _client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue(
                "Bearer",
                TestAuthorization.GetToken("Admin", "User", new Guid())
            );

            var response = await _client.GetAsync($"/api/ScheduleSettings/{new Guid()}");

            Assert.Equal(HttpStatusCode.NotFound, response.StatusCode);
        }

        [Fact]
        public async Task ScheduleSettingsControllerThrowsUnauthorized()
        {
            _client.DefaultRequestHeaders.Authorization = null;

            var response = await _client.GetAsync($"/api/ScheduleSettings/bySchedule/{new Guid()}");
            Assert.Equal(HttpStatusCode.Unauthorized, response.StatusCode);

            response = await _client.PatchAsJsonAsync(
                $"/api/ScheduleSettings/{new Guid()}",
                MockData.GetUpdateScheduleSettingsDTO()
            );
            Assert.Equal(HttpStatusCode.Unauthorized, response.StatusCode);

            response = await _client.GetAsync($"/api/ScheduleSettings/{new Guid()}");
            Assert.Equal(HttpStatusCode.Unauthorized, response.StatusCode);
        }

        [Fact]
        public async Task ScheduleControllerThrowsTooManyRequests()
        {
            for (int i = 0; i != RateLimiterSettings.permitLimit; i++)
            {
                await _client.GetAsync($"/api/ScheduleSettings/{new Guid()}");
            }

            var response = await _client.GetAsync($"/api/ScheduleSettings/bySchedule/{new Guid()}");
            Assert.Equal(HttpStatusCode.TooManyRequests, response.StatusCode);

            response = await _client.PatchAsJsonAsync(
                $"/api/ScheduleSettings/{new Guid()}",
                MockData.GetUpdateScheduleSettingsDTO()
            );
            Assert.Equal(HttpStatusCode.TooManyRequests, response.StatusCode);

            response = await _client.GetAsync($"/api/ScheduleSettings/{new Guid()}");
            Assert.Equal(HttpStatusCode.TooManyRequests, response.StatusCode);
        }
    }
}
