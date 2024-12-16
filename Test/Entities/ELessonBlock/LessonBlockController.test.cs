using System.Net;
using System.Net.Http.Headers;
using AlpimiAPI.Entities.ELesson.DTO;
using AlpimiAPI.Entities.ELessonBlock;
using AlpimiAPI.Responses;
using AlpimiAPI.Settings;
using AlpimiAPI.Utilities;
using AlpimiTest.TestSetup;
using AlpimiTest.TestUtilities;
using Azure;
using Xunit;

namespace AlpimiTest.Entities.ELessonBlock
{
    [Collection("Sequential Tests")]
    public class LessonBlockControllerTest : IAsyncLifetime
    {
        CustomWebApplicationFactory<Program> _factory;
        HttpClient _client;
        Guid userId;
        Guid scheduleId;
        Guid groupId;
        Guid subgroupId1;
        Guid subgroupId2;
        Guid lessonTypeId;
        Guid lessonId1;
        Guid lessonId2;
        Guid teacherId1;
        Guid teacherId2;
        Guid classroomId1;
        Guid classroomId2;

        public LessonBlockControllerTest()
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
            subgroupId1 = await DbHelper.SetupSubgroup(
                _client,
                MockData.GetCreateSubgroupDTODetails(groupId)
            );
            subgroupId2 = await DbHelper.SetupSubgroup(
                _client,
                MockData.GetCreateSecondSubgroupDTODetails(groupId)
            );
            lessonTypeId = await DbHelper.SetupLessonType(
                _client,
                MockData.GetCreateLessonTypeDTODetails(scheduleId)
            );
            lessonId1 = await DbHelper.SetupLesson(
                _client,
                MockData.GetCreateLessonDTODetails(subgroupId1, lessonTypeId)
            );
            lessonId2 = await DbHelper.SetupLesson(
                _client,
                MockData.GetCreateSecondLessonDTODetails(subgroupId2, lessonTypeId)
            );
            teacherId1 = await DbHelper.SetupTeacher(
                _client,
                MockData.GetCreateTeacherDTODetails(scheduleId)
            );
            teacherId2 = await DbHelper.SetupTeacher(
                _client,
                MockData.GetCreateSecondTeacherDTODetails(scheduleId)
            );
            classroomId1 = await DbHelper.SetupClassroom(
                _client,
                MockData.GetCreateClassroomDTODetails(scheduleId)
            );
            classroomId2 = await DbHelper.SetupClassroom(
                _client,
                MockData.GetCreateSecondClassroomDTODetails(scheduleId)
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
        public async Task LessonBlockIsDeleted()
        {
            var lessonBlockRequest = MockData.GetCreateLessonBlockDTODetails(
                lessonId1,
                classroomId1,
                teacherId1
            );
            var lessonBlockId = await DbHelper.SetupLessonBlock(_client, lessonBlockRequest);

            _client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue(
                "Bearer",
                TestAuthorization.GetToken("Admin", "User", new Guid())
            );

            var response = await _client.DeleteAsync($"/api/LessonBlock/{lessonBlockId}");
            Assert.Equal(HttpStatusCode.NoContent, response.StatusCode);

            var query = $"?id={groupId}";
            response = await _client.GetAsync($"/api/LessonBlock{query}");
            var stringResponse = await response.Content.ReadAsStringAsync();
            Assert.DoesNotContain(Convert.ToString(lessonBlockRequest.LessonDate)!, stringResponse);
        }

        [Fact]
        public async Task LessonBlockClusterIsDeleted()
        {
            var lessonBlockRequest = MockData.GetCreateThirdLessonBlockDTODetails(
                lessonId1,
                classroomId1,
                teacherId1
            );
            var clusterId = await DbHelper.SetupLessonBlock(_client, lessonBlockRequest);

            _client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue(
                "Bearer",
                TestAuthorization.GetToken("Admin", "User", new Guid())
            );

            var response = await _client.DeleteAsync($"/api/LessonBlock/{clusterId}");
            Assert.Equal(HttpStatusCode.NoContent, response.StatusCode);

            var query = $"?id={groupId}";
            var secondResponse = await _client.GetAsync($"/api/LessonBlock{query}");
            var jsonResponse = await secondResponse.Content.ReadFromJsonAsync<
                ApiGetAllResponse<IEnumerable<LessonBlockDTO>>
            >();
            Assert.Equal(0, jsonResponse!.Pagination.TotalItems);
        }

        [Fact]
        public async Task DeleteLessonBlockUpdatesLessonsCurrentHours()
        {
            var lessonBlockRequest = MockData.GetCreateThirdLessonBlockDTODetails(
                lessonId1,
                classroomId1,
                teacherId1
            );
            var clusterId = await DbHelper.SetupLessonBlock(_client, lessonBlockRequest);

            _client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue(
                "Bearer",
                TestAuthorization.GetToken("Admin", "User", new Guid())
            );

            var response = await _client.DeleteAsync($"/api/LessonBlock/{clusterId}");
            Assert.Equal(HttpStatusCode.NoContent, response.StatusCode);

            var secondResponse = await _client.GetAsync($"/api/Lesson/{lessonId1}");
            var jsonResponse = await secondResponse.Content.ReadFromJsonAsync<
                ApiGetResponse<LessonDTO>
            >();
            Assert.Equal(0, jsonResponse!.Content.CurrentHours);
        }

        [Fact]
        public async Task LessonBlockIsCreated()
        {
            var lessonBlockRequest = MockData.GetCreateLessonBlockDTODetails(
                lessonId1,
                classroomId1,
                teacherId1
            );

            _client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue(
                "Bearer",
                TestAuthorization.GetToken("Admin", "User", userId)
            );

            var response = await _client.PostAsJsonAsync("/api/LessonBlock", lessonBlockRequest);
            Assert.Equal(HttpStatusCode.OK, response.StatusCode);

            var query = $"?id={groupId}";
            response = await _client.GetAsync($"/api/LessonBlock{query}");
            var stringResponse = await response.Content.ReadAsStringAsync();
            Assert.Contains(classroomId1.ToString(), stringResponse);
        }

        [Fact]
        public async Task LessonBlockClusterIsCreated()
        {
            var lessonBlockRequest = MockData.GetCreateThirdLessonBlockDTODetails(
                lessonId1,
                classroomId1,
                teacherId1
            );

            _client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue(
                "Bearer",
                TestAuthorization.GetToken("Admin", "User", new Guid())
            );

            var response = await _client.PostAsJsonAsync("/api/LessonBlock", lessonBlockRequest);
            Assert.Equal(HttpStatusCode.OK, response.StatusCode);

            var query = $"?id={groupId}";
            var secondResponse = await _client.GetAsync($"/api/LessonBlock{query}");
            var jsonResponse = await secondResponse.Content.ReadFromJsonAsync<
                ApiGetAllResponse<IEnumerable<LessonBlockDTO>>
            >();
            Assert.NotEqual(0, jsonResponse!.Pagination.TotalItems);
        }

        [Fact]
        public async Task CreateLessonBlockUpdatesLessonsCurrentHours()
        {
            var lessonBlockRequest = MockData.GetCreateLessonBlockDTODetails(
                lessonId1,
                classroomId1,
                teacherId1
            );

            _client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue(
                "Bearer",
                TestAuthorization.GetToken("Admin", "User", new Guid())
            );

            var response = await _client.PostAsJsonAsync("/api/LessonBlock", lessonBlockRequest);
            Assert.Equal(HttpStatusCode.OK, response.StatusCode);

            var secondResponse = await _client.GetAsync($"/api/Lesson/{lessonId1}");
            var jsonResponse = await secondResponse.Content.ReadFromJsonAsync<
                ApiGetResponse<LessonDTO>
            >();
            Assert.NotEqual(0, jsonResponse!.Content.CurrentHours);
        }

        [Fact]
        public async Task UpdateLessonBlockReturnsId()
        {
            var lessonBlockUpdateRequest = MockData.GetUpdateLessonBlockDTODetails();

            _client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue(
                "Bearer",
                TestAuthorization.GetToken("Admin", "Bob", userId)
            );

            var lessonBlockId = await DbHelper.SetupLessonBlock(
                _client,
                MockData.GetCreateLessonBlockDTODetails(lessonId1, classroomId1, teacherId1)
            );

            var response = await _client.PatchAsJsonAsync(
                $"/api/LessonBlock/{lessonBlockId}",
                lessonBlockUpdateRequest
            );
            var jsonResponse = await response.Content.ReadFromJsonAsync<ApiGetResponse<Guid>>();

            Assert.Equal(lessonBlockId, jsonResponse!.Content);
        }

        [Fact]
        public async Task UpdateLessonBlockUpdatesLessonBlockCluster()
        {
            var lessonBlockUpdateRequest = MockData.GetUpdateLessonBlockDTODetails();
            lessonBlockUpdateRequest.UpdateCluster = true;

            _client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue(
                "Bearer",
                TestAuthorization.GetToken("Admin", "Bob", userId)
            );

            var clusterId = await DbHelper.SetupLessonBlock(
                _client,
                MockData.GetCreateThirdLessonBlockDTODetails(lessonId1, classroomId1, teacherId1)
            );

            var response = await _client.PatchAsJsonAsync(
                $"/api/LessonBlock/{clusterId}",
                lessonBlockUpdateRequest
            );
            var jsonResponse = await response.Content.ReadFromJsonAsync<ApiGetResponse<Guid>>();

            Assert.Equal(clusterId, jsonResponse!.Content);
        }

        [Fact]
        public async Task UpdateLessonBlockUpdatesLessonsCurrentHours()
        {
            var lessonBlockUpdateRequest = MockData.GetUpdateLessonBlockDTODetails();
            lessonBlockUpdateRequest.UpdateCluster = true;

            _client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue(
                "Bearer",
                TestAuthorization.GetToken("Admin", "Bob", userId)
            );

            var clusterId = await DbHelper.SetupLessonBlock(
                _client,
                MockData.GetCreateThirdLessonBlockDTODetails(lessonId1, classroomId1, teacherId1)
            );

            await _client.PatchAsJsonAsync(
                $"/api/LessonBlock/{clusterId}",
                lessonBlockUpdateRequest
            );

            var response = await _client.GetAsync($"/api/Lesson/{lessonId1}");
            var jsonResponse = await response.Content.ReadFromJsonAsync<
                ApiGetResponse<LessonDTO>
            >();
            Assert.Equal(22, jsonResponse!.Content.CurrentHours);
        }

        [Fact]
        public async Task UpdateLessonBlockThrowsNotFoundErrorWhenWrongIdIsGiven()
        {
            var lessonBlockUpdateRequest = MockData.GetUpdateLessonBlockDTODetails();

            _client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue(
                "Bearer",
                TestAuthorization.GetToken("Admin", "Bob", userId)
            );

            await DbHelper.SetupLessonBlock(
                _client,
                MockData.GetCreateLessonBlockDTODetails(lessonId1, classroomId1, teacherId1)
            );

            var response = await _client.PatchAsJsonAsync(
                $"/api/LessonBlock/{new Guid()}",
                lessonBlockUpdateRequest
            );

            Assert.Equal(HttpStatusCode.NotFound, response.StatusCode);
        }

        [Fact]
        public async Task UpdateLessonBlockThrowsNotFoundErrorWhenWrongUserAttemptsUpdate()
        {
            var lessonBlockUpdateRequest = MockData.GetUpdateLessonBlockDTODetails();

            var lessonBlockId = await DbHelper.SetupLessonBlock(
                _client,
                MockData.GetCreateLessonBlockDTODetails(lessonId1, classroomId1, teacherId1)
            );

            _client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue(
                "Bearer",
                TestAuthorization.GetToken("User", "Bob", new Guid())
            );

            var response = await _client.PatchAsJsonAsync(
                $"/api/LessonBlock/{lessonBlockId}",
                lessonBlockUpdateRequest
            );

            Assert.Equal(HttpStatusCode.NotFound, response.StatusCode);
        }

        [Fact]
        public async Task GetAllLessonBlocksReturnsLessonBlocksFromGroupIfGroupIdIsProvided()
        {
            _client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue(
                "Bearer",
                TestAuthorization.GetToken("Admin", "Bob", userId)
            );

            await DbHelper.SetupLessonBlock(
                _client,
                MockData.GetCreateLessonBlockDTODetails(lessonId1, classroomId1, teacherId1)
            );
            await DbHelper.SetupLessonBlock(
                _client,
                MockData.GetCreateSecondLessonBlockDTODetails(lessonId1, classroomId1, teacherId2)
            );
            await DbHelper.SetupLessonBlock(
                _client,
                MockData.GetCreateThirdLessonBlockDTODetails(lessonId2, classroomId2, teacherId2)
            );

            var query = $"?id={groupId}";
            var response = await _client.GetAsync($"/api/LessonBlock{query}");
            var stringResponse = await response.Content.ReadAsStringAsync();
            Assert.Contains(classroomId1.ToString(), stringResponse);
            Assert.Contains(classroomId2.ToString(), stringResponse);
            Assert.Contains(teacherId1.ToString(), stringResponse);
            Assert.Contains(teacherId2.ToString(), stringResponse);
            Assert.Contains(lessonId1.ToString(), stringResponse);
            Assert.Contains(lessonId2.ToString(), stringResponse);
        }

        [Fact]
        public async Task GetAllLessonBlocksReturnsLessonBlocksFromSubgroupIfSubgroupIdIsProvided()
        {
            _client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue(
                "Bearer",
                TestAuthorization.GetToken("Admin", "Bob", userId)
            );

            await DbHelper.SetupLessonBlock(
                _client,
                MockData.GetCreateLessonBlockDTODetails(lessonId1, classroomId1, teacherId1)
            );
            await DbHelper.SetupLessonBlock(
                _client,
                MockData.GetCreateSecondLessonBlockDTODetails(lessonId1, classroomId1, teacherId2)
            );
            await DbHelper.SetupLessonBlock(
                _client,
                MockData.GetCreateThirdLessonBlockDTODetails(lessonId2, classroomId2, teacherId2)
            );

            var query = $"?id={subgroupId1}";
            var response = await _client.GetAsync($"/api/LessonBlock{query}");
            var stringResponse = await response.Content.ReadAsStringAsync();
            Assert.Contains(classroomId1.ToString(), stringResponse);
            Assert.DoesNotContain(classroomId2.ToString(), stringResponse);
            Assert.Contains(teacherId1.ToString(), stringResponse);
            Assert.Contains(teacherId2.ToString(), stringResponse);
            Assert.Contains(lessonId1.ToString(), stringResponse);
            Assert.DoesNotContain(lessonId2.ToString(), stringResponse);
        }

        [Fact]
        public async Task GetAllLessonBlocksReturnsLessonBlocksFromScheduleIfScheduleIdIsProvided()
        {
            _client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue(
                "Bearer",
                TestAuthorization.GetToken("Admin", "Bob", userId)
            );

            await DbHelper.SetupLessonBlock(
                _client,
                MockData.GetCreateLessonBlockDTODetails(lessonId1, classroomId1, teacherId1)
            );
            await DbHelper.SetupLessonBlock(
                _client,
                MockData.GetCreateSecondLessonBlockDTODetails(lessonId1, classroomId1, teacherId2)
            );
            await DbHelper.SetupLessonBlock(
                _client,
                MockData.GetCreateThirdLessonBlockDTODetails(lessonId2, classroomId2, teacherId2)
            );

            var query = $"?id={scheduleId}";
            var response = await _client.GetAsync($"/api/LessonBlock{query}");
            var stringResponse = await response.Content.ReadAsStringAsync();
            Assert.Contains(classroomId1.ToString(), stringResponse);
            Assert.Contains(classroomId2.ToString(), stringResponse);
            Assert.Contains(teacherId1.ToString(), stringResponse);
            Assert.Contains(teacherId2.ToString(), stringResponse);
            Assert.Contains(lessonId1.ToString(), stringResponse);
            Assert.Contains(lessonId2.ToString(), stringResponse);
        }

        [Fact]
        public async Task GetAllLessonBlocksReturnsLessonBlocksFromClassroomIfClassroomIdIsProvided()
        {
            _client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue(
                "Bearer",
                TestAuthorization.GetToken("Admin", "Bob", userId)
            );

            await DbHelper.SetupLessonBlock(
                _client,
                MockData.GetCreateLessonBlockDTODetails(lessonId1, classroomId1, teacherId1)
            );
            await DbHelper.SetupLessonBlock(
                _client,
                MockData.GetCreateSecondLessonBlockDTODetails(lessonId1, classroomId1, teacherId2)
            );
            await DbHelper.SetupLessonBlock(
                _client,
                MockData.GetCreateThirdLessonBlockDTODetails(lessonId2, classroomId2, teacherId2)
            );

            var query = $"?id={classroomId1}";
            var response = await _client.GetAsync($"/api/LessonBlock{query}");
            var stringResponse = await response.Content.ReadAsStringAsync();
            Assert.Contains(classroomId1.ToString(), stringResponse);
            Assert.DoesNotContain(classroomId2.ToString(), stringResponse);
            Assert.Contains(teacherId1.ToString(), stringResponse);
            Assert.Contains(teacherId2.ToString(), stringResponse);
            Assert.Contains(lessonId1.ToString(), stringResponse);
            Assert.DoesNotContain(lessonId2.ToString(), stringResponse);
        }

        [Fact]
        public async Task GetAllLessonBlocksReturnsLessonBlocksFromLessonIfLessonIdIsProvided()
        {
            _client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue(
                "Bearer",
                TestAuthorization.GetToken("Admin", "Bob", userId)
            );

            await DbHelper.SetupLessonBlock(
                _client,
                MockData.GetCreateLessonBlockDTODetails(lessonId1, classroomId1, teacherId1)
            );
            await DbHelper.SetupLessonBlock(
                _client,
                MockData.GetCreateSecondLessonBlockDTODetails(lessonId1, classroomId1, teacherId2)
            );
            await DbHelper.SetupLessonBlock(
                _client,
                MockData.GetCreateThirdLessonBlockDTODetails(lessonId2, classroomId2, teacherId2)
            );

            var query = $"?id={lessonId1}";
            var response = await _client.GetAsync($"/api/LessonBlock{query}");
            var stringResponse = await response.Content.ReadAsStringAsync();
            Assert.Contains(classroomId1.ToString(), stringResponse);
            Assert.DoesNotContain(classroomId2.ToString(), stringResponse);
            Assert.Contains(teacherId1.ToString(), stringResponse);
            Assert.Contains(teacherId2.ToString(), stringResponse);
            Assert.Contains(lessonId1.ToString(), stringResponse);
            Assert.DoesNotContain(lessonId2.ToString(), stringResponse);
        }

        [Fact]
        public async Task GetAllLessonBlocksReturnsLessonBlocksFromTeacherIfTeacherIdIsProvided()
        {
            _client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue(
                "Bearer",
                TestAuthorization.GetToken("Admin", "Bob", userId)
            );

            await DbHelper.SetupLessonBlock(
                _client,
                MockData.GetCreateLessonBlockDTODetails(lessonId1, classroomId1, teacherId1)
            );
            await DbHelper.SetupLessonBlock(
                _client,
                MockData.GetCreateSecondLessonBlockDTODetails(lessonId1, classroomId1, teacherId2)
            );
            await DbHelper.SetupLessonBlock(
                _client,
                MockData.GetCreateThirdLessonBlockDTODetails(lessonId2, classroomId2, teacherId2)
            );

            var query = $"?id={teacherId1}";
            var response = await _client.GetAsync($"/api/LessonBlock{query}");
            var stringResponse = await response.Content.ReadAsStringAsync();
            Assert.Contains(classroomId1.ToString(), stringResponse);
            Assert.DoesNotContain(classroomId2.ToString(), stringResponse);
            Assert.Contains(teacherId1.ToString(), stringResponse);
            Assert.DoesNotContain(teacherId2.ToString(), stringResponse);
            Assert.Contains(lessonId1.ToString(), stringResponse);
            Assert.DoesNotContain(lessonId2.ToString(), stringResponse);
        }

        [Fact]
        public async Task GetAllLessonBlocksReturnsLessonBlocksFromClsterIfClusterIdIsProvided()
        {
            _client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue(
                "Bearer",
                TestAuthorization.GetToken("Admin", "Bob", userId)
            );

            await DbHelper.SetupLessonBlock(
                _client,
                MockData.GetCreateLessonBlockDTODetails(lessonId1, classroomId1, teacherId1)
            );
            await DbHelper.SetupLessonBlock(
                _client,
                MockData.GetCreateSecondLessonBlockDTODetails(lessonId1, classroomId1, teacherId2)
            );
            var clusterId = await DbHelper.SetupLessonBlock(
                _client,
                MockData.GetCreateThirdLessonBlockDTODetails(lessonId2, classroomId2, teacherId2)
            );

            var query = $"?id={clusterId}";
            var response = await _client.GetAsync($"/api/LessonBlock{query}");
            var stringResponse = await response.Content.ReadAsStringAsync();
            Assert.DoesNotContain(classroomId1.ToString(), stringResponse);
            Assert.Contains(classroomId2.ToString(), stringResponse);
            Assert.DoesNotContain(teacherId1.ToString(), stringResponse);
            Assert.Contains(teacherId2.ToString(), stringResponse);
            Assert.DoesNotContain(lessonId1.ToString(), stringResponse);
            Assert.Contains(lessonId2.ToString(), stringResponse);
        }

        [Fact]
        public async Task GetAllLessonBlocksReturnsLessonBlocksFromProvidedDateRange()
        {
            _client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue(
                "Bearer",
                TestAuthorization.GetToken("Admin", "Bob", userId)
            );

            await DbHelper.SetupLessonBlock(
                _client,
                MockData.GetCreateLessonBlockDTODetails(lessonId1, classroomId1, teacherId1)
            );
            await DbHelper.SetupLessonBlock(
                _client,
                MockData.GetCreateSecondLessonBlockDTODetails(lessonId1, classroomId1, teacherId2)
            );
            await DbHelper.SetupLessonBlock(
                _client,
                MockData.GetCreateThirdLessonBlockDTODetails(lessonId2, classroomId2, teacherId2)
            );

            var query = $"?id={lessonId1}&fromDate=10.10.2023&toDate=10.10.2023";
            var response = await _client.GetAsync($"/api/LessonBlock{query}");
            var stringResponse = await response.Content.ReadAsStringAsync();
            Assert.Contains(classroomId1.ToString(), stringResponse);
            Assert.DoesNotContain(classroomId2.ToString(), stringResponse);
            Assert.Contains(teacherId1.ToString(), stringResponse);
            Assert.DoesNotContain(teacherId2.ToString(), stringResponse);
            Assert.Contains(lessonId1.ToString(), stringResponse);
            Assert.DoesNotContain(lessonId2.ToString(), stringResponse);
        }

        [Fact]
        public async Task GetAllLessonBlocksReturnsEmptyContentWhenWrongIdIsGiven()
        {
            _client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue(
                "Bearer",
                TestAuthorization.GetToken("Admin", "Bob", userId)
            );

            await DbHelper.SetupLessonBlock(
                _client,
                MockData.GetCreateLessonBlockDTODetails(lessonId1, classroomId1, teacherId1)
            );
            await DbHelper.SetupLessonBlock(
                _client,
                MockData.GetCreateSecondLessonBlockDTODetails(lessonId1, classroomId1, teacherId2)
            );
            await DbHelper.SetupLessonBlock(
                _client,
                MockData.GetCreateThirdLessonBlockDTODetails(lessonId2, classroomId2, teacherId2)
            );

            var query = $"?id={new Guid()}";
            var response = await _client.GetAsync($"/api/LessonBlock{query}");
            var stringResponse = await response.Content.ReadAsStringAsync();
            Assert.DoesNotContain(classroomId1.ToString(), stringResponse);
            Assert.DoesNotContain(classroomId2.ToString(), stringResponse);
            Assert.DoesNotContain(teacherId1.ToString(), stringResponse);
            Assert.DoesNotContain(teacherId2.ToString(), stringResponse);
            Assert.DoesNotContain(lessonId1.ToString(), stringResponse);
            Assert.DoesNotContain(lessonId2.ToString(), stringResponse);
        }

        [Fact]
        public async Task GetAllLessonBlocksReturnsEmptyContentWhenWrongUserAttemptsGet()
        {
            await DbHelper.SetupLessonBlock(
                _client,
                MockData.GetCreateLessonBlockDTODetails(lessonId1, classroomId1, teacherId1)
            );
            await DbHelper.SetupLessonBlock(
                _client,
                MockData.GetCreateSecondLessonBlockDTODetails(lessonId1, classroomId1, teacherId2)
            );
            await DbHelper.SetupLessonBlock(
                _client,
                MockData.GetCreateThirdLessonBlockDTODetails(lessonId2, classroomId2, teacherId2)
            );

            _client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue(
                "Bearer",
                TestAuthorization.GetToken("User", "Bob", new Guid())
            );

            var query = $"?id={lessonId1}";
            var response = await _client.GetAsync($"/api/LessonBlock{query}");
            var stringResponse = await response.Content.ReadAsStringAsync();
            Assert.DoesNotContain(classroomId1.ToString(), stringResponse);
            Assert.DoesNotContain(classroomId2.ToString(), stringResponse);
            Assert.DoesNotContain(teacherId1.ToString(), stringResponse);
            Assert.DoesNotContain(teacherId2.ToString(), stringResponse);
            Assert.DoesNotContain(lessonId1.ToString(), stringResponse);
            Assert.DoesNotContain(lessonId2.ToString(), stringResponse);
        }

        [Fact]
        public async Task GetLessonBlockReturnsLessonBlock()
        {
            _client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue(
                "Bearer",
                TestAuthorization.GetToken("Admin", "Bob", userId)
            );

            var lessonBlockId = await DbHelper.SetupLessonBlock(
                _client,
                MockData.GetCreateLessonBlockDTODetails(lessonId1, classroomId1, teacherId1)
            );

            var response = await _client.GetAsync($"/api/LessonBlock/{lessonBlockId}");
            var stringResponse = await response.Content.ReadAsStringAsync();
            Assert.Contains(classroomId1.ToString(), stringResponse);
            Assert.Contains(teacherId1.ToString(), stringResponse);
            Assert.Contains(lessonId1.ToString(), stringResponse);
        }

        [Fact]
        public async Task GetLessonBlockThrowsNotFoundErrorWhenWrongUserTokenIsGiven()
        {
            var lessonBlockId = await DbHelper.SetupLessonBlock(
                _client,
                MockData.GetCreateLessonBlockDTODetails(lessonId1, classroomId1, teacherId1)
            );

            _client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue(
                "Bearer",
                TestAuthorization.GetToken("User", "Bob", new Guid())
            );

            var response = await _client.GetAsync($"/api/LessonBlock/{lessonBlockId}");
            Assert.Equal(HttpStatusCode.NotFound, response.StatusCode);
        }

        [Fact]
        public async Task GetLessonBlockThrowsNotFoundWhenWrongIdIsGiven()
        {
            _client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue(
                "Bearer",
                TestAuthorization.GetToken("Admin", "Bob", userId)
            );

            var lessonBlockId = await DbHelper.SetupLessonBlock(
                _client,
                MockData.GetCreateLessonBlockDTODetails(lessonId1, classroomId1, teacherId1)
            );

            var response = await _client.GetAsync($"/api/LessonBlock/{new Guid()}");
            Assert.Equal(HttpStatusCode.NotFound, response.StatusCode);
        }

        [Fact]
        public async Task LessonBlockSettingsControllerThrowsUnauthorized()
        {
            _client.DefaultRequestHeaders.Authorization = null;

            var response = await _client.DeleteAsync($"/api/LessonBlock/{new Guid()}");
            Assert.Equal(HttpStatusCode.Unauthorized, response.StatusCode);

            response = await _client.PostAsJsonAsync(
                "/api/LessonBlock",
                MockData.GetCreateLessonDTODetails(subgroupId1, lessonTypeId)
            );
            Assert.Equal(HttpStatusCode.Unauthorized, response.StatusCode);

            var query = $"?groupId={new Guid()}";
            response = await _client.GetAsync($"/api/LessonBlock{query}");
            Assert.Equal(HttpStatusCode.Unauthorized, response.StatusCode);

            response = await _client.PatchAsJsonAsync(
                $"/api/LessonBlock/{new Guid()}",
                MockData.GetUpdateLessonDTODetails()
            );
            Assert.Equal(HttpStatusCode.Unauthorized, response.StatusCode);

            response = await _client.GetAsync($"/api/LessonBlock/{new Guid()}");
            Assert.Equal(HttpStatusCode.Unauthorized, response.StatusCode);
        }

        [Fact]
        public async Task LessonBlockControllerThrowsTooManyRequests()
        {
            for (int i = 0; i != Configuration.GetPermitLimit(); i++)
            {
                await _client.GetAsync("/api/LessonBlock");
            }

            _client.DefaultRequestHeaders.Authorization = null;

            var response = await _client.DeleteAsync($"/api/LessonBlock/{new Guid()}");
            Assert.Equal(HttpStatusCode.TooManyRequests, response.StatusCode);

            response = await _client.PostAsJsonAsync(
                "/api/LessonBlock",
                MockData.GetCreateLessonDTODetails(subgroupId1, lessonTypeId)
            );
            Assert.Equal(HttpStatusCode.TooManyRequests, response.StatusCode);

            var query = $"?groupId={new Guid()}";
            response = await _client.GetAsync($"/api/LessonBlock{query}");
            Assert.Equal(HttpStatusCode.TooManyRequests, response.StatusCode);

            response = await _client.PatchAsJsonAsync(
                $"/api/LessonBlock/{new Guid()}",
                MockData.GetUpdateLessonDTODetails()
            );
            Assert.Equal(HttpStatusCode.TooManyRequests, response.StatusCode);

            response = await _client.GetAsync($"/api/LessonBlock/{new Guid()}");
            Assert.Equal(HttpStatusCode.TooManyRequests, response.StatusCode);
        }
    }
}
