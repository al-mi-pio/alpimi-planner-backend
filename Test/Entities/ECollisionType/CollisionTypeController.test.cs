using System.Net;
using System.Net.Http.Headers;
using AlpimiAPI.Entities.ECollisionType.DTO;
using AlpimiAPI.Responses;
using AlpimiAPI.Utilities;
using AlpimiTest.TestSetup;
using AlpimiTest.TestUtilities;
using Xunit;

namespace AlpimiTest.Entities.ECollisionType
{
    [Collection("Sequential Tests")]
    public class CollisionTypeControllerTest : IAsyncLifetime
    {
        CustomWebApplicationFactory<Program> _factory;
        HttpClient _client;
        Guid userId;
        Guid scheduleId;

        public CollisionTypeControllerTest()
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
        public async Task CollisionTypeControllerThrowsUnauthorized()
        {
            _client.DefaultRequestHeaders.Authorization = null;

            var response = await _client.DeleteAsync($"/api/CollisionType/{new Guid()}");
            Assert.Equal(HttpStatusCode.Unauthorized, response.StatusCode);

            response = await _client.PostAsJsonAsync(
                "/api/CollisionType",
                MockData.GetCreateCollisionTypeDTODetails(scheduleId)
            );
            Assert.Equal(HttpStatusCode.Unauthorized, response.StatusCode);

            response = await _client.PatchAsJsonAsync(
                $"/api/CollisionType/{new Guid()}",
                MockData.GetUpdateCollisionTypeDTODetails()
            );
            Assert.Equal(HttpStatusCode.Unauthorized, response.StatusCode);

            response = await _client.GetAsync($"/api/CollisionType/{new Guid()}");
            Assert.Equal(HttpStatusCode.Unauthorized, response.StatusCode);
        }

        [Fact]
        public async Task CollisionTypeControllerThrowsTooManyRequests()
        {
            for (int i = 0; i != Configuration.GetPermitLimit(); i++)
            {
                await _client.GetAsync("/api/CollisionType");
            }
            _client.DefaultRequestHeaders.Authorization = null;

            var response = await _client.DeleteAsync($"/api/CollisionType/{new Guid()}");
            Assert.Equal(HttpStatusCode.TooManyRequests, response.StatusCode);

            response = await _client.PostAsJsonAsync(
                "/api/CollisionType",
                MockData.GetCreateCollisionTypeDTODetails(scheduleId)
            );
            Assert.Equal(HttpStatusCode.TooManyRequests, response.StatusCode);

            var query = $"?scheduleId={new Guid()}";
            response = await _client.GetAsync($"/api/CollisionType{query}");
            Assert.Equal(HttpStatusCode.TooManyRequests, response.StatusCode);

            response = await _client.PatchAsJsonAsync(
                $"/api/CollisionType/{new Guid()}",
                MockData.GetUpdateCollisionTypeDTODetails()
            );
            Assert.Equal(HttpStatusCode.TooManyRequests, response.StatusCode);

            response = await _client.GetAsync($"/api/CollisionType/{new Guid()}");
            Assert.Equal(HttpStatusCode.TooManyRequests, response.StatusCode);
        }

        [Fact]
        public async Task CollisionTypeIsCreated()
        {
            var collisionTypeRequest = MockData.GetCreateCollisionTypeDTODetails(scheduleId);
            _client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue(
                "Bearer",
                TestAuthorization.GetToken("Admin", "User", userId)
            );

            var response = await _client.PostAsJsonAsync(
                "/api/CollisionType",
                collisionTypeRequest
            );
            Assert.Equal(HttpStatusCode.OK, response.StatusCode);

            var query = $"?scheduleId={scheduleId}";
            response = await _client.GetAsync($"/api/CollisionType{query}");
            Assert.Equal(HttpStatusCode.OK, response.StatusCode);
            var stringResponse = await response.Content.ReadAsStringAsync();
            Assert.Contains(collisionTypeRequest.Name!, stringResponse);
        }

        [Fact]
        public async Task CreateCollisionTypeIsUndone()
        {
            var collisionTypeRequest = MockData.GetCreateCollisionTypeDTODetails(scheduleId);
            _client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue(
                "Bearer",
                TestAuthorization.GetToken("Admin", "User", userId)
            );

            await _client.PostAsJsonAsync("/api/CollisionType", collisionTypeRequest);
            await _client.PatchAsJsonAsync($"/api/History/undo/{scheduleId}", "");

            var query = $"?scheduleId={scheduleId}";
            var response = await _client.GetAsync($"/api/CollisionType{query}");
            Assert.Equal(HttpStatusCode.OK, response.StatusCode);
            var stringResponse = await response.Content.ReadAsStringAsync();
            Assert.DoesNotContain(collisionTypeRequest.Name!, stringResponse);
        }

        [Fact]
        public async Task CreateCollisionTypeIsRedone()
        {
            var collisionTypeRequest = MockData.GetCreateCollisionTypeDTODetails(scheduleId);
            _client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue(
                "Bearer",
                TestAuthorization.GetToken("Admin", "User", userId)
            );

            await _client.PostAsJsonAsync("/api/CollisionType", collisionTypeRequest);
            await _client.PatchAsJsonAsync($"/api/History/undo/{scheduleId}", "");
            await _client.PatchAsJsonAsync($"/api/History/redo/{scheduleId}", "");

            var query = $"?scheduleId={scheduleId}";
            var response = await _client.GetAsync($"/api/CollisionType{query}");
            Assert.Equal(HttpStatusCode.OK, response.StatusCode);
            var stringResponse = await response.Content.ReadAsStringAsync();
            Assert.Contains(collisionTypeRequest.Name!, stringResponse);
        }

        [Fact]
        public async Task CollisionTypeIsDeleted()
        {
            var collisionTypeRequest = MockData.GetCreateCollisionTypeDTODetails(scheduleId);
            var collisionTypeId = await DbHelper.SetupCollisionType(_client, collisionTypeRequest);
            _client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue(
                "Bearer",
                TestAuthorization.GetToken("Admin", "User", new Guid())
            );

            var response = await _client.DeleteAsync($"/api/CollisionType/{collisionTypeId}");
            Assert.Equal(HttpStatusCode.NoContent, response.StatusCode);

            var query = $"?scheduleId={scheduleId}";
            response = await _client.GetAsync($"/api/CollisionType{query}");
            Assert.Equal(HttpStatusCode.OK, response.StatusCode);
            var stringResponse = await response.Content.ReadAsStringAsync();
            Assert.DoesNotContain(collisionTypeRequest.Name!, stringResponse);
        }

        [Fact]
        public async Task DeleteCollisionTypeIsUndone()
        {
            var collisionTypeRequest = MockData.GetCreateCollisionTypeDTODetails(scheduleId);
            var collisionTypeId = await DbHelper.SetupCollisionType(_client, collisionTypeRequest);
            _client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue(
                "Bearer",
                TestAuthorization.GetToken("Admin", "User", new Guid())
            );

            await _client.DeleteAsync($"/api/CollisionType/{collisionTypeId}");
            await _client.PatchAsJsonAsync($"/api/History/undo/{scheduleId}", "");

            var query = $"?scheduleId={scheduleId}";
            var response = await _client.GetAsync($"/api/CollisionType{query}");
            Assert.Equal(HttpStatusCode.OK, response.StatusCode);
            var stringResponse = await response.Content.ReadAsStringAsync();
            Assert.Contains(collisionTypeRequest.Name!, stringResponse);
        }

        [Fact]
        public async Task DeleteCollisionTypeIsRedone()
        {
            var collisionTypeRequest = MockData.GetCreateCollisionTypeDTODetails(scheduleId);
            var collisionTypeId = await DbHelper.SetupCollisionType(_client, collisionTypeRequest);
            _client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue(
                "Bearer",
                TestAuthorization.GetToken("Admin", "User", new Guid())
            );

            await _client.DeleteAsync($"/api/CollisionType/{collisionTypeId}");
            await _client.PatchAsJsonAsync($"/api/History/undo/{scheduleId}", "");
            await _client.PatchAsJsonAsync($"/api/History/redo/{scheduleId}", "");

            var query = $"?scheduleId={scheduleId}";
            var response = await _client.GetAsync($"/api/CollisionType{query}");
            Assert.Equal(HttpStatusCode.OK, response.StatusCode);
            var stringResponse = await response.Content.ReadAsStringAsync();
            Assert.DoesNotContain(collisionTypeRequest.Name!, stringResponse);
        }

        [Fact]
        public async Task UpdateCollisionTypeReturnsUpdatedCollisionType()
        {
            var collisionTypeUpdateRequest = MockData.GetUpdateCollisionTypeDTODetails();
            var collisionTypeId = await DbHelper.SetupCollisionType(
                _client,
                MockData.GetCreateCollisionTypeDTODetails(scheduleId)
            );
            _client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue(
                "Bearer",
                TestAuthorization.GetToken("Admin", "Bob", userId)
            );

            var response = await _client.PatchAsJsonAsync(
                $"/api/CollisionType/{collisionTypeId}",
                collisionTypeUpdateRequest
            );
            var jsonResponse = await response.Content.ReadFromJsonAsync<
                ApiGetResponse<CollisionTypeDTO>
            >();

            Assert.Equal(collisionTypeUpdateRequest.Name, jsonResponse!.Content.Name);
            Assert.Equal(collisionTypeUpdateRequest.Weight, jsonResponse!.Content.Weight);
        }

        [Fact]
        public async Task UpdateCollisionTypeThrowsNotFoundErrorWhenWrongIdIsGiven()
        {
            var collisionTypeUpdateRequest = MockData.GetUpdateCollisionTypeDTODetails();
            var collisionTypeId = await DbHelper.SetupCollisionType(
                _client,
                MockData.GetCreateCollisionTypeDTODetails(scheduleId)
            );
            _client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue(
                "Bearer",
                TestAuthorization.GetToken("Admin", "User", userId)
            );

            var response = await _client.PatchAsJsonAsync(
                $"/api/CollisionType/{new Guid()}",
                collisionTypeUpdateRequest
            );

            Assert.Equal(HttpStatusCode.NotFound, response.StatusCode);
        }

        [Fact]
        public async Task UpdateCollisionTypeThrowsNotFoundErrorWhenWrongUserAttemptsUpdate()
        {
            var collisionTypeUpdateRequest = MockData.GetUpdateCollisionTypeDTODetails();
            var collisionTypeId = await DbHelper.SetupCollisionType(
                _client,
                MockData.GetCreateCollisionTypeDTODetails(scheduleId)
            );
            _client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue(
                "Bearer",
                TestAuthorization.GetToken("User", "User", new Guid())
            );

            var response = await _client.PatchAsJsonAsync(
                $"/api/CollisionType/{collisionTypeId}",
                collisionTypeUpdateRequest
            );

            Assert.Equal(HttpStatusCode.NotFound, response.StatusCode);
        }

        [Fact]
        public async Task UpdateCollisionTypeIsUndone()
        {
            var collisionTypeUpdateRequest = MockData.GetUpdateCollisionTypeDTODetails();
            var dto = MockData.GetCreateCollisionTypeDTODetails(scheduleId);
            var collisionTypeId = await DbHelper.SetupCollisionType(_client, dto);
            _client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue(
                "Bearer",
                TestAuthorization.GetToken("Admin", "Bob", userId)
            );

            await _client.PatchAsJsonAsync(
                $"/api/CollisionType/{collisionTypeId}",
                collisionTypeUpdateRequest
            );
            await _client.PatchAsJsonAsync($"/api/History/undo/{scheduleId}", "");

            var response = await _client.GetAsync($"/api/CollisionType/{collisionTypeId}");
            var jsonResponse = await response.Content.ReadFromJsonAsync<
                ApiGetResponse<CollisionTypeDTO>
            >();
            Assert.Equal(dto.Name, jsonResponse!.Content.Name);
            Assert.Equal(dto.Weight, jsonResponse!.Content.Weight);
        }

        [Fact]
        public async Task UpdateCollisionTypeIsRedone()
        {
            var collisionTypeUpdateRequest = MockData.GetUpdateCollisionTypeDTODetails();
            var collisionTypeId = await DbHelper.SetupCollisionType(
                _client,
                MockData.GetCreateCollisionTypeDTODetails(scheduleId)
            );
            _client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue(
                "Bearer",
                TestAuthorization.GetToken("Admin", "Bob", userId)
            );

            await _client.PatchAsJsonAsync(
                $"/api/CollisionType/{collisionTypeId}",
                collisionTypeUpdateRequest
            );
            await _client.PatchAsJsonAsync($"/api/History/undo/{scheduleId}", "");
            await _client.PatchAsJsonAsync($"/api/History/redo/{scheduleId}", "");

            var response = await _client.GetAsync($"/api/CollisionType/{collisionTypeId}");
            var jsonResponse = await response.Content.ReadFromJsonAsync<
                ApiGetResponse<CollisionTypeDTO>
            >();
            Assert.Equal(collisionTypeUpdateRequest.Name, jsonResponse!.Content.Name);
            Assert.Equal(collisionTypeUpdateRequest.Weight, jsonResponse!.Content.Weight);
        }

        [Fact]
        public async Task GetAllCollisionTypeByScheduleReturnsCollisionTypes()
        {
            var collisionTypeRequest1 = MockData.GetCreateCollisionTypeDTODetails(scheduleId);
            var collisionTypeRequest2 = MockData.GetCreateSecondCollisionTypeDTODetails(scheduleId);
            await DbHelper.SetupCollisionType(_client, collisionTypeRequest1);
            await DbHelper.SetupCollisionType(_client, collisionTypeRequest2);
            _client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue(
                "Bearer",
                TestAuthorization.GetToken("Admin", "User", userId)
            );

            var query = $"?scheduleId={scheduleId}";
            var response = await _client.GetAsync($"/api/CollisionType{query}");
            Assert.Equal(HttpStatusCode.OK, response.StatusCode);
            var stringResponse = await response.Content.ReadAsStringAsync();

            Assert.Contains(collisionTypeRequest1.Name!, stringResponse);
            Assert.Contains(collisionTypeRequest2.Name!, stringResponse);
        }

        [Fact]
        public async Task GetAllCollisionTypeByScheduleReturnsEmptyContentWhenWrongUserAttemptsGet()
        {
            var collisionTypeRequest1 = MockData.GetCreateCollisionTypeDTODetails(scheduleId);
            var collisionTypeRequest2 = MockData.GetCreateSecondCollisionTypeDTODetails(scheduleId);
            await DbHelper.SetupCollisionType(_client, collisionTypeRequest1);
            await DbHelper.SetupCollisionType(_client, collisionTypeRequest2);
            _client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue(
                "Bearer",
                TestAuthorization.GetToken("User", "User", new Guid())
            );

            var query = $"?scheduleId={scheduleId}";
            var response = await _client.GetAsync($"/api/CollisionType{query}");
            Assert.Equal(HttpStatusCode.OK, response.StatusCode);
            var stringResponse = await response.Content.ReadAsStringAsync();

            Assert.DoesNotContain(collisionTypeRequest1.Name!, stringResponse);
            Assert.DoesNotContain(collisionTypeRequest2.Name!, stringResponse);
        }

        [Fact]
        public async Task GetAllCollisionTypeByScheduleReturnsEmptyContentWhenWrongIdIsGiven()
        {
            var collisionTypeRequest1 = MockData.GetCreateCollisionTypeDTODetails(scheduleId);
            var collisionTypeRequest2 = MockData.GetCreateSecondCollisionTypeDTODetails(scheduleId);
            await DbHelper.SetupCollisionType(_client, collisionTypeRequest1);
            await DbHelper.SetupCollisionType(_client, collisionTypeRequest2);
            _client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue(
                "Bearer",
                TestAuthorization.GetToken("Admin", "User", userId)
            );

            var query = $"?scheduleId={new Guid()}";
            var response = await _client.GetAsync($"/api/CollisionType{query}");
            Assert.Equal(HttpStatusCode.OK, response.StatusCode);
            var stringResponse = await response.Content.ReadAsStringAsync();

            Assert.DoesNotContain(collisionTypeRequest1.Name!, stringResponse);
            Assert.DoesNotContain(collisionTypeRequest2.Name!, stringResponse);
        }

        [Fact]
        public async Task GetCollisionTypeReturnsCollisionType()
        {
            var collisionTypeRequest = MockData.GetCreateCollisionTypeDTODetails(scheduleId);
            var collisionTypeId = await DbHelper.SetupCollisionType(_client, collisionTypeRequest);
            _client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue(
                "Bearer",
                TestAuthorization.GetToken("Admin", "User", userId)
            );

            var response = await _client.GetAsync($"/api/CollisionType/{collisionTypeId}");
            var jsonResponse = await response.Content.ReadFromJsonAsync<
                ApiGetResponse<CollisionTypeDTO>
            >();

            Assert.Equal(collisionTypeRequest.Name!, jsonResponse!.Content.Name);
            Assert.Equal(collisionTypeRequest.Weight, jsonResponse!.Content.Weight);
        }

        [Fact]
        public async Task GetCollisionTypeThrowsNotFoundErrorWhenWrongUserTokenIsGiven()
        {
            var collisionTypeRequest = MockData.GetCreateCollisionTypeDTODetails(scheduleId);
            var collisionTypeId = await DbHelper.SetupCollisionType(_client, collisionTypeRequest);
            _client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue(
                "Bearer",
                TestAuthorization.GetToken("User", "User", new Guid())
            );

            var response = await _client.GetAsync($"/api/CollisionType/{collisionTypeId}");

            Assert.Equal(HttpStatusCode.NotFound, response.StatusCode);
        }

        [Fact]
        public async Task GetCollisionTypeThrowsNotFoundWhenWrongIdIsGiven()
        {
            var collisionTypeRequest = MockData.GetCreateCollisionTypeDTODetails(scheduleId);
            await DbHelper.SetupCollisionType(_client, collisionTypeRequest);
            _client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue(
                "Bearer",
                TestAuthorization.GetToken("Admin", "User", userId)
            );

            var response = await _client.GetAsync($"/api/CollisionType/{new Guid()}");

            Assert.Equal(HttpStatusCode.NotFound, response.StatusCode);
        }
    }
}
