using AlpimiAPI.Database;
using AlpimiAPI.Entities.EGroup;
using AlpimiAPI.Entities.ESubgroup;
using AlpimiAPI.Entities.ESubgroup.Commands;
using AlpimiAPI.Locales;
using AlpimiAPI.Responses;
using AlpimiTest.TestSetup;
using AlpimiTest.TestUtilities;
using Microsoft.Extensions.Localization;
using Moq;
using Xunit;

namespace AlpimiTest.Entities.ESubgroup.Commands
{
    [Collection("Sequential Tests")]
    public class JoinSubgroupCommandUnit
    {
        private readonly Mock<IDbService> _dbService = new();
        private readonly Mock<IStringLocalizer<Errors>> _str;

        public JoinSubgroupCommandUnit()
        {
            _str = ResourceSetup.Setup();
        }

        [Fact]
        public async Task ThrowsErrorWhenDuplicatedSubgroupIdsAreGiven()
        {
            var joinSubgroupCommand = new JoinSubgroupCommand(
                new Guid(),
                [new Guid(), new Guid()],
                new Guid(),
                "Admin"
            );
            var joinSubgroupHandler = new JoinSubgroupHandler(_dbService.Object, _str.Object);
            var result = await Assert.ThrowsAsync<ApiErrorException>(
                async () =>
                    await joinSubgroupHandler.Handle(joinSubgroupCommand, new CancellationToken())
            );

            Assert.Equal(
                $"Cannot add multiple Subgroups with the value {new Guid()}",
                result.errors.First().message
            );
        }

        [Fact]
        public async Task ThrowsErrorWhenWrongSubgroupIdIsGiven()
        {
            var joinSubgroupCommand = new JoinSubgroupCommand(
                new Guid(),
                [new Guid()],
                new Guid(),
                "Admin"
            );
            var joinSubgroupHandler = new JoinSubgroupHandler(_dbService.Object, _str.Object);
            var result = await Assert.ThrowsAsync<ApiErrorException>(
                async () =>
                    await joinSubgroupHandler.Handle(joinSubgroupCommand, new CancellationToken())
            );

            Assert.Equal(
                $"Subgroup with id {new Guid()} was not found",
                result.errors.First().message
            );
        }
    }
}
