using AlpimiAPI.Database;
using AlpimiAPI.Entities.ESubgroup.Queries;
using AlpimiAPI.Locales;
using AlpimiAPI.Responses;
using AlpimiTest.TestSetup;
using Microsoft.Extensions.Localization;
using Moq;
using Newtonsoft.Json;
using Xunit;

namespace AlpimiTest.Entities.ESubgroup.Queries
{
    [Collection("Sequential Tests")]
    public class GetAllSubgroupsByGroupQueryUnit
    {
        private readonly Mock<IDbService> _dbService = new();
        private readonly Mock<IStringLocalizer<Errors>> _str;

        public GetAllSubgroupsByGroupQueryUnit()
        {
            _str = ResourceSetup.Setup();
        }

        [Fact]
        public async Task ThrowsErrorWhenIncorrectPerPageIsGiven()
        {
            var getAllSubgroupByGroupQuery = new GetAllSubgroupsQuery(
                new Guid(),
                new Guid(),
                "Admin",
                new PaginationParams(-20, 0, "Id", "ASC")
            );
            var getAllSubgroupByGroupHandler = new GetAllSubgroupsHandler(
                _dbService.Object,
                _str.Object
            );

            var result = await Assert.ThrowsAsync<ApiErrorException>(
                async () =>
                    await getAllSubgroupByGroupHandler.Handle(
                        getAllSubgroupByGroupQuery,
                        new CancellationToken()
                    )
            );

            Assert.Equal(
                JsonConvert.SerializeObject(
                    new ErrorObject[] { new ErrorObject("PerPage parameter is invalid") }
                ),
                JsonConvert.SerializeObject(result.errors)
            );
        }

        [Fact]
        public async Task ThrowsErrorWhenIncorrectPageIsGiven()
        {
            var getAllSubgroupByGroupQuery = new GetAllSubgroupsQuery(
                new Guid(),
                new Guid(),
                "Admin",
                new PaginationParams(20, -1, "Id", "ASC")
            );
            var getAllSubgroupByGroupHandler = new GetAllSubgroupsHandler(
                _dbService.Object,
                _str.Object
            );

            var result = await Assert.ThrowsAsync<ApiErrorException>(
                async () =>
                    await getAllSubgroupByGroupHandler.Handle(
                        getAllSubgroupByGroupQuery,
                        new CancellationToken()
                    )
            );

            Assert.Equal(
                JsonConvert.SerializeObject(
                    new ErrorObject[] { new ErrorObject("Page parameter is invalid") }
                ),
                JsonConvert.SerializeObject(result.errors)
            );
        }

        [Fact]
        public async Task ThrowsErrorWhenIncorrectSortByIsGiven()
        {
            var getAllSubgroupByGroupQuery = new GetAllSubgroupsQuery(
                new Guid(),
                new Guid(),
                "Admin",
                new PaginationParams(20, 0, "wrong", "ASC")
            );
            var getAllSubgroupByGroupHandler = new GetAllSubgroupsHandler(
                _dbService.Object,
                _str.Object
            );

            var result = await Assert.ThrowsAsync<ApiErrorException>(
                async () =>
                    await getAllSubgroupByGroupHandler.Handle(
                        getAllSubgroupByGroupQuery,
                        new CancellationToken()
                    )
            );

            Assert.Equal(
                JsonConvert.SerializeObject(
                    new ErrorObject[] { new ErrorObject("SortBy parameter is invalid") }
                ),
                JsonConvert.SerializeObject(result.errors)
            );
        }

        [Fact]
        public async Task ThrowsErrorWhenIncorrectSortOrderIsGiven()
        {
            var getAllSubgroupByGroupQuery = new GetAllSubgroupsQuery(
                new Guid(),
                new Guid(),
                "Admin",
                new PaginationParams(20, 0, "Id", "wrong")
            );
            var getAllSubgroupByGroupHandler = new GetAllSubgroupsHandler(
                _dbService.Object,
                _str.Object
            );

            var result = await Assert.ThrowsAsync<ApiErrorException>(
                async () =>
                    await getAllSubgroupByGroupHandler.Handle(
                        getAllSubgroupByGroupQuery,
                        new CancellationToken()
                    )
            );

            Assert.Equal(
                JsonConvert.SerializeObject(
                    new ErrorObject[] { new ErrorObject("SortOrder parameter is invalid") }
                ),
                JsonConvert.SerializeObject(result.errors)
            );
        }

        [Fact]
        public async Task ThrowsMultipleErrorMessages()
        {
            var getAllSubgroupByGroupQuery = new GetAllSubgroupsQuery(
                new Guid(),
                new Guid(),
                "Admin",
                new PaginationParams(20, 0, "wrong", "wrong")
            );
            var getAllSubgroupByGroupHandler = new GetAllSubgroupsHandler(
                _dbService.Object,
                _str.Object
            );

            var result = await Assert.ThrowsAsync<ApiErrorException>(
                async () =>
                    await getAllSubgroupByGroupHandler.Handle(
                        getAllSubgroupByGroupQuery,
                        new CancellationToken()
                    )
            );

            Assert.Equal(
                JsonConvert.SerializeObject(
                    new ErrorObject[]
                    {
                        new ErrorObject("SortOrder parameter is invalid"),
                        new ErrorObject("SortBy parameter is invalid")
                    }
                ),
                JsonConvert.SerializeObject(result.errors)
            );
        }
    }
}
