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
        private readonly Mock<IStringLocalizer<Fields>> _strFields;

        public GetAllDayOffByScheduleQueryUnit()
        {
            _str = ResourceSetup.Setup();
            _strFields = ResourceSetup.FieldSetup();
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
                _str.Object,
                _strFields.Object
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
                        new FieldErrorObject("perPage", "Per Page parameter is invalid")
                    }
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
                _str.Object,
                _strFields.Object
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
                    new ErrorObject[] { new FieldErrorObject("page", "Page parameter is invalid") }
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
                _str.Object,
                _strFields.Object
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
                        new FieldErrorObject("sortBy", "Sort By parameter is invalid")
                    }
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
                _str.Object,
                _strFields.Object
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
                        new FieldErrorObject("sortOrder", "Sort Order parameter is invalid")
                    }
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
                _str.Object,
                _strFields.Object
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
                        new FieldErrorObject("sortOrder", "Sort Order parameter is invalid"),
                        new FieldErrorObject("sortBy", "Sort By parameter is invalid")
                    }
                ),
                JsonConvert.SerializeObject(result.errors)
            );
        }
    }
}
