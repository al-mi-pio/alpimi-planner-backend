using System.Net;
using System.Net.Http.Headers;
using AlpimiAPI.Entities.ESchedule;
using AlpimiAPI.Entities.ESchedule.DTO;
using AlpimiAPI.Entities.EUser;
using AlpimiAPI.Entities.EUser.DTO;
using AlpimiAPI.Responses;
using AlpimiTest.TestUtilities;
using Sprache;
using Xunit;

namespace AlpimiTest.Entities.ESchedule
{
    [Collection("Sequential Tests")]
    public class ScheduleControllerTest
    {
        CustomWebApplicationFactory<Program> _factory;
        HttpClient _client;

        public ScheduleControllerTest()
        {
            _factory = new CustomWebApplicationFactory<Program>();
            _client = _factory.CreateClient();
        }

        [Fact]
        public async Task DeleteScheduleReturnsNoContentStatusCode()
        {
            var user = MockData.GetUserDetails();
            _client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue(
                "Bearer",
                TestAuthorization.GetToken("Admin", user.Login, user.Id)
            );
            var response = await _client.DeleteAsync(
                "/api/Schedule/b70eda99-ed0a-4c06-bc65-44166ce58bb0"
            );
            Assert.Equal(HttpStatusCode.NoContent, response.StatusCode);
        }

        [Fact]
        public async Task DeleteScheduleThrowsUnothorizedErrorWhenNoTokenIsGiven()
        {
            var user = MockData.GetUserDetails();

            var response = await _client.DeleteAsync(
                "/api/Schedule/b70eda99-ed0a-4c06-bc65-44166ce58bb0"
            );
            Assert.Equal(HttpStatusCode.Unauthorized, response.StatusCode);
        }

        [Fact]
        public async Task CreateScheduleReturnsOkStatusCode()
        {
            var schedule = MockData.GetScheduleDetails();
            var userRequest = new CreateUserDTO
            {
                Login = schedule.User.Login,
                CustomURL = schedule.User.CustomURL!,
                Password = "sssSSS1!"
            };
            var scheduleRequest = new CreateScheduleDTO
            {
                Name = schedule.Name,
                SchoolHour = schedule.SchoolHour
            };

            _client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue(
                "Bearer",
                TestAuthorization.GetToken("Admin", schedule.User.Login, schedule.User.Id)
            );

            var id = await _client.PostAsJsonAsync("/api/User", userRequest);
            var jsonId = await id.Content.ReadFromJsonAsync<ApiGetResponse<Guid>>();

            _client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue(
                "Bearer",
                TestAuthorization.GetToken("Admin", schedule.User.Login, jsonId!.Content)
            );

            var response = await _client.PostAsJsonAsync("/api/Schedule", scheduleRequest);

            await _client.DeleteAsync($"/api/User/{jsonId.Content}");

            Assert.Equal(HttpStatusCode.OK, response.StatusCode);
        }

        [Fact]
        public async Task CreateScheduleThrowsUnothorizedErrorWhenNoTokenIsGiven()
        {
            var schedule = MockData.GetScheduleDetails();
            var userRequest = new CreateUserDTO
            {
                Login = schedule.User.Login,
                CustomURL = schedule.User.CustomURL!,
                Password = "sssSSS1!"
            };
            var scheduleRequest = new CreateScheduleDTO
            {
                Name = schedule.Name,
                SchoolHour = schedule.SchoolHour
            };

            _client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue(
                "Bearer",
                TestAuthorization.GetToken("Admin", schedule.User.Login, schedule.User.Id)
            );

            var id = await _client.PostAsJsonAsync("/api/User", userRequest);
            var jsonId = await id.Content.ReadFromJsonAsync<ApiGetResponse<Guid>>();

            _client.DefaultRequestHeaders.Authorization = null;

            var response = await _client.PostAsJsonAsync("/api/Schedule", scheduleRequest);

            _client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue(
                "Bearer",
                TestAuthorization.GetToken("Admin", schedule.User.Login, schedule.User.Id)
            );
            await _client.DeleteAsync($"/api/User/{jsonId!.Content}");

            Assert.Equal(HttpStatusCode.Unauthorized, response.StatusCode);
        }

        [Fact]
        public async Task UpdateScheduleReturnsUpdatedSchedule()
        {
            var schedule = MockData.GetScheduleDetails();
            var userRequest = new CreateUserDTO
            {
                Login = schedule.User.Login,
                CustomURL = schedule.User.CustomURL!,
                Password = "sssSSS1!"
            };
            var scheduleRequest = new CreateScheduleDTO
            {
                Name = schedule.Name,
                SchoolHour = schedule.SchoolHour
            };
            var scheduleUpdateRequest = new UpdateScheduleDTO { Name = "New_Name" };

            _client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue(
                "Bearer",
                TestAuthorization.GetToken("Admin", schedule.User.Login, schedule.User.Id)
            );

            var userId = await _client.PostAsJsonAsync("/api/User", userRequest);
            var jsonUserId = await userId.Content.ReadFromJsonAsync<ApiGetResponse<Guid>>();

            _client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue(
                "Bearer",
                TestAuthorization.GetToken("Admin", schedule.User.Login, jsonUserId!.Content)
            );

            var scheduleId = await _client.PostAsJsonAsync("/api/Schedule", scheduleRequest);
            var jsonScheduleId = await scheduleId.Content.ReadFromJsonAsync<ApiGetResponse<Guid>>();

            var response = await _client.PatchAsJsonAsync(
                $"/api/Schedule/{jsonScheduleId!.Content}",
                scheduleUpdateRequest
            );

            var jsonResponse = await response.Content.ReadFromJsonAsync<ApiGetResponse<Schedule>>();

            await _client.DeleteAsync($"/api/User/{jsonUserId.Content}");

            Assert.Equal(scheduleUpdateRequest.Name, jsonResponse!.Content.Name);
            Assert.Equal(scheduleRequest.SchoolHour, jsonResponse!.Content.SchoolHour);
        }

        [Fact]
        public async Task UpdateScheduleThrowsNotFoundErrorWhenWrongIdIsGiven()
        {
            var schedule = MockData.GetScheduleDetails();
            var userRequest = new CreateUserDTO
            {
                Login = schedule.User.Login,
                CustomURL = schedule.User.CustomURL!,
                Password = "sssSSS1!"
            };
            var scheduleRequest = new CreateScheduleDTO
            {
                Name = schedule.Name,
                SchoolHour = schedule.SchoolHour
            };
            var scheduleUpdateRequest = new UpdateScheduleDTO { Name = "New_Name" };

            _client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue(
                "Bearer",
                TestAuthorization.GetToken("Admin", schedule.User.Login, schedule.User.Id)
            );

            var userId = await _client.PostAsJsonAsync("/api/User", userRequest);
            var jsonUserId = await userId.Content.ReadFromJsonAsync<ApiGetResponse<Guid>>();

            _client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue(
                "Bearer",
                TestAuthorization.GetToken("Admin", schedule.User.Login, jsonUserId!.Content)
            );

            var scheduleId = await _client.PostAsJsonAsync("/api/Schedule", scheduleRequest);
            var jsonScheduleId = await scheduleId.Content.ReadFromJsonAsync<ApiGetResponse<Guid>>();

            var response = await _client.PatchAsJsonAsync(
                $"/api/Schedule/{Guid.NewGuid()}",
                scheduleUpdateRequest
            );

            await _client.DeleteAsync($"/api/User/{jsonUserId.Content}");

            Assert.Equal(HttpStatusCode.NotFound, response.StatusCode);
        }

        [Fact]
        public async Task UpdateScheduleThrowsNotFoundErrorWhenWrongUserAttemptsUpdate()
        {
            var schedule = MockData.GetScheduleDetails();
            var userRequest = new CreateUserDTO
            {
                Login = schedule.User.Login,
                CustomURL = schedule.User.CustomURL!,
                Password = "sssSSS1!"
            };
            var scheduleRequest = new CreateScheduleDTO
            {
                Name = schedule.Name,
                SchoolHour = schedule.SchoolHour
            };
            var scheduleUpdateRequest = new UpdateScheduleDTO { Name = "New_Name" };

            _client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue(
                "Bearer",
                TestAuthorization.GetToken("Admin", schedule.User.Login, schedule.User.Id)
            );

            var userId = await _client.PostAsJsonAsync("/api/User", userRequest);
            var jsonUserId = await userId.Content.ReadFromJsonAsync<ApiGetResponse<Guid>>();

            _client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue(
                "Bearer",
                TestAuthorization.GetToken("Admin", schedule.User.Login, jsonUserId!.Content)
            );

            var scheduleId = await _client.PostAsJsonAsync("/api/Schedule", scheduleRequest);
            var jsonScheduleId = await scheduleId.Content.ReadFromJsonAsync<ApiGetResponse<Guid>>();

            _client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue(
                "Bearer",
                TestAuthorization.GetToken("User", schedule.User.Login, new Guid())
            );
            var response = await _client.PatchAsJsonAsync(
                $"/api/Schedule/{jsonScheduleId!.Content}",
                scheduleUpdateRequest
            );

            _client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue(
                "Bearer",
                TestAuthorization.GetToken("Admin", schedule.User.Login, jsonUserId!.Content)
            );
            await _client.DeleteAsync($"/api/User/{jsonUserId.Content}");

            Assert.Equal(HttpStatusCode.NotFound, response.StatusCode);
        }

        [Fact]
        public async Task UpdateScheduleThrowsUnothorizedErrorWhenNoTokenIsGiven()
        {
            var schedule = MockData.GetScheduleDetails();
            var userRequest = new CreateUserDTO
            {
                Login = schedule.User.Login,
                CustomURL = schedule.User.CustomURL!,
                Password = "sssSSS1!"
            };
            var scheduleRequest = new CreateScheduleDTO
            {
                Name = schedule.Name,
                SchoolHour = schedule.SchoolHour
            };
            var scheduleUpdateRequest = new UpdateScheduleDTO { Name = "New_Name" };

            _client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue(
                "Bearer",
                TestAuthorization.GetToken("Admin", schedule.User.Login, schedule.User.Id)
            );

            var userId = await _client.PostAsJsonAsync("/api/User", userRequest);
            var jsonUserId = await userId.Content.ReadFromJsonAsync<ApiGetResponse<Guid>>();

            _client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue(
                "Bearer",
                TestAuthorization.GetToken("Admin", schedule.User.Login, jsonUserId!.Content)
            );

            var scheduleId = await _client.PostAsJsonAsync("/api/Schedule", scheduleRequest);
            var jsonScheduleId = await scheduleId.Content.ReadFromJsonAsync<ApiGetResponse<Guid>>();

            _client.DefaultRequestHeaders.Authorization = null;
            var response = await _client.PatchAsJsonAsync(
                $"/api/Schedule/{jsonScheduleId!.Content}",
                scheduleUpdateRequest
            );

            _client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue(
                "Bearer",
                TestAuthorization.GetToken("Admin", schedule.User.Login, jsonUserId!.Content)
            );
            await _client.DeleteAsync($"/api/User/{jsonUserId.Content}");

            Assert.Equal(HttpStatusCode.Unauthorized, response.StatusCode);
        }

        [Fact]
        public async Task GetScheduleByNameReturnsSchedule()
        {
            var schedule = MockData.GetScheduleDetails();
            var userRequest = new CreateUserDTO
            {
                Login = schedule.User.Login,
                CustomURL = schedule.User.CustomURL!,
                Password = "sssSSS1!"
            };
            var scheduleRequest = new CreateScheduleDTO
            {
                Name = schedule.Name,
                SchoolHour = schedule.SchoolHour
            };

            _client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue(
                "Bearer",
                TestAuthorization.GetToken("Admin", schedule.User.Login, schedule.User.Id)
            );

            var userId = await _client.PostAsJsonAsync("/api/User", userRequest);
            var jsonUserId = await userId.Content.ReadFromJsonAsync<ApiGetResponse<Guid>>();

            _client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue(
                "Bearer",
                TestAuthorization.GetToken("Admin", schedule.User.Login, jsonUserId!.Content)
            );

            var scheduleId = await _client.PostAsJsonAsync("/api/Schedule", scheduleRequest);
            var jsonScheduleId = await scheduleId.Content.ReadFromJsonAsync<ApiGetResponse<Guid>>();

            var response = await _client.GetAsync($"/api/Schedule/byName/{scheduleRequest.Name}");

            var jsonResponse = await response.Content.ReadFromJsonAsync<ApiGetResponse<Schedule>>();

            await _client.DeleteAsync($"/api/User/{jsonUserId.Content}");

            Assert.Equal(scheduleRequest.Name, jsonResponse!.Content.Name);
            Assert.Equal(scheduleRequest.SchoolHour, jsonResponse!.Content.SchoolHour);
        }

        [Fact]
        public async Task GetScheduleByNameThrowsNotFoundErrorWhenWrongUserAttemptsGet()
        {
            var schedule = MockData.GetScheduleDetails();
            var userRequest = new CreateUserDTO
            {
                Login = schedule.User.Login,
                CustomURL = schedule.User.CustomURL!,
                Password = "sssSSS1!"
            };
            var scheduleRequest = new CreateScheduleDTO
            {
                Name = schedule.Name,
                SchoolHour = schedule.SchoolHour
            };

            _client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue(
                "Bearer",
                TestAuthorization.GetToken("Admin", schedule.User.Login, schedule.User.Id)
            );

            var userId = await _client.PostAsJsonAsync("/api/User", userRequest);
            var jsonUserId = await userId.Content.ReadFromJsonAsync<ApiGetResponse<Guid>>();

            _client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue(
                "Bearer",
                TestAuthorization.GetToken("Admin", schedule.User.Login, jsonUserId!.Content)
            );

            var scheduleId = await _client.PostAsJsonAsync("/api/Schedule", scheduleRequest);
            var jsonScheduleId = await scheduleId.Content.ReadFromJsonAsync<ApiGetResponse<Guid>>();

            _client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue(
                "Bearer",
                TestAuthorization.GetToken("User", schedule.User.Login, new Guid())
            );
            var response = await _client.GetAsync($"/api/Schedule/byName/{scheduleRequest.Name}");

            _client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue(
                "Bearer",
                TestAuthorization.GetToken("Admin", schedule.User.Login, jsonUserId!.Content)
            );
            await _client.DeleteAsync($"/api/User/{jsonUserId.Content}");

            Assert.Equal(HttpStatusCode.NotFound, response.StatusCode);
        }

        [Fact]
        public async Task GetScheduleByNameThrowsNotFoundErrorWhenWrongNameIsGiven()
        {
            var schedule = MockData.GetScheduleDetails();
            var userRequest = new CreateUserDTO
            {
                Login = schedule.User.Login,
                CustomURL = schedule.User.CustomURL!,
                Password = "sssSSS1!"
            };
            var scheduleRequest = new CreateScheduleDTO
            {
                Name = schedule.Name,
                SchoolHour = schedule.SchoolHour
            };

            _client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue(
                "Bearer",
                TestAuthorization.GetToken("Admin", schedule.User.Login, schedule.User.Id)
            );

            var userId = await _client.PostAsJsonAsync("/api/User", userRequest);
            var jsonUserId = await userId.Content.ReadFromJsonAsync<ApiGetResponse<Guid>>();

            _client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue(
                "Bearer",
                TestAuthorization.GetToken("Admin", schedule.User.Login, jsonUserId!.Content)
            );

            var scheduleId = await _client.PostAsJsonAsync("/api/Schedule", scheduleRequest);

            var response = await _client.GetAsync($"/api/Schedule/byName/WrongName");

            await _client.DeleteAsync($"/api/User/{jsonUserId.Content}");

            Assert.Equal(HttpStatusCode.NotFound, response.StatusCode);
        }

        [Fact]
        public async Task GetScheduleThrowsUnothorizedErrorWhenNoTokenIsGiven()
        {
            var schedule = MockData.GetScheduleDetails();
            var userRequest = new CreateUserDTO
            {
                Login = schedule.User.Login,
                CustomURL = schedule.User.CustomURL!,
                Password = "sssSSS1!"
            };
            var scheduleRequest = new CreateScheduleDTO
            {
                Name = schedule.Name,
                SchoolHour = schedule.SchoolHour
            };

            _client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue(
                "Bearer",
                TestAuthorization.GetToken("Admin", schedule.User.Login, schedule.User.Id)
            );

            var userId = await _client.PostAsJsonAsync("/api/User", userRequest);
            var jsonUserId = await userId.Content.ReadFromJsonAsync<ApiGetResponse<Guid>>();

            _client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue(
                "Bearer",
                TestAuthorization.GetToken("Admin", schedule.User.Login, jsonUserId!.Content)
            );

            var scheduleId = await _client.PostAsJsonAsync("/api/Schedule", scheduleRequest);
            var jsonScheduleId = await scheduleId.Content.ReadFromJsonAsync<ApiGetResponse<Guid>>();

            _client.DefaultRequestHeaders.Authorization = null;

            var response = await _client.GetAsync($"/api/Schedule/{new Guid()}");

            _client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue(
                "Bearer",
                TestAuthorization.GetToken("Admin", schedule.User.Login, jsonUserId!.Content)
            );
            await _client.DeleteAsync($"/api/User/{jsonUserId.Content}");

            Assert.Equal(HttpStatusCode.Unauthorized, response.StatusCode);
        }

        [Fact]
        public async Task GetScheduleReturnsSchedule()
        {
            var schedule = MockData.GetScheduleDetails();
            var userRequest = new CreateUserDTO
            {
                Login = schedule.User.Login,
                CustomURL = schedule.User.CustomURL!,
                Password = "sssSSS1!"
            };
            var scheduleRequest = new CreateScheduleDTO
            {
                Name = schedule.Name,
                SchoolHour = schedule.SchoolHour
            };

            _client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue(
                "Bearer",
                TestAuthorization.GetToken("Admin", schedule.User.Login, schedule.User.Id)
            );

            var userId = await _client.PostAsJsonAsync("/api/User", userRequest);
            var jsonUserId = await userId.Content.ReadFromJsonAsync<ApiGetResponse<Guid>>();

            _client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue(
                "Bearer",
                TestAuthorization.GetToken("Admin", schedule.User.Login, jsonUserId!.Content)
            );

            var scheduleId = await _client.PostAsJsonAsync("/api/Schedule", scheduleRequest);
            var jsonScheduleId = await scheduleId.Content.ReadFromJsonAsync<ApiGetResponse<Guid>>();

            var response = await _client.GetAsync($"/api/Schedule/{jsonScheduleId!.Content}");

            var jsonResponse = await response.Content.ReadFromJsonAsync<ApiGetResponse<Schedule>>();

            await _client.DeleteAsync($"/api/User/{jsonUserId.Content}");

            Assert.Equal(scheduleRequest.Name, jsonResponse!.Content.Name);
            Assert.Equal(scheduleRequest.SchoolHour, jsonResponse!.Content.SchoolHour);
        }

        [Fact]
        public async Task GetScheduleThrowsNotFoundErrorWhenWrongUserAttemptsGet()
        {
            var schedule = MockData.GetScheduleDetails();
            var userRequest = new CreateUserDTO
            {
                Login = schedule.User.Login,
                CustomURL = schedule.User.CustomURL!,
                Password = "sssSSS1!"
            };
            var scheduleRequest = new CreateScheduleDTO
            {
                Name = schedule.Name,
                SchoolHour = schedule.SchoolHour
            };

            _client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue(
                "Bearer",
                TestAuthorization.GetToken("Admin", schedule.User.Login, schedule.User.Id)
            );

            var userId = await _client.PostAsJsonAsync("/api/User", userRequest);
            var jsonUserId = await userId.Content.ReadFromJsonAsync<ApiGetResponse<Guid>>();

            _client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue(
                "Bearer",
                TestAuthorization.GetToken("Admin", schedule.User.Login, jsonUserId!.Content)
            );

            var scheduleId = await _client.PostAsJsonAsync("/api/Schedule", scheduleRequest);
            var jsonScheduleId = await scheduleId.Content.ReadFromJsonAsync<ApiGetResponse<Guid>>();

            _client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue(
                "Bearer",
                TestAuthorization.GetToken("User", schedule.User.Login, new Guid())
            );
            var response = await _client.GetAsync($"/api/Schedule/{jsonScheduleId!.Content}");

            _client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue(
                "Bearer",
                TestAuthorization.GetToken("Admin", schedule.User.Login, jsonUserId!.Content)
            );
            await _client.DeleteAsync($"/api/User/{jsonUserId.Content}");

            Assert.Equal(HttpStatusCode.NotFound, response.StatusCode);
        }

        [Fact]
        public async Task GetScheduleThrowsNotFoundErrorWhenWrongNameIsGiven()
        {
            var schedule = MockData.GetScheduleDetails();
            var userRequest = new CreateUserDTO
            {
                Login = schedule.User.Login,
                CustomURL = schedule.User.CustomURL!,
                Password = "sssSSS1!"
            };
            var scheduleRequest = new CreateScheduleDTO
            {
                Name = schedule.Name,
                SchoolHour = schedule.SchoolHour
            };

            _client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue(
                "Bearer",
                TestAuthorization.GetToken("Admin", schedule.User.Login, schedule.User.Id)
            );

            var userId = await _client.PostAsJsonAsync("/api/User", userRequest);
            var jsonUserId = await userId.Content.ReadFromJsonAsync<ApiGetResponse<Guid>>();

            _client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue(
                "Bearer",
                TestAuthorization.GetToken("Admin", schedule.User.Login, jsonUserId!.Content)
            );

            var response = await _client.GetAsync($"/api/Schedule/{new Guid()}");

            await _client.DeleteAsync($"/api/User/{jsonUserId.Content}");

            Assert.Equal(HttpStatusCode.NotFound, response.StatusCode);
        }

        [Fact]
        public async Task GetScheduleByNameThrowsUnothorizedErrorWhenNoTokenIsGiven()
        {
            var schedule = MockData.GetScheduleDetails();
            var userRequest = new CreateUserDTO
            {
                Login = schedule.User.Login,
                CustomURL = schedule.User.CustomURL!,
                Password = "sssSSS1!"
            };
            var scheduleRequest = new CreateScheduleDTO
            {
                Name = schedule.Name,
                SchoolHour = schedule.SchoolHour
            };

            _client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue(
                "Bearer",
                TestAuthorization.GetToken("Admin", schedule.User.Login, schedule.User.Id)
            );

            var userId = await _client.PostAsJsonAsync("/api/User", userRequest);
            var jsonUserId = await userId.Content.ReadFromJsonAsync<ApiGetResponse<Guid>>();

            _client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue(
                "Bearer",
                TestAuthorization.GetToken("Admin", schedule.User.Login, jsonUserId!.Content)
            );

            var scheduleId = await _client.PostAsJsonAsync("/api/Schedule", scheduleRequest);

            _client.DefaultRequestHeaders.Authorization = null;

            var response = await _client.GetAsync($"/api/Schedule/{new Guid()}");

            _client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue(
                "Bearer",
                TestAuthorization.GetToken("Admin", schedule.User.Login, jsonUserId!.Content)
            );
            await _client.DeleteAsync($"/api/User/{jsonUserId.Content}");

            Assert.Equal(HttpStatusCode.Unauthorized, response.StatusCode);
        }

        [Fact]
        public async Task GetAllScheduleTReturnsSchedules()
        {
            var schedule = MockData.GetScheduleDetails();
            var userRequest1 = new CreateUserDTO
            {
                Login = schedule.User.Login,
                CustomURL = schedule.User.CustomURL!,
                Password = "sssSSS1!"
            };
            var userRequest2 = new CreateUserDTO
            {
                Login = schedule.User.Login + "2",
                CustomURL = schedule.User.CustomURL! + "2",
                Password = "sssSSS1!"
            };
            var scheduleRequest1 = new CreateScheduleDTO
            {
                Name = schedule.Name,
                SchoolHour = schedule.SchoolHour
            };
            var scheduleRequest2 = new CreateScheduleDTO
            {
                Name = "Other_Plan",
                SchoolHour = schedule.SchoolHour + 2
            };

            _client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue(
                "Bearer",
                TestAuthorization.GetToken("Admin", schedule.User.Login, schedule.User.Id)
            );

            var userId1 = await _client.PostAsJsonAsync("/api/User", userRequest1);
            var jsonUserId1 = await userId1.Content.ReadFromJsonAsync<ApiGetResponse<Guid>>();
            var userId2 = await _client.PostAsJsonAsync("/api/User", userRequest2);
            var jsonUserId2 = await userId2.Content.ReadFromJsonAsync<ApiGetResponse<Guid>>();

            _client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue(
                "Bearer",
                TestAuthorization.GetToken("Admin", schedule.User.Login, jsonUserId1!.Content)
            );

            await _client.PostAsJsonAsync("/api/Schedule", scheduleRequest1);

            _client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue(
                "Bearer",
                TestAuthorization.GetToken("Admin", schedule.User.Login, jsonUserId2!.Content)
            );

            await _client.PostAsJsonAsync("/api/Schedule", scheduleRequest2);

            var response = await _client.GetAsync("/api/Schedule");

            var stringResponse = await response.Content.ReadAsStringAsync();

            await _client.DeleteAsync($"/api/User/{jsonUserId1.Content}");
            await _client.DeleteAsync($"/api/User/{jsonUserId2.Content}");

            Assert.Contains(scheduleRequest1.Name, stringResponse);
            Assert.Contains(scheduleRequest2.Name, stringResponse);
        }

        [Fact]
        public async Task GetAllScheduleTReturnsEmptyContentWhenWrongUserAttemptsGet()
        {
            var schedule = MockData.GetScheduleDetails();
            var userRequest1 = new CreateUserDTO
            {
                Login = schedule.User.Login,
                CustomURL = schedule.User.CustomURL!,
                Password = "sssSSS1!"
            };
            var userRequest2 = new CreateUserDTO
            {
                Login = schedule.User.Login + "2",
                CustomURL = schedule.User.CustomURL! + "2",
                Password = "sssSSS1!"
            };
            var scheduleRequest1 = new CreateScheduleDTO
            {
                Name = schedule.Name,
                SchoolHour = schedule.SchoolHour
            };
            var scheduleRequest2 = new CreateScheduleDTO
            {
                Name = "Other_Plan",
                SchoolHour = schedule.SchoolHour + 2
            };

            _client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue(
                "Bearer",
                TestAuthorization.GetToken("Admin", schedule.User.Login, schedule.User.Id)
            );

            var userId1 = await _client.PostAsJsonAsync("/api/User", userRequest1);
            var jsonUserId1 = await userId1.Content.ReadFromJsonAsync<ApiGetResponse<Guid>>();
            var userId2 = await _client.PostAsJsonAsync("/api/User", userRequest2);
            var jsonUserId2 = await userId2.Content.ReadFromJsonAsync<ApiGetResponse<Guid>>();

            _client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue(
                "Bearer",
                TestAuthorization.GetToken("Admin", schedule.User.Login, jsonUserId1!.Content)
            );
            await _client.PostAsJsonAsync("/api/Schedule", scheduleRequest1);

            _client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue(
                "Bearer",
                TestAuthorization.GetToken("Admin", schedule.User.Login, jsonUserId2!.Content)
            );
            await _client.PostAsJsonAsync("/api/Schedule", scheduleRequest2);

            _client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue(
                "Bearer",
                TestAuthorization.GetToken("User", schedule.User.Login, new Guid())
            );
            var response = await _client.GetAsync("/api/Schedule");

            var stringResponse = await response.Content.ReadAsStringAsync();

            _client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue(
                "Bearer",
                TestAuthorization.GetToken("Admin", schedule.User.Login, jsonUserId2!.Content)
            );
            await _client.DeleteAsync($"/api/User/{jsonUserId1.Content}");
            await _client.DeleteAsync($"/api/User/{jsonUserId2.Content}");

            Assert.DoesNotContain(scheduleRequest1.Name, stringResponse);
            Assert.DoesNotContain(scheduleRequest2.Name, stringResponse);
        }

        [Fact]
        public async Task GetAllScheduleTReturnsOnlyUserMadeSchedules()
        {
            var schedule = MockData.GetScheduleDetails();
            var userRequest1 = new CreateUserDTO
            {
                Login = schedule.User.Login,
                CustomURL = schedule.User.CustomURL!,
                Password = "sssSSS1!"
            };
            var userRequest2 = new CreateUserDTO
            {
                Login = schedule.User.Login + "2",
                CustomURL = schedule.User.CustomURL! + "2",
                Password = "sssSSS1!"
            };
            var scheduleRequest1 = new CreateScheduleDTO
            {
                Name = schedule.Name,
                SchoolHour = schedule.SchoolHour
            };
            var scheduleRequest2 = new CreateScheduleDTO
            {
                Name = "Other_Plan",
                SchoolHour = schedule.SchoolHour + 2
            };

            _client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue(
                "Bearer",
                TestAuthorization.GetToken("Admin", schedule.User.Login, schedule.User.Id)
            );

            var userId1 = await _client.PostAsJsonAsync("/api/User", userRequest1);
            var jsonUserId1 = await userId1.Content.ReadFromJsonAsync<ApiGetResponse<Guid>>();
            var userId2 = await _client.PostAsJsonAsync("/api/User", userRequest2);
            var jsonUserId2 = await userId2.Content.ReadFromJsonAsync<ApiGetResponse<Guid>>();

            _client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue(
                "Bearer",
                TestAuthorization.GetToken("User", schedule.User.Login, jsonUserId1!.Content)
            );
            await _client.PostAsJsonAsync("/api/Schedule", scheduleRequest1);

            _client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue(
                "Bearer",
                TestAuthorization.GetToken("User", schedule.User.Login, jsonUserId2!.Content)
            );
            await _client.PostAsJsonAsync("/api/Schedule", scheduleRequest2);

            var response = await _client.GetAsync("/api/Schedule");

            var stringResponse = await response.Content.ReadAsStringAsync();

            _client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue(
                "Bearer",
                TestAuthorization.GetToken("Admin", schedule.User.Login, jsonUserId1!.Content)
            );
            await _client.DeleteAsync($"/api/User/{jsonUserId1.Content}");
            await _client.DeleteAsync($"/api/User/{jsonUserId2.Content}");

            Assert.DoesNotContain(scheduleRequest1.Name, stringResponse);
            Assert.Contains(scheduleRequest2.Name, stringResponse);
        }

        [Fact]
        public async Task ThrowsUnothorizedErrorWhenNoTokenIsGiven()
        {
            var schedule = MockData.GetScheduleDetails();
            var userRequest1 = new CreateUserDTO
            {
                Login = schedule.User.Login,
                CustomURL = schedule.User.CustomURL!,
                Password = "sssSSS1!"
            };
            var userRequest2 = new CreateUserDTO
            {
                Login = schedule.User.Login + "2",
                CustomURL = schedule.User.CustomURL! + "2",
                Password = "sssSSS1!"
            };
            var scheduleRequest1 = new CreateScheduleDTO
            {
                Name = schedule.Name,
                SchoolHour = schedule.SchoolHour
            };
            var scheduleRequest2 = new CreateScheduleDTO
            {
                Name = "Other_Plan",
                SchoolHour = schedule.SchoolHour + 2
            };

            _client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue(
                "Bearer",
                TestAuthorization.GetToken("Admin", schedule.User.Login, schedule.User.Id)
            );

            var userId1 = await _client.PostAsJsonAsync("/api/User", userRequest1);
            var jsonUserId1 = await userId1.Content.ReadFromJsonAsync<ApiGetResponse<Guid>>();
            var userId2 = await _client.PostAsJsonAsync("/api/User", userRequest2);
            var jsonUserId2 = await userId2.Content.ReadFromJsonAsync<ApiGetResponse<Guid>>();

            _client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue(
                "Bearer",
                TestAuthorization.GetToken("Admin", schedule.User.Login, jsonUserId1!.Content)
            );
            await _client.PostAsJsonAsync("/api/Schedule", scheduleRequest1);
            await _client.PostAsJsonAsync("/api/Schedule", scheduleRequest2);

            _client.DefaultRequestHeaders.Authorization = null;
            var response = await _client.GetAsync("/api/Schedule");

            _client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue(
                "Bearer",
                TestAuthorization.GetToken("Admin", schedule.User.Login, jsonUserId1!.Content)
            );
            await _client.DeleteAsync($"/api/User/{jsonUserId1.Content}");
            await _client.DeleteAsync($"/api/User/{jsonUserId2!.Content}");

            Assert.Equal(HttpStatusCode.Unauthorized, response.StatusCode);
        }
    }
}
