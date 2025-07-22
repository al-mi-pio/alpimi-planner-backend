using AlpimiAPI.Database;
using AlpimiAPI.Entities.ELessonBlock.Queries;
using AlpimiAPI.Entities.EScheduleSettings;
using AlpimiAPI.Locales;
using AlpimiAPI.Responses;
using AlpimiTest.TestSetup;
using AlpimiTest.TestUtilities;
using Microsoft.Extensions.Localization;
using Moq;
using Newtonsoft.Json;
using Xunit;

namespace AlpimiTest.Entities.ELessonBlock.Queries
{
    [Collection("Sequential Tests")]
    public class GetAllLessonBlocksQueryUnit
    {
        private readonly Mock<IDbService> _dbService = new();
        private readonly Mock<IStringLocalizer<Errors>> _str;
        private readonly Mock<IStringLocalizer<Fields>> _strFields;

        public GetAllLessonBlocksQueryUnit()
        {
            _str = ResourceSetup.Setup();
            _strFields = ResourceSetup.FieldSetup();
        }

        [Fact]
        public async Task ThrowsErrorWhenIncorrectPerPageIsGiven()
        {
            var getAllLessonBlockQuery = new GetAllLessonBlocksQuery(
                new Guid(),
                null,
                null,
                new Guid(),
                "Admin",
                new PaginationParams(-20, 0, "Id", "ASC")
            );
            var getAllLessonBlockHandler = new GetAllLessonBlocksHandler(
                _dbService.Object,
                _str.Object,
                _strFields.Object
            );
            var result = await Assert.ThrowsAsync<ApiErrorException>(
                async () =>
                    await getAllLessonBlockHandler.Handle(
                        getAllLessonBlockQuery,
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
            var getAllLessonBlockQuery = new GetAllLessonBlocksQuery(
                new Guid(),
                null,
                null,
                new Guid(),
                "Admin",
                new PaginationParams(20, -1, "Id", "ASC")
            );
            var getAllLessonBlockHandler = new GetAllLessonBlocksHandler(
                _dbService.Object,
                _str.Object,
                _strFields.Object
            );
            var result = await Assert.ThrowsAsync<ApiErrorException>(
                async () =>
                    await getAllLessonBlockHandler.Handle(
                        getAllLessonBlockQuery,
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
            var getAllLessonBlockQuery = new GetAllLessonBlocksQuery(
                new Guid(),
                null,
                null,
                new Guid(),
                "Admin",
                new PaginationParams(20, 0, "wrong", "ASC")
            );
            var getAllLessonBlockHandler = new GetAllLessonBlocksHandler(
                _dbService.Object,
                _str.Object,
                _strFields.Object
            );
            var result = await Assert.ThrowsAsync<ApiErrorException>(
                async () =>
                    await getAllLessonBlockHandler.Handle(
                        getAllLessonBlockQuery,
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
            var getAllLessonBlockQuery = new GetAllLessonBlocksQuery(
                new Guid(),
                null,
                null,
                new Guid(),
                "Admin",
                new PaginationParams(20, 0, "Id", "wrong")
            );
            var getAllLessonBlockHandler = new GetAllLessonBlocksHandler(
                _dbService.Object,
                _str.Object,
                _strFields.Object
            );
            var result = await Assert.ThrowsAsync<ApiErrorException>(
                async () =>
                    await getAllLessonBlockHandler.Handle(
                        getAllLessonBlockQuery,
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
            var getAllLessonBlockQuery = new GetAllLessonBlocksQuery(
                new Guid(),
                null,
                null,
                new Guid(),
                "Admin",
                new PaginationParams(20, 0, "wrong", "wrong")
            );
            var getAllLessonBlockHandler = new GetAllLessonBlocksHandler(
                _dbService.Object,
                _str.Object,
                _strFields.Object
            );
            var result = await Assert.ThrowsAsync<ApiErrorException>(
                async () =>
                    await getAllLessonBlockHandler.Handle(
                        getAllLessonBlockQuery,
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

        [Fact]
        public async Task ThrowsErrorWhenFromDateIsAfterToDate()
        {
            _dbService
                .Setup(s => s.Get<ScheduleSettings>(It.IsAny<string>(), It.IsAny<object>()))
                .ReturnsAsync(MockData.GetScheduleSettingsDetails());

            var getAllLessonBlockQuery = new GetAllLessonBlocksQuery(
                new Guid(),
                new DateOnly(2000, 10, 10),
                new DateOnly(2000, 1, 1),
                new Guid(),
                "Admin",
                new PaginationParams(20, 0, "Id", "ASC")
            );
            var getAllLessonBlockHandler = new GetAllLessonBlocksHandler(
                _dbService.Object,
                _str.Object,
                _strFields.Object
            );
            var result = await Assert.ThrowsAsync<ApiErrorException>(
                async () =>
                    await getAllLessonBlockHandler.Handle(
                        getAllLessonBlockQuery,
                        new CancellationToken()
                    )
            );

            Assert.Equal(
                JsonConvert.SerializeObject(
                    new ErrorObject[]
                    {
                        new ErrorObject("The end date cannot happen before the start date")
                    }
                ),
                JsonConvert.SerializeObject(result.errors)
            );
        }
    }
}
