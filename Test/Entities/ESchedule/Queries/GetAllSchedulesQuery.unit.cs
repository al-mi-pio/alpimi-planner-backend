using AlpimiAPI.Database;
using AlpimiAPI.Entities.ESchedule.Queries;
using AlpimiAPI.Responses;
using Moq;
using Newtonsoft.Json;
using Xunit;

namespace AlpimiTest.Entities.ESchedule.Queries
{
    [Collection("Sequential Tests")]
    public class GetAllSchedulesQueryUnit
    {
        private readonly Mock<IDbService> _dbService = new();

        [Fact]
        public async Task ThrowsErrorWhenIncorrectPerPageIsGiven()
        {
            var getSchedulesQuery = new GetAllSchedulesQuery(
                new Guid(),
                "Admin",
                new PaginationParams(-20, 0, "Id", "ASC")
            );
            var getSchedulesHandler = new GetSchedulesHandler(_dbService.Object);

            var result = await Assert.ThrowsAsync<ApiErrorException>(
                async () =>
                    await getSchedulesHandler.Handle(getSchedulesQuery, new CancellationToken())
            );

            Assert.Equal(
                JsonConvert.SerializeObject(new ErrorObject[] { new ErrorObject("Bad PerPage") }),
                JsonConvert.SerializeObject(result.errors)
            );
        }

        [Fact]
        public async Task ThrowsErrorWhenIncorrectPageIsGiven()
        {
            var getSchedulesQuery = new GetAllSchedulesQuery(
                new Guid(),
                "Admin",
                new PaginationParams(20, -1, "Id", "ASC")
            );
            var getSchedulesHandler = new GetSchedulesHandler(_dbService.Object);

            var result = await Assert.ThrowsAsync<ApiErrorException>(
                async () =>
                    await getSchedulesHandler.Handle(getSchedulesQuery, new CancellationToken())
            );

            Assert.Equal(
                JsonConvert.SerializeObject(new ErrorObject[] { new ErrorObject("Bad Page") }),
                JsonConvert.SerializeObject(result.errors)
            );
        }

        [Fact]
        public async Task ThrowsErrorWhenIncorrectSortByIsGiven()
        {
            var getSchedulesQuery = new GetAllSchedulesQuery(
                new Guid(),
                "Admin",
                new PaginationParams(20, 0, "wrong", "ASC")
            );
            var getSchedulesHandler = new GetSchedulesHandler(_dbService.Object);

            var result = await Assert.ThrowsAsync<ApiErrorException>(
                async () =>
                    await getSchedulesHandler.Handle(getSchedulesQuery, new CancellationToken())
            );

            Assert.Equal(
                JsonConvert.SerializeObject(new ErrorObject[] { new ErrorObject("Bad SortBy") }),
                JsonConvert.SerializeObject(result.errors)
            );
        }

        [Fact]
        public async Task ThrowsErrorWhenIncorrectSortOrderIsGiven()
        {
            var getSchedulesQuery = new GetAllSchedulesQuery(
                new Guid(),
                "Admin",
                new PaginationParams(20, 0, "Id", "wrong")
            );
            var getSchedulesHandler = new GetSchedulesHandler(_dbService.Object);

            var result = await Assert.ThrowsAsync<ApiErrorException>(
                async () =>
                    await getSchedulesHandler.Handle(getSchedulesQuery, new CancellationToken())
            );

            Assert.Equal(
                JsonConvert.SerializeObject(new ErrorObject[] { new ErrorObject("Bad SortOrder") }),
                JsonConvert.SerializeObject(result.errors)
            );
        }

        [Fact]
        public async Task ThrowsMultipleErrorMessages()
        {
            var getSchedulesQuery = new GetAllSchedulesQuery(
                new Guid(),
                "Admin",
                new PaginationParams(20, 0, "wrong", "wrong")
            );
            var getSchedulesHandler = new GetSchedulesHandler(_dbService.Object);

            var result = await Assert.ThrowsAsync<ApiErrorException>(
                async () =>
                    await getSchedulesHandler.Handle(getSchedulesQuery, new CancellationToken())
            );

            Assert.Equal(
                JsonConvert.SerializeObject(
                    new ErrorObject[]
                    {
                        new ErrorObject("Bad SortOrder"),
                        new ErrorObject("Bad SortBy")
                    }
                ),
                JsonConvert.SerializeObject(result.errors)
            );
        }
    }
}
