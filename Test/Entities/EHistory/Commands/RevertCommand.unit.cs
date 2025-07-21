using AlpimiAPI.Database;
using AlpimiAPI.Entities.EHistory.Commands;
using AlpimiAPI.Locales;
using AlpimiAPI.Responses;
using AlpimiTest.TestSetup;
using Microsoft.Extensions.Localization;
using Moq;
using Newtonsoft.Json;
using Xunit;

namespace AlpimiTest.Entities.EHistory.Commands
{
    [Collection("Sequential Tests")]
    public class CreateHistoryCommandUnit
    {
        private readonly Mock<IDbService> _dbService = new();
        private readonly Mock<IStringLocalizer<Errors>> _str;
        private readonly Mock<IStringLocalizer<Fields>> _strFields;

        public CreateHistoryCommandUnit()
        {
            _str = ResourceSetup.Setup();
            _strFields = ResourceSetup.FieldSetup();
        }

        [Fact]
        public async Task ThrowsErrorWhenThereAreNoChangesToUndo()
        {
            var revertCommand = new RevertCommand(new Guid(), false, new Guid(), "User");
            var revertHandler = new RevertCommandHandler(
                _dbService.Object,
                _str.Object,
                _strFields.Object
            );
            var result = await Assert.ThrowsAsync<ApiErrorException>(
                async () => await revertHandler.Handle(revertCommand, new CancellationToken())
            );

            Assert.Equal(
                JsonConvert.SerializeObject(
                    new ErrorObject[] { new ErrorObject("There is nothing to undo") }
                ),
                JsonConvert.SerializeObject(result.errors)
            );
        }

        [Fact]
        public async Task ThrowsErrorWhenThereAreNoChangesToRedo()
        {
            var revertCommand = new RevertCommand(new Guid(), true, new Guid(), "User");
            var revertHandler = new RevertCommandHandler(
                _dbService.Object,
                _str.Object,
                _strFields.Object
            );
            var result = await Assert.ThrowsAsync<ApiErrorException>(
                async () => await revertHandler.Handle(revertCommand, new CancellationToken())
            );

            Assert.Equal(
                JsonConvert.SerializeObject(
                    new ErrorObject[] { new ErrorObject("There is nothing to redo") }
                ),
                JsonConvert.SerializeObject(result.errors)
            );
        }
    }
}
