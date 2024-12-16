using AlpimiAPI.Database;
using AlpimiAPI.Entities.ELesson.Queries;
using AlpimiAPI.Locales;
using AlpimiAPI.Responses;
using AlpimiTest.TestSetup;
using Microsoft.Extensions.Localization;
using Moq;
using Newtonsoft.Json;
using Xunit;

namespace AlpimiTest.Entities.ELesson.Queries
{
    [Collection("Sequential Tests")]
    public class GetAllLessonsQueryUnit
    {
        private readonly Mock<IDbService> _dbService = new();
        private readonly Mock<IStringLocalizer<Errors>> _str;

        public GetAllLessonsQueryUnit()
        {
            _str = ResourceSetup.Setup();
        }

        [Fact]
        public async Task ThrowsErrorWhenIncorrectPerPageIsGiven()
        {
            var getAllLessonQuery = new GetAllLessonsQuery(
                new Guid(),
                new Guid(),
                "Admin",
                new PaginationParams(-20, 0, "Id", "ASC")
            );
            var getAllLessonHandler = new GetAllLessonsHandler(_dbService.Object, _str.Object);
            var result = await Assert.ThrowsAsync<ApiErrorException>(
                async () =>
                    await getAllLessonHandler.Handle(getAllLessonQuery, new CancellationToken())
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
            var getAllLessonQuery = new GetAllLessonsQuery(
                new Guid(),
                new Guid(),
                "Admin",
                new PaginationParams(20, -1, "Id", "ASC")
            );
            var getAllLessonHandler = new GetAllLessonsHandler(_dbService.Object, _str.Object);
            var result = await Assert.ThrowsAsync<ApiErrorException>(
                async () =>
                    await getAllLessonHandler.Handle(getAllLessonQuery, new CancellationToken())
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
            var getAllLessonQuery = new GetAllLessonsQuery(
                new Guid(),
                new Guid(),
                "Admin",
                new PaginationParams(20, 0, "wrong", "ASC")
            );
            var getAllLessonHandler = new GetAllLessonsHandler(_dbService.Object, _str.Object);
            var result = await Assert.ThrowsAsync<ApiErrorException>(
                async () =>
                    await getAllLessonHandler.Handle(getAllLessonQuery, new CancellationToken())
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
            var getAllLessonQuery = new GetAllLessonsQuery(
                new Guid(),
                new Guid(),
                "Admin",
                new PaginationParams(20, 0, "Id", "wrong")
            );
            var getAllLessonHandler = new GetAllLessonsHandler(_dbService.Object, _str.Object);
            var result = await Assert.ThrowsAsync<ApiErrorException>(
                async () =>
                    await getAllLessonHandler.Handle(getAllLessonQuery, new CancellationToken())
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
            var getAllLessonQuery = new GetAllLessonsQuery(
                new Guid(),
                new Guid(),
                "Admin",
                new PaginationParams(20, 0, "wrong", "wrong")
            );
            var getAllLessonHandler = new GetAllLessonsHandler(_dbService.Object, _str.Object);
            var result = await Assert.ThrowsAsync<ApiErrorException>(
                async () =>
                    await getAllLessonHandler.Handle(getAllLessonQuery, new CancellationToken())
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
