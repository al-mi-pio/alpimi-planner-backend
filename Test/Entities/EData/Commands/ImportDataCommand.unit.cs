using System.Diagnostics;
using System.Text.Json;
using AlpimiAPI.Database;
using AlpimiAPI.Entities.EAvailability;
using AlpimiAPI.Entities.EClassroom;
using AlpimiAPI.Entities.EClassroomType;
using AlpimiAPI.Entities.ECollisionType;
using AlpimiAPI.Entities.ECollisionType.Commands;
using AlpimiAPI.Entities.EData.Commands;
using AlpimiAPI.Entities.EData.DTO;
using AlpimiAPI.Entities.EDayOff;
using AlpimiAPI.Entities.EDayOff.Commands;
using AlpimiAPI.Entities.EGroup;
using AlpimiAPI.Entities.ELesson;
using AlpimiAPI.Entities.ELessonPeriod;
using AlpimiAPI.Entities.ELessonType;
using AlpimiAPI.Entities.ESchedule;
using AlpimiAPI.Entities.EScheduleSettings;
using AlpimiAPI.Entities.EStudent;
using AlpimiAPI.Entities.ESubgroup;
using AlpimiAPI.Entities.ETeacher;
using AlpimiAPI.Locales;
using AlpimiAPI.Responses;
using AlpimiTest.TestSetup;
using AlpimiTest.TestUtilities;
using Azure;
using Microsoft.Extensions.Localization;
using Moq;
using Xunit;

namespace AlpimiTest.Entities.EData.Commands
{
    [Collection("Sequential Tests")]
    public class ImportDataCommandUnit
    {
        private readonly Mock<IDbService> _dbService = new();
        private readonly Mock<IStringLocalizer<Errors>> _str;
        private readonly Mock<IStringLocalizer<Fields>> _strFields;
        private readonly Mock<IStringLocalizer<Data>> _strData;

        public ImportDataCommandUnit()
        {
            _str = ResourceSetup.Setup();
            _strFields = ResourceSetup.FieldSetup();
            _strData = ResourceSetup.DataSetup();
        }

        [Fact]
        public async Task ImportDataReturnsMultipleBadImportDataErrors()
        {
            _dbService
                .Setup(s => s.Get<Schedule>(It.IsAny<string>(), It.IsAny<object>()))
                .ReturnsAsync(MockData.GetScheduleDetails());

            var dto = new ImportDataDTO()
            {
                Payload = MockXMLs.GetBadDataXML(),
                ScheduleId = new Guid()
            };
            var importDataCommand = new ImportDataCommand(dto, new Guid(), "Admin");
            var importDataHandler = new ImportDataHandler(
                _dbService.Object,
                _str.Object,
                _strFields.Object,
                _strData.Object
            );
            var result = await importDataHandler.Handle(importDataCommand, new CancellationToken());
            string jsonString = JsonSerializer.Serialize(result);
            Assert.Equal(2, result.successfulItems);
            Assert.Contains("lessonPeriod", jsonString);
            Assert.Contains("dayOff", jsonString);
            Assert.Contains("lessonType", jsonString);
            Assert.Contains("group", jsonString);
            Assert.Contains("subgroup", jsonString);
            Assert.Contains("classroom", jsonString);
            Assert.Contains("lesson", jsonString);
            Assert.Contains("availability", jsonString);
            Assert.Contains("student", jsonString);
        }

        [Fact]
        public async Task ImportDataReturnsErrorsWhenBadReferencesAreGiven()
        {
            _dbService
                .Setup(s => s.Get<Schedule>(It.IsAny<string>(), It.IsAny<object>()))
                .ReturnsAsync(MockData.GetScheduleDetails());
            _dbService
                .Setup(s => s.Get<ScheduleSettings>(It.IsAny<string>(), It.IsAny<object>()))
                .ReturnsAsync(MockData.GetScheduleSettingsDetails());

            var dto = new ImportDataDTO()
            {
                Payload = MockXMLs.GetBadReferencesXML(),
                ScheduleId = new Guid()
            };
            var importDataCommand = new ImportDataCommand(dto, new Guid(), "Admin");
            var importDataHandler = new ImportDataHandler(
                _dbService.Object,
                _str.Object,
                _strFields.Object,
                _strData.Object
            );
            var result = await importDataHandler.Handle(importDataCommand, new CancellationToken());
            string jsonString = JsonSerializer.Serialize(result);
            Assert.Equal(8, result.successfulItems);
            Assert.Contains("subgroup", jsonString);
            Assert.Contains("classroom", jsonString);
            Assert.Contains("lesson", jsonString);
            Assert.Contains("availability", jsonString);
            Assert.Contains("student", jsonString);
        }
    }
}
