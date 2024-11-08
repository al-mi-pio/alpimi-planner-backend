using AlpimiAPI.Database;
using AlpimiAPI.Entities.ESchedule;
using AlpimiAPI.Entities.ESchedule.Queries;
using AlpimiAPI.Entities.EUser;
using AlpimiAPI.Responses;
using AlpimiTest.TestUtilities;
using Moq;
using Newtonsoft.Json;
using Xunit;

namespace alpimi_planner_backend.Test.Entities.ESchedule.Queries
{
    public class GetScheduleCommandUnit
    {
        private readonly Mock<IDbService> _dbService = new();

        [Fact]
        public async Task GetsSchedules()
        {
            var schedules = MockData.GetSchedulesDetails();

            _dbService
                .Setup(s => s.GetAll<Schedule>(It.IsAny<string>(), It.IsAny<object>()))
                .ReturnsAsync(schedules);

            var getSchedulesCommand = new GetSchedulesQuery(
                new Guid(),
                "Admin",
                new PaginationParams(20, 0, "Id", "ASC")
            );

            var getSchedulesHandler = new GetSchedulesHandler(_dbService.Object);

            var result = await getSchedulesHandler.Handle(
                getSchedulesCommand,
                new CancellationToken()
            );

            Assert.Equal(schedules, result.Item1);
        }

        [Fact]
        public async Task ReturnsEmptyWhenWrongUserGetsSchedules()
        {
            IEnumerable<Schedule> schedules = Enumerable.Empty<Schedule>();

            _dbService
                .Setup(s => s.GetAll<Schedule>(It.IsAny<string>(), It.IsAny<object>()))
                .ReturnsAsync(schedules);

            var getSchedulesCommand = new GetSchedulesQuery(
                new Guid(),
                "Admin",
                new PaginationParams(20, 0, "Id", "ASC")
            );
            var getSchedulesHandler = new GetSchedulesHandler(_dbService.Object);

            var result = await getSchedulesHandler.Handle(
                getSchedulesCommand,
                new CancellationToken()
            );

            Assert.Empty(result.Item1!);
        }

        [Fact]
        public async Task ThrowsErrorWhenIncorrectPerPageIsGiven()
        {
            IEnumerable<Schedule> schedules = Enumerable.Empty<Schedule>();

            _dbService
                .Setup(s => s.GetAll<Schedule>(It.IsAny<string>(), It.IsAny<object>()))
                .ReturnsAsync(schedules);

            var getSchedulesCommand = new GetSchedulesQuery(
                new Guid(),
                "Admin",
                new PaginationParams(-20, 0, "Id", "ASC")
            );
            var getSchedulesHandler = new GetSchedulesHandler(_dbService.Object);

            var result = await Assert.ThrowsAsync<ApiErrorException>(
                async () =>
                    await getSchedulesHandler.Handle(getSchedulesCommand, new CancellationToken())
            );

            Assert.Equal(
                JsonConvert.SerializeObject(new ErrorObject[] { new ErrorObject("Bad PerPage") }),
                JsonConvert.SerializeObject(result.errors)
            );
        }

        [Fact]
        public async Task ThrowsErrorWhenIncorrectPageIsGiven()
        {
            IEnumerable<Schedule> schedules = Enumerable.Empty<Schedule>();

            _dbService
                .Setup(s => s.GetAll<Schedule>(It.IsAny<string>(), It.IsAny<object>()))
                .ReturnsAsync(schedules);

            var getSchedulesCommand = new GetSchedulesQuery(
                new Guid(),
                "Admin",
                new PaginationParams(20, -1, "Id", "ASC")
            );
            var getSchedulesHandler = new GetSchedulesHandler(_dbService.Object);

            var result = await Assert.ThrowsAsync<ApiErrorException>(
                async () =>
                    await getSchedulesHandler.Handle(getSchedulesCommand, new CancellationToken())
            );

            Assert.Equal(
                JsonConvert.SerializeObject(new ErrorObject[] { new ErrorObject("Bad Page") }),
                JsonConvert.SerializeObject(result.errors)
            );
        }

        [Fact]
        public async Task ThrowsErrorWhenIncorrectSortByIsGiven()
        {
            IEnumerable<Schedule> schedules = Enumerable.Empty<Schedule>();

            _dbService
                .Setup(s => s.GetAll<Schedule>(It.IsAny<string>(), It.IsAny<object>()))
                .ReturnsAsync(schedules);

            var getSchedulesCommand = new GetSchedulesQuery(
                new Guid(),
                "Admin",
                new PaginationParams(20, 0, "wrong", "ASC")
            );
            var getSchedulesHandler = new GetSchedulesHandler(_dbService.Object);

            var result = await Assert.ThrowsAsync<ApiErrorException>(
                async () =>
                    await getSchedulesHandler.Handle(getSchedulesCommand, new CancellationToken())
            );

            Assert.Equal(
                JsonConvert.SerializeObject(new ErrorObject[] { new ErrorObject("Bad SortBy") }),
                JsonConvert.SerializeObject(result.errors)
            );
        }

        [Fact]
        public async Task ThrowsErrorWhenIncorrectSortOrderIsGiven()
        {
            IEnumerable<Schedule> schedules = Enumerable.Empty<Schedule>();

            _dbService
                .Setup(s => s.GetAll<Schedule>(It.IsAny<string>(), It.IsAny<object>()))
                .ReturnsAsync(schedules);

            var getSchedulesCommand = new GetSchedulesQuery(
                new Guid(),
                "Admin",
                new PaginationParams(20, 0, "Id", "wrong")
            );
            var getSchedulesHandler = new GetSchedulesHandler(_dbService.Object);

            var result = await Assert.ThrowsAsync<ApiErrorException>(
                async () =>
                    await getSchedulesHandler.Handle(getSchedulesCommand, new CancellationToken())
            );

            Assert.Equal(
                JsonConvert.SerializeObject(new ErrorObject[] { new ErrorObject("Bad SortOrder") }),
                JsonConvert.SerializeObject(result.errors)
            );
        }

        [Fact]
        public async Task ThrowsMultipleErrorMessages()
        {
            IEnumerable<Schedule> schedules = Enumerable.Empty<Schedule>();

            _dbService
                .Setup(s => s.GetAll<Schedule>(It.IsAny<string>(), It.IsAny<object>()))
                .ReturnsAsync(schedules);

            var getSchedulesCommand = new GetSchedulesQuery(
                new Guid(),
                "Admin",
                new PaginationParams(20, 0, "wrong", "wrong")
            );
            var getSchedulesHandler = new GetSchedulesHandler(_dbService.Object);

            var result = await Assert.ThrowsAsync<ApiErrorException>(
                async () =>
                    await getSchedulesHandler.Handle(getSchedulesCommand, new CancellationToken())
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
