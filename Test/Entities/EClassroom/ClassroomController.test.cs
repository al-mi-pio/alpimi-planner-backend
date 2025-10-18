using System.Net;
using System.Net.Http.Headers;
using AlpimiAPI.Entities.EClassroom.DTO;
using AlpimiAPI.Responses;
using AlpimiAPI.Utilities;
using AlpimiTest.TestSetup;
using AlpimiTest.TestUtilities;
using Xunit;

namespace AlpimiTest.Entities.EClassroom
{
    [Collection("Sequential Tests")]
    public class ClassroomControllerTest : IAsyncLifetime
    {
        CustomWebApplicationFactory<Program> _factory;
        HttpClient _client;
        Guid userId;
        Guid scheduleId;

        public ClassroomControllerTest()
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
        public async Task ClassroomControllerThrowsUnauthorized()
        {
            _client.DefaultRequestHeaders.Authorization = null;

            var response = await _client.DeleteAsync($"/api/Classroom/{new Guid()}");
            Assert.Equal(HttpStatusCode.Unauthorized, response.StatusCode);

            response = await _client.PostAsJsonAsync(
                "/api/Classroom",
                MockData.GetCreateClassroomDTODetails(scheduleId)
            );
            Assert.Equal(HttpStatusCode.Unauthorized, response.StatusCode);

            response = await _client.PatchAsJsonAsync(
                $"/api/Classroom/{new Guid()}",
                MockData.GetUpdateClassroomDTODetails()
            );
            Assert.Equal(HttpStatusCode.Unauthorized, response.StatusCode);

            response = await _client.GetAsync($"/api/Classroom/{new Guid()}");
            Assert.Equal(HttpStatusCode.Unauthorized, response.StatusCode);
        }

        [Fact]
        public async Task ClassroomTypeControllerThrowsTooManyRequests()
        {
            _client.DefaultRequestHeaders.Authorization = null;

            for (int i = 0; i != Configuration.GetLoosePermitLimit(); i++)
            {
                await _client.DeleteAsync($"/api/ClassroomType/{new Guid()}");
            }
            var response = await _client.DeleteAsync($"/api/ClassroomType/{new Guid()}");
            Assert.Equal(HttpStatusCode.TooManyRequests, response.StatusCode);

            for (int i = 0; i != Configuration.GetLoosePermitLimit(); i++)
            {
                await _client.PostAsJsonAsync(
                    "/api/ClassroomType",
                    MockData.GetCreateClassroomTypeDTODetails(scheduleId)
                );
            }
            response = await _client.PostAsJsonAsync(
                "/api/ClassroomType",
                MockData.GetCreateClassroomTypeDTODetails(scheduleId)
            );
            Assert.Equal(HttpStatusCode.TooManyRequests, response.StatusCode);

            var query = $"?id={new Guid()}";
            for (int i = 0; i != Configuration.GetLoosePermitLimit(); i++)
            {
                await _client.GetAsync($"/api/ClassroomType{query}");
            }
            response = await _client.GetAsync($"/api/ClassroomType{query}");
            Assert.Equal(HttpStatusCode.TooManyRequests, response.StatusCode);

            for (int i = 0; i != Configuration.GetLoosePermitLimit(); i++)
            {
                await _client.PatchAsJsonAsync(
                    $"/api/ClassroomType/{new Guid()}",
                    MockData.GetUpdateClassroomTypeDTODetails()
                );
            }
            response = await _client.PatchAsJsonAsync(
                $"/api/ClassroomType/{new Guid()}",
                MockData.GetUpdateClassroomTypeDTODetails()
            );
            Assert.Equal(HttpStatusCode.TooManyRequests, response.StatusCode);

            for (int i = 0; i != Configuration.GetLoosePermitLimit(); i++)
            {
                await _client.GetAsync($"/api/ClassroomType/{new Guid()}");
            }
            response = await _client.GetAsync($"/api/ClassroomType/{new Guid()}");
            Assert.Equal(HttpStatusCode.TooManyRequests, response.StatusCode);
        }

        [Fact]
        public async Task ClassroomIsCreated()
        {
            var classroomRequest = MockData.GetCreateClassroomDTODetails(scheduleId);
            _client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue(
                "Bearer",
                TestAuthorization.GetToken("Admin", "User", userId)
            );

            var response = await _client.PostAsJsonAsync("/api/Classroom", classroomRequest);
            Assert.Equal(HttpStatusCode.OK, response.StatusCode);

            var query = $"?id={scheduleId}";
            response = await _client.GetAsync($"/api/Classroom{query}");
            Assert.Equal(HttpStatusCode.OK, response.StatusCode);
            var stringResponse = await response.Content.ReadAsStringAsync();
            Assert.Contains(classroomRequest.Name!, stringResponse);
        }

        [Fact]
        public async Task CreateClassroomIsUndone()
        {
            var classroomRequest = MockData.GetCreateClassroomDTODetails(scheduleId);
            _client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue(
                "Bearer",
                TestAuthorization.GetToken("Admin", "User", userId)
            );

            await _client.PostAsJsonAsync("/api/Classroom", classroomRequest);
            await _client.PatchAsJsonAsync($"/api/History/undo/{scheduleId}", "");

            var query = $"?id={scheduleId}";
            var response = await _client.GetAsync($"/api/Classroom{query}");
            Assert.Equal(HttpStatusCode.OK, response.StatusCode);
            var stringResponse = await response.Content.ReadAsStringAsync();
            Assert.DoesNotContain(classroomRequest.Name!, stringResponse);
        }

        [Fact]
        public async Task CreateClassroomIsRedone()
        {
            var classroomRequest = MockData.GetCreateClassroomDTODetails(scheduleId);
            _client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue(
                "Bearer",
                TestAuthorization.GetToken("Admin", "User", userId)
            );

            await _client.PostAsJsonAsync("/api/Classroom", classroomRequest);
            await _client.PatchAsJsonAsync($"/api/History/undo/{scheduleId}", "");
            await _client.PatchAsJsonAsync($"/api/History/redo/{scheduleId}", "");

            var query = $"?id={scheduleId}";
            var response = await _client.GetAsync($"/api/Classroom{query}");
            Assert.Equal(HttpStatusCode.OK, response.StatusCode);
            var stringResponse = await response.Content.ReadAsStringAsync();
            Assert.Contains(classroomRequest.Name!, stringResponse);
        }

        [Fact]
        public async Task ClassroomIsCreatedWithClassroomTypes()
        {
            _client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue(
                "Bearer",
                TestAuthorization.GetToken("Admin", "User", userId)
            );
            var classroomTypeId = await DbHelper.SetupClassroomType(
                _client,
                MockData.GetCreateClassroomTypeDTODetails(scheduleId)
            );
            var classroomRequest = MockData.GetCreateClassroomDTODetails(scheduleId);
            classroomRequest.ClassroomTypeIds = [classroomTypeId];

            var response = await _client.PostAsJsonAsync("/api/Classroom", classroomRequest);
            Assert.Equal(HttpStatusCode.OK, response.StatusCode);

            var query = $"?id={classroomTypeId}";
            response = await _client.GetAsync($"/api/Classroom{query}");
            Assert.Equal(HttpStatusCode.OK, response.StatusCode);
            var stringResponse = await response.Content.ReadAsStringAsync();
            Assert.Contains(classroomRequest.Name!, stringResponse);
        }

        [Fact]
        public async Task ClassroomIsDeleted()
        {
            var classroomRequest = MockData.GetCreateClassroomDTODetails(scheduleId);
            var classroomId = await DbHelper.SetupClassroom(_client, classroomRequest);
            _client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue(
                "Bearer",
                TestAuthorization.GetToken("Admin", "User", new Guid())
            );

            var response = await _client.DeleteAsync($"/api/Classroom/{classroomId}");
            Assert.Equal(HttpStatusCode.NoContent, response.StatusCode);

            var query = $"?id={scheduleId}";
            response = await _client.GetAsync($"/api/Classroom{query}");
            Assert.Equal(HttpStatusCode.OK, response.StatusCode);
            var stringResponse = await response.Content.ReadAsStringAsync();
            Assert.DoesNotContain(classroomRequest.Name!, stringResponse);
        }

        [Fact]
        public async Task DeleteClassroomIsUndone()
        {
            var classroomRequest = MockData.GetCreateClassroomDTODetails(scheduleId);
            var classroomId = await DbHelper.SetupClassroom(_client, classroomRequest);
            _client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue(
                "Bearer",
                TestAuthorization.GetToken("Admin", "User", new Guid())
            );

            await _client.DeleteAsync($"/api/Classroom/{classroomId}");
            await _client.PatchAsJsonAsync($"/api/History/undo/{scheduleId}", "");

            var query = $"?id={scheduleId}";
            var response = await _client.GetAsync($"/api/Classroom{query}");
            Assert.Equal(HttpStatusCode.OK, response.StatusCode);
            var stringResponse = await response.Content.ReadAsStringAsync();
            Assert.Contains(classroomRequest.Name!, stringResponse);
        }

        [Fact]
        public async Task DeleteClassroomIsRedone()
        {
            var classroomRequest = MockData.GetCreateClassroomDTODetails(scheduleId);
            var classroomId = await DbHelper.SetupClassroom(_client, classroomRequest);
            _client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue(
                "Bearer",
                TestAuthorization.GetToken("Admin", "User", new Guid())
            );

            await _client.DeleteAsync($"/api/Classroom/{classroomId}");
            await _client.PatchAsJsonAsync($"/api/History/undo/{scheduleId}", "");
            await _client.PatchAsJsonAsync($"/api/History/redo/{scheduleId}", "");

            var query = $"?id={scheduleId}";
            var response = await _client.GetAsync($"/api/Classroom{query}");
            Assert.Equal(HttpStatusCode.OK, response.StatusCode);
            var stringResponse = await response.Content.ReadAsStringAsync();
            Assert.DoesNotContain(classroomRequest.Name!, stringResponse);
        }

        [Fact]
        public async Task UpdateClassroomReturnsUpdatedClassroom()
        {
            var classroomUpdateRequest = MockData.GetUpdateClassroomDTODetails();
            var classroomId = await DbHelper.SetupClassroom(
                _client,
                MockData.GetCreateClassroomDTODetails(scheduleId)
            );
            _client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue(
                "Bearer",
                TestAuthorization.GetToken("Admin", "Bob", userId)
            );

            var response = await _client.PatchAsJsonAsync(
                $"/api/Classroom/{classroomId}",
                classroomUpdateRequest
            );
            var jsonResponse = await response.Content.ReadFromJsonAsync<
                ApiGetResponse<ClassroomDTO>
            >();

            Assert.Equal(classroomUpdateRequest.Name, jsonResponse!.Content.Name);
        }

        [Fact]
        public async Task UpdateClassroomUpdatesClassroomsClassroomTypes()
        {
            _client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue(
                "Bearer",
                TestAuthorization.GetToken("Admin", "Bob", userId)
            );
            var classroomId = await DbHelper.SetupClassroom(
                _client,
                MockData.GetCreateClassroomDTODetails(scheduleId)
            );

            var classroomUpdateRequest = MockData.GetUpdateClassroomDTODetails();
            var classroomTypeId = await DbHelper.SetupClassroomType(
                _client,
                MockData.GetCreateClassroomTypeDTODetails(scheduleId)
            );
            classroomUpdateRequest.ClassroomTypeIds = [classroomTypeId];
            await _client.PatchAsJsonAsync($"/api/Classroom/{classroomId}", classroomUpdateRequest);

            var query = $"?id={classroomTypeId}";
            var response = await _client.GetAsync($"/api/Classroom{query}");
            Assert.Equal(HttpStatusCode.OK, response.StatusCode);
            var stringResponse = await response.Content.ReadAsStringAsync();
            Assert.Contains(classroomUpdateRequest.Name!, stringResponse);
        }

        [Fact]
        public async Task UpdateClassroomThrowsNotFoundErrorWhenWrongIdIsGiven()
        {
            var classroomId = await DbHelper.SetupClassroom(
                _client,
                MockData.GetCreateClassroomDTODetails(scheduleId)
            );
            _client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue(
                "Bearer",
                TestAuthorization.GetToken("Admin", "User", userId)
            );

            var classroomUpdateRequest = MockData.GetUpdateClassroomDTODetails();
            var response = await _client.PatchAsJsonAsync(
                $"/api/Classroom/{new Guid()}",
                classroomUpdateRequest
            );

            Assert.Equal(HttpStatusCode.NotFound, response.StatusCode);
        }

        [Fact]
        public async Task UpdateClassroomThrowsNotFoundErrorWhenWrongUserAttemptsUpdate()
        {
            var classroomId = await DbHelper.SetupClassroom(
                _client,
                MockData.GetCreateClassroomDTODetails(scheduleId)
            );
            _client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue(
                "Bearer",
                TestAuthorization.GetToken("User", "User", new Guid())
            );

            var classroomUpdateRequest = MockData.GetUpdateClassroomDTODetails();
            var response = await _client.PatchAsJsonAsync(
                $"/api/Classroom/{classroomId}",
                classroomUpdateRequest
            );

            Assert.Equal(HttpStatusCode.NotFound, response.StatusCode);
        }

        [Fact]
        public async Task UpdateClassroomIsUndone()
        {
            var classroomUpdateRequest = MockData.GetUpdateClassroomDTODetails();
            var dto = MockData.GetCreateClassroomDTODetails(scheduleId);
            var classroomId = await DbHelper.SetupClassroom(_client, dto);
            _client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue(
                "Bearer",
                TestAuthorization.GetToken("Admin", "Bob", userId)
            );

            await _client.PatchAsJsonAsync($"/api/Classroom/{classroomId}", classroomUpdateRequest);
            await _client.PatchAsJsonAsync($"/api/History/undo/{scheduleId}", "");

            var response = await _client.GetAsync($"/api/Classroom/{classroomId}");
            var jsonResponse = await response.Content.ReadFromJsonAsync<
                ApiGetResponse<ClassroomDTO>
            >();
            Assert.Equal(dto.Name, jsonResponse!.Content.Name);
        }

        [Fact]
        public async Task UpdateClassroomIsRedone()
        {
            var classroomUpdateRequest = MockData.GetUpdateClassroomDTODetails();
            var classroomId = await DbHelper.SetupClassroom(
                _client,
                MockData.GetCreateClassroomDTODetails(scheduleId)
            );
            _client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue(
                "Bearer",
                TestAuthorization.GetToken("Admin", "Bob", userId)
            );

            await _client.PatchAsJsonAsync($"/api/Classroom/{classroomId}", classroomUpdateRequest);
            await _client.PatchAsJsonAsync($"/api/History/undo/{scheduleId}", "");
            await _client.PatchAsJsonAsync($"/api/History/redo/{scheduleId}", "");

            var response = await _client.GetAsync($"/api/Classroom/{classroomId}");
            var jsonResponse = await response.Content.ReadFromJsonAsync<
                ApiGetResponse<ClassroomDTO>
            >();
            Assert.Equal(classroomUpdateRequest.Name, jsonResponse!.Content.Name);
        }

        [Fact]
        public async Task GetAllClassroomsReturnsClassroomsFromScheduleIfShceduleIdIsProvided()
        {
            var classroomRequest1 = MockData.GetCreateClassroomDTODetails(scheduleId);
            var classroomRequest2 = MockData.GetCreateSecondClassroomDTODetails(scheduleId);
            await DbHelper.SetupClassroom(_client, classroomRequest1);
            await DbHelper.SetupClassroom(_client, classroomRequest2);
            _client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue(
                "Bearer",
                TestAuthorization.GetToken("Admin", "User", userId)
            );

            var query = $"?id={scheduleId}";
            var response = await _client.GetAsync($"/api/Classroom{query}");
            Assert.Equal(HttpStatusCode.OK, response.StatusCode);
            var stringResponse = await response.Content.ReadAsStringAsync();

            Assert.Contains(classroomRequest1.Name!, stringResponse);
            Assert.Contains(classroomRequest2.Name!, stringResponse);
        }

        [Fact]
        public async Task GetAllClassroomsReturnsClassroomsFromClassroomTypeIfClassroomTypeIdIsProvided()
        {
            var classromTypeId = await DbHelper.SetupClassroomType(
                _client,
                MockData.GetCreateClassroomTypeDTODetails(scheduleId)
            );
            var classroomRequest1 = MockData.GetCreateClassroomDTODetails(scheduleId);
            classroomRequest1.ClassroomTypeIds = [classromTypeId];
            var classroomRequest2 = MockData.GetCreateSecondClassroomDTODetails(scheduleId);
            await DbHelper.SetupClassroom(_client, classroomRequest1);
            await DbHelper.SetupClassroom(_client, classroomRequest2);
            _client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue(
                "Bearer",
                TestAuthorization.GetToken("Admin", "User", userId)
            );

            var query = $"?id={classromTypeId}";
            var response = await _client.GetAsync($"/api/Classroom{query}");
            Assert.Equal(HttpStatusCode.OK, response.StatusCode);
            var stringResponse = await response.Content.ReadAsStringAsync();

            Assert.Contains(classroomRequest1.Name!, stringResponse);
            Assert.DoesNotContain(classroomRequest2.Name!, stringResponse);
        }

        [Fact]
        public async Task GetAllClassroomsReturnsClassroomsFromPublicSchedules()
        {
            var classromTypeId = await DbHelper.SetupClassroomType(
                _client,
                MockData.GetCreateClassroomTypeDTODetails(scheduleId)
            );
            await DbHelper.PublishSchedule(_client, scheduleId);
            var classroomRequest1 = MockData.GetCreateClassroomDTODetails(scheduleId);
            var classroomRequest2 = MockData.GetCreateSecondClassroomDTODetails(scheduleId);
            await DbHelper.SetupClassroom(_client, classroomRequest1);
            await DbHelper.SetupClassroom(_client, classroomRequest2);
            _client.DefaultRequestHeaders.Authorization = null;

            var query = $"?id={scheduleId}";
            var response = await _client.GetAsync($"/api/Classroom{query}");
            Assert.Equal(HttpStatusCode.OK, response.StatusCode);
            var stringResponse = await response.Content.ReadAsStringAsync();

            Assert.Contains(classroomRequest1.Name!, stringResponse);
            Assert.Contains(classroomRequest2.Name!, stringResponse);
        }

        [Fact]
        public async Task GetAllClassroomReturnsEmptyContentWhenWrongUserAttemptsGet()
        {
            var classroomRequest1 = MockData.GetCreateClassroomDTODetails(scheduleId);
            var classroomRequest2 = MockData.GetCreateSecondClassroomDTODetails(scheduleId);
            await DbHelper.SetupClassroom(_client, classroomRequest1);
            await DbHelper.SetupClassroom(_client, classroomRequest2);
            _client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue(
                "Bearer",
                TestAuthorization.GetToken("User", "User", new Guid())
            );

            var query = $"?id={scheduleId}";
            var response = await _client.GetAsync($"/api/Classroom{query}");
            Assert.Equal(HttpStatusCode.OK, response.StatusCode);
            var stringResponse = await response.Content.ReadAsStringAsync();

            Assert.DoesNotContain(classroomRequest1.Name!, stringResponse);
            Assert.DoesNotContain(classroomRequest2.Name!, stringResponse);
        }

        [Fact]
        public async Task GetAllClassroomReturnsEmptyContentWhenWrongIdIsGiven()
        {
            var classroomRequest1 = MockData.GetCreateClassroomDTODetails(scheduleId);
            var classroomRequest2 = MockData.GetCreateSecondClassroomDTODetails(scheduleId);
            await DbHelper.SetupClassroom(_client, classroomRequest1);
            await DbHelper.SetupClassroom(_client, classroomRequest2);
            _client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue(
                "Bearer",
                TestAuthorization.GetToken("Admin", "User", userId)
            );

            var query = $"?id={new Guid()}";
            var response = await _client.GetAsync($"/api/Classroom{query}");
            Assert.Equal(HttpStatusCode.OK, response.StatusCode);
            var stringResponse = await response.Content.ReadAsStringAsync();

            Assert.DoesNotContain(classroomRequest1.Name!, stringResponse);
            Assert.DoesNotContain(classroomRequest2.Name!, stringResponse);
        }

        [Fact]
        public async Task GetClassroomReturnsClassroom()
        {
            var classroomRequest = MockData.GetCreateClassroomDTODetails(scheduleId);
            var classroomId = await DbHelper.SetupClassroom(_client, classroomRequest);
            _client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue(
                "Bearer",
                TestAuthorization.GetToken("Admin", "User", userId)
            );

            var response = await _client.GetAsync($"/api/Classroom/{classroomId}");
            var jsonResponse = await response.Content.ReadFromJsonAsync<
                ApiGetResponse<ClassroomDTO>
            >();

            Assert.Equal(classroomRequest.Name!, jsonResponse!.Content.Name);
        }

        [Fact]
        public async Task GetScheduleThrowsNotFoundWhenWrongIdIsGiven()
        {
            var classroomRequest = MockData.GetCreateClassroomDTODetails(scheduleId);
            await DbHelper.SetupClassroom(_client, classroomRequest);
            _client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue(
                "Bearer",
                TestAuthorization.GetToken("Admin", "User", userId)
            );

            var response = await _client.GetAsync($"/api/Classroom/{new Guid()}");

            Assert.Equal(HttpStatusCode.NotFound, response.StatusCode);
        }

        [Fact]
        public async Task GetScheduleThrowsNotFoundErrorWhenWrongUserTokenIsGiven()
        {
            var classroomRequest = MockData.GetCreateClassroomDTODetails(scheduleId);
            var classroomId = await DbHelper.SetupClassroom(_client, classroomRequest);
            _client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue(
                "Bearer",
                TestAuthorization.GetToken("User", "User", new Guid())
            );

            var response = await _client.GetAsync($"/api/Classroom/{classroomId}");

            Assert.Equal(HttpStatusCode.NotFound, response.StatusCode);
        }
    }
}
