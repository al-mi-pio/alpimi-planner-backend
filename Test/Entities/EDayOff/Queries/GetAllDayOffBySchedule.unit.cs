using AlpimiAPI.Database;
using AlpimiAPI.Entities.EDayOff.Queries;
using AlpimiAPI.Locales;
using AlpimiAPI.Responses;
using AlpimiTest.TestSetup;
using Microsoft.Extensions.Localization;
using Moq;
using Newtonsoft.Json;
using Xunit;

namespace AlpimiTest.Entities.EDayOff.Queries
{
    [Collection("Sequential Tests")]
    public class GetAllDayOffByScheduleQueryUnit
    {
        private readonly Mock<IDbService> _dbService = new();
        private readonly Mock<IStringLocalizer<Errors>> _str;

        public GetAllDayOffByScheduleQueryUnit()
        {
            _str = ResourceSetup.Setup();
        }

        [Fact]
        public async Task ThrowsErrorWhenIncorrectPerPageIsGiven()
        {
            var getAllDayOffByScheduleQuery = new GetAllDayOffByScheduleQuery(
                new Guid(),
                new Guid(),
                "Admin",
                new PaginationParams(-20, 0, "Id", "ASC")
            );
            var getAllDayOffByScheduleHandler = new GetAllDayOffByScheduleHandler(
                _dbService.Object,
                _str.Object
            );

            var result = await Assert.ThrowsAsync<ApiErrorException>(
                async () =>
                    await getAllDayOffByScheduleHandler.Handle(
                        getAllDayOffByScheduleQuery,
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
            var getAllDayOffByScheduleQuery = new GetAllDayOffByScheduleQuery(
                new Guid(),
                new Guid(),
                "Admin",
                new PaginationParams(20, -1, "Id", "ASC")
            );
            var getAllDayOffByScheduleHandler = new GetAllDayOffByScheduleHandler(
                _dbService.Object,
                _str.Object
            );

            var result = await Assert.ThrowsAsync<ApiErrorException>(
                async () =>
                    await getAllDayOffByScheduleHandler.Handle(
                        getAllDayOffByScheduleQuery,
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
            var getAllDayOffByScheduleQuery = new GetAllDayOffByScheduleQuery(
                new Guid(),
                new Guid(),
                "Admin",
                new PaginationParams(20, 0, "wrong", "ASC")
            );
            var getAllDayOffByScheduleHandler = new GetAllDayOffByScheduleHandler(
                _dbService.Object,
                _str.Object
            );

            var result = await Assert.ThrowsAsync<ApiErrorException>(
                async () =>
                    await getAllDayOffByScheduleHandler.Handle(
                        getAllDayOffByScheduleQuery,
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
            var getAllDayOffByScheduleQuery = new GetAllDayOffByScheduleQuery(
                new Guid(),
                new Guid(),
                "Admin",
                new PaginationParams(20, 0, "Id", "wrong")
            );
            var getAllDayOffByScheduleHandler = new GetAllDayOffByScheduleHandler(
                _dbService.Object,
                _str.Object
            );

            var result = await Assert.ThrowsAsync<ApiErrorException>(
                async () =>
                    await getAllDayOffByScheduleHandler.Handle(
                        getAllDayOffByScheduleQuery,
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
            var getAllDayOffByScheduleQuery = new GetAllDayOffByScheduleQuery(
                new Guid(),
                new Guid(),
                "Admin",
                new PaginationParams(20, 0, "wrong", "wrong")
            );
            var getAllDayOffByScheduleHandler = new GetAllDayOffByScheduleHandler(
                _dbService.Object,
                _str.Object
            );

            var result = await Assert.ThrowsAsync<ApiErrorException>(
                async () =>
                    await getAllDayOffByScheduleHandler.Handle(
                        getAllDayOffByScheduleQuery,
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
