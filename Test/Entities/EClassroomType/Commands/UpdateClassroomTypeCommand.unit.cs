using AlpimiAPI.Database;
using AlpimiAPI.Entities.EClassroomType;
using AlpimiAPI.Entities.EClassroomType.Commands;
using AlpimiAPI.Locales;
using AlpimiAPI.Responses;
using AlpimiTest.TestSetup;
using AlpimiTest.TestUtilities;
using Microsoft.Extensions.Localization;
using Moq;
using Xunit;

namespace AlpimiTest.Entities.EClassroomType.Commands
{
    [Collection("Sequential Tests")]
    public class UpdateClassroomTypeCommandUnit
    {
        private readonly Mock<IDbService> _dbService = new();
        private readonly Mock<IStringLocalizer<Errors>> _str;

        public UpdateClassroomTypeCommandUnit()
        {
            _str = ResourceSetup.Setup();
        }

        [Fact]
        public async Task ThrowsErrorWhenNameIsAlreadyTaken()
        {
            var dto = MockData.GetUpdateClassroomTypeDTODetails();
            _dbService
                .Setup(s => s.Get<ClassroomType>(It.IsAny<string>(), It.IsAny<object>()))
                .ReturnsAsync(MockData.GetClassroomTypeDetails());
            _dbService
                .Setup(s => s.GetAll<ClassroomType>(It.IsAny<string>(), It.IsAny<object>()))
                .ReturnsAsync(new List<ClassroomType> { MockData.GetClassroomTypeDetails() });

            var updateClassroomTypeCommand = new UpdateClassroomTypeCommand(
                new Guid(),
                dto,
                new Guid(),
                "Admin"
            );
            var updateClassroomTypeHandler = new UpdateClassroomTypeHandler(
                _dbService.Object,
                _str.Object
            );
            var result = await Assert.ThrowsAsync<ApiErrorException>(
                async () =>
                    await updateClassroomTypeHandler.Handle(
                        updateClassroomTypeCommand,
                        new CancellationToken()
                    )
            );

            Assert.Equal(
                "There is already a ClassroomType with the name Hartowanie",
                result.errors.First().message
            );
        }
    }
}
