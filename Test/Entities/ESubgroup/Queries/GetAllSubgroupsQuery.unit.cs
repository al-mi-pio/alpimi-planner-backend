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
    public class GetAllSubgroupsQueryUnit
    {
        private readonly Mock<IDbService> _dbService = new();
        private readonly Mock<IStringLocalizer<Errors>> _str;
        private readonly Mock<IStringLocalizer<Fields>> _strFields;

        public GetAllSubgroupsQueryUnit()
        {
            _str = ResourceSetup.Setup();
            _strFields = ResourceSetup.FieldSetup();
        }

        [Fact]
        public async Task ThrowsErrorWhenIncorrectPerPageIsGiven()
        {
            var getAllSubgroupQuery = new GetAllSubgroupsQuery(
                new Guid(),
                new Guid(),
                "Admin",
                new PaginationParams(-20, 0, "Id", "ASC")
            );
            var getAllSubgroupHandler = new GetAllSubgroupsHandler(
                _dbService.Object,
                _str.Object,
                _strFields.Object
            );
            var result = await Assert.ThrowsAsync<ApiErrorException>(
                async () =>
                    await getAllSubgroupHandler.Handle(getAllSubgroupQuery, new CancellationToken())
            );

            Assert.Equal(
                JsonConvert.SerializeObject(
                    new ErrorObject[]
                    {
                        new FieldErrorObject("perPage", "Per Page parameter is invalid")
                    }
                ),
                JsonConvert.SerializeObject(result.errors)
            );
        }

        [Fact]
        public async Task ThrowsErrorWhenIncorrectPageIsGiven()
        {
            var getAllSubgroupQuery = new GetAllSubgroupsQuery(
                new Guid(),
                new Guid(),
                "Admin",
                new PaginationParams(20, -1, "Id", "ASC")
            );
            var getAllSubgroupHandler = new GetAllSubgroupsHandler(
                _dbService.Object,
                _str.Object,
                _strFields.Object
            );
            var result = await Assert.ThrowsAsync<ApiErrorException>(
                async () =>
                    await getAllSubgroupHandler.Handle(getAllSubgroupQuery, new CancellationToken())
            );

            Assert.Equal(
                JsonConvert.SerializeObject(
                    new ErrorObject[] { new FieldErrorObject("page", "Page parameter is invalid") }
                ),
                JsonConvert.SerializeObject(result.errors)
            );
        }

        [Fact]
        public async Task ThrowsErrorWhenIncorrectSortByIsGiven()
        {
            var getAllSubgroupQuery = new GetAllSubgroupsQuery(
                new Guid(),
                new Guid(),
                "Admin",
                new PaginationParams(20, 0, "wrong", "ASC")
            );
            var getAllSubgroupHandler = new GetAllSubgroupsHandler(
                _dbService.Object,
                _str.Object,
                _strFields.Object
            );
            var result = await Assert.ThrowsAsync<ApiErrorException>(
                async () =>
                    await getAllSubgroupHandler.Handle(getAllSubgroupQuery, new CancellationToken())
            );

            Assert.Equal(
                JsonConvert.SerializeObject(
                    new ErrorObject[]
                    {
                        new FieldErrorObject("sortBy", "Sort By parameter is invalid")
                    }
                ),
                JsonConvert.SerializeObject(result.errors)
            );
        }

        [Fact]
        public async Task ThrowsErrorWhenIncorrectSortOrderIsGiven()
        {
            var getAllSubgroupQuery = new GetAllSubgroupsQuery(
                new Guid(),
                new Guid(),
                "Admin",
                new PaginationParams(20, 0, "Id", "wrong")
            );
            var getAllSubgroupHandler = new GetAllSubgroupsHandler(
                _dbService.Object,
                _str.Object,
                _strFields.Object
            );
            var result = await Assert.ThrowsAsync<ApiErrorException>(
                async () =>
                    await getAllSubgroupHandler.Handle(getAllSubgroupQuery, new CancellationToken())
            );

            Assert.Equal(
                JsonConvert.SerializeObject(
                    new ErrorObject[]
                    {
                        new FieldErrorObject("sortOrder", "Sort Order parameter is invalid")
                    }
                ),
                JsonConvert.SerializeObject(result.errors)
            );
        }

        [Fact]
        public async Task ThrowsMultipleErrorMessages()
        {
            var getAllSubgroupQuery = new GetAllSubgroupsQuery(
                new Guid(),
                new Guid(),
                "Admin",
                new PaginationParams(20, 0, "wrong", "wrong")
            );
            var getAllSubgroupHandler = new GetAllSubgroupsHandler(
                _dbService.Object,
                _str.Object,
                _strFields.Object
            );
            var result = await Assert.ThrowsAsync<ApiErrorException>(
                async () =>
                    await getAllSubgroupHandler.Handle(getAllSubgroupQuery, new CancellationToken())
            );

            Assert.Equal(
                JsonConvert.SerializeObject(
                    new ErrorObject[]
                    {
                        new FieldErrorObject("sortOrder", "Sort Order parameter is invalid"),
                        new FieldErrorObject("sortBy", "Sort By parameter is invalid")
                    }
                ),
                JsonConvert.SerializeObject(result.errors)
            );
        }
    }
}
