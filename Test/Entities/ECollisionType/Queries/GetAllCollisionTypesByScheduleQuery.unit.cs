using AlpimiAPI.Database;
using AlpimiAPI.Entities.ECollisionType.Queries;
using AlpimiAPI.Locales;
using AlpimiAPI.Responses;
using AlpimiTest.TestSetup;
using Microsoft.Extensions.Localization;
using Moq;
using Newtonsoft.Json;
using Xunit;

namespace AlpimiTest.Entities.ECollisionType.Queries
{
    [Collection("Sequential Tests")]
    public class GetAllCollisionTypesByScheduleQueryUnit
    {
        private readonly Mock<IDbService> _dbService = new();
        private readonly Mock<IStringLocalizer<Errors>> _str;
        private readonly Mock<IStringLocalizer<Fields>> _strFields;

        public GetAllCollisionTypesByScheduleQueryUnit()
        {
            _str = ResourceSetup.Setup();
            _strFields = ResourceSetup.FieldSetup();
        }

        [Fact]
        public async Task ThrowsErrorWhenIncorrectPerPageIsGiven()
        {
            var getAllCollisionTypeByScheduleQuery = new GetAllCollisionTypesByScheduleQuery(
                new Guid(),
                new Guid(),
                "Admin",
                new PaginationParams(-20, 0, "Id", "ASC")
            );
            var getAllCollisionTypeByScheduleHandler = new GetAllCollisionTypesByScheduleHandler(
                _dbService.Object,
                _str.Object,
                _strFields.Object
            );
            var result = await Assert.ThrowsAsync<ApiErrorException>(
                async () =>
                    await getAllCollisionTypeByScheduleHandler.Handle(
                        getAllCollisionTypeByScheduleQuery,
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
            var getAllCollisionTypeByScheduleQuery = new GetAllCollisionTypesByScheduleQuery(
                new Guid(),
                new Guid(),
                "Admin",
                new PaginationParams(20, -1, "Id", "ASC")
            );
            var getAllCollisionTypeByScheduleHandler = new GetAllCollisionTypesByScheduleHandler(
                _dbService.Object,
                _str.Object,
                _strFields.Object
            );
            var result = await Assert.ThrowsAsync<ApiErrorException>(
                async () =>
                    await getAllCollisionTypeByScheduleHandler.Handle(
                        getAllCollisionTypeByScheduleQuery,
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
            var getAllCollisionTypeByScheduleQuery = new GetAllCollisionTypesByScheduleQuery(
                new Guid(),
                new Guid(),
                "Admin",
                new PaginationParams(20, 0, "wrong", "ASC")
            );
            var getAllCollisionTypeByScheduleHandler = new GetAllCollisionTypesByScheduleHandler(
                _dbService.Object,
                _str.Object,
                _strFields.Object
            );
            var result = await Assert.ThrowsAsync<ApiErrorException>(
                async () =>
                    await getAllCollisionTypeByScheduleHandler.Handle(
                        getAllCollisionTypeByScheduleQuery,
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
            var getAllCollisionTypeByScheduleQuery = new GetAllCollisionTypesByScheduleQuery(
                new Guid(),
                new Guid(),
                "Admin",
                new PaginationParams(20, 0, "Id", "wrong")
            );
            var getAllCollisionTypeByScheduleHandler = new GetAllCollisionTypesByScheduleHandler(
                _dbService.Object,
                _str.Object,
                _strFields.Object
            );
            var result = await Assert.ThrowsAsync<ApiErrorException>(
                async () =>
                    await getAllCollisionTypeByScheduleHandler.Handle(
                        getAllCollisionTypeByScheduleQuery,
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
            var getAllCollisionTypeByScheduleQuery = new GetAllCollisionTypesByScheduleQuery(
                new Guid(),
                new Guid(),
                "Admin",
                new PaginationParams(20, 0, "wrong", "wrong")
            );
            var getAllCollisionTypeByScheduleHandler = new GetAllCollisionTypesByScheduleHandler(
                _dbService.Object,
                _str.Object,
                _strFields.Object
            );
            var result = await Assert.ThrowsAsync<ApiErrorException>(
                async () =>
                    await getAllCollisionTypeByScheduleHandler.Handle(
                        getAllCollisionTypeByScheduleQuery,
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
