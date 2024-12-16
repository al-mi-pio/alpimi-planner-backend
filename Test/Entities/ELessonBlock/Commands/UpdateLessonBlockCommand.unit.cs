using AlpimiAPI.Database;
using AlpimiAPI.Entities.EClassroom;
using AlpimiAPI.Entities.ELesson;
using AlpimiAPI.Entities.ELessonBlock;
using AlpimiAPI.Entities.ELessonBlock.Commands;
using AlpimiAPI.Entities.ELessonType;
using AlpimiAPI.Entities.EScheduleSettings;
using AlpimiAPI.Entities.ETeacher;
using AlpimiAPI.Locales;
using AlpimiAPI.Responses;
using AlpimiTest.TestSetup;
using AlpimiTest.TestUtilities;
using Microsoft.Extensions.Localization;
using Moq;
using Newtonsoft.Json;
using Xunit;

namespace AlpimiTest.Entities.ELessonBlock.Commands
{
    [Collection("Sequential Tests")]
    public class UpdateLessonBlockCommandUnit
    {
        private readonly Mock<IDbService> _dbService = new();
        private readonly Mock<IStringLocalizer<Errors>> _str;

        public UpdateLessonBlockCommandUnit()
        {
            _str = ResourceSetup.Setup();
        }

        [Fact]
        public async Task ThrowsErrorWhenWrongTeacherIdIsGiven()
        {
            _dbService
                .Setup(s => s.Get<LessonBlock>(It.IsAny<string>(), It.IsAny<object>()))
                .ReturnsAsync(MockData.GetLessonBlockDetails());
            _dbService
                .Setup(s => s.Get<Lesson>(It.IsAny<string>(), It.IsAny<object>()))
                .ReturnsAsync(MockData.GetLessonDetails());
            _dbService
                .Setup(s => s.Get<LessonType>(It.IsAny<string>(), It.IsAny<object>()))
                .ReturnsAsync(MockData.GetLessonTypeDetails());
            _dbService
                .Setup(s => s.Get<Classroom>(It.IsAny<string>(), It.IsAny<object>()))
                .ReturnsAsync(MockData.GetClassroomDetails());
            _dbService
                .Setup(s => s.Get<ScheduleSettings>(It.IsAny<string>(), It.IsAny<object>()))
                .ReturnsAsync(MockData.GetScheduleSettingsDetails());
            _dbService
                .Setup(s => s.Get<int>(It.IsAny<string>(), It.IsAny<object>()))
                .ReturnsAsync(5);

            var updateLessonBlockCommand = new UpdateLessonBlockCommand(
                new Guid(),
                MockData.GetUpdateLessonBlockDTODetails(),
                new Guid(),
                "User"
            );
            var updateLessonBlockHandler = new UpdateLessonBlockHandler(
                _dbService.Object,
                _str.Object
            );
            var result = await Assert.ThrowsAsync<ApiErrorException>(
                async () =>
                    await updateLessonBlockHandler.Handle(
                        updateLessonBlockCommand,
                        new CancellationToken()
                    )
            );

            Assert.Equal(
                JsonConvert.SerializeObject(
                    new ErrorObject[]
                    {
                        new ErrorObject(
                            "Teacher with id 00000000-0000-0000-0000-000000000000 was not found"
                        )
                    }
                ),
                JsonConvert.SerializeObject(result.errors)
            );
        }

        [Fact]
        public async Task ThrowsErrorWhenScheduleIdsFromTeacherAndClassroomDontMatch()
        {
            var teacher = MockData.GetTeacherDetails();
            teacher.ScheduleId = Guid.NewGuid();
            _dbService
                .Setup(s => s.Get<LessonBlock>(It.IsAny<string>(), It.IsAny<object>()))
                .ReturnsAsync(MockData.GetLessonBlockDetails());
            _dbService
                .Setup(s => s.Get<Lesson>(It.IsAny<string>(), It.IsAny<object>()))
                .ReturnsAsync(MockData.GetLessonDetails());
            _dbService
                .Setup(s => s.Get<LessonType>(It.IsAny<string>(), It.IsAny<object>()))
                .ReturnsAsync(MockData.GetLessonTypeDetails());
            _dbService
                .Setup(s => s.Get<Teacher>(It.IsAny<string>(), It.IsAny<object>()))
                .ReturnsAsync(teacher);
            _dbService
                .Setup(s => s.Get<Classroom>(It.IsAny<string>(), It.IsAny<object>()))
                .ReturnsAsync(MockData.GetClassroomDetails());
            _dbService
                .Setup(s => s.Get<ScheduleSettings>(It.IsAny<string>(), It.IsAny<object>()))
                .ReturnsAsync(MockData.GetScheduleSettingsDetails());
            _dbService
                .Setup(s => s.Get<int>(It.IsAny<string>(), It.IsAny<object>()))
                .ReturnsAsync(5);

            var updateLessonBlockCommand = new UpdateLessonBlockCommand(
                new Guid(),
                MockData.GetUpdateLessonBlockDTODetails(),
                new Guid(),
                "User"
            );
            var updateLessonBlockHandler = new UpdateLessonBlockHandler(
                _dbService.Object,
                _str.Object
            );
            var result = await Assert.ThrowsAsync<ApiErrorException>(
                async () =>
                    await updateLessonBlockHandler.Handle(
                        updateLessonBlockCommand,
                        new CancellationToken()
                    )
            );

            Assert.Equal(
                JsonConvert.SerializeObject(
                    new ErrorObject[]
                    {
                        new ErrorObject("Teacher must be in the same Schedule as Lesson")
                    }
                ),
                JsonConvert.SerializeObject(result.errors)
            );
        }

        [Fact]
        public async Task ThrowsErrorWhenWrongClassroomIdIsGiven()
        {
            _dbService
                .Setup(s => s.Get<LessonBlock>(It.IsAny<string>(), It.IsAny<object>()))
                .ReturnsAsync(MockData.GetLessonBlockDetails());
            _dbService
                .Setup(s => s.Get<Lesson>(It.IsAny<string>(), It.IsAny<object>()))
                .ReturnsAsync(MockData.GetLessonDetails());
            _dbService
                .Setup(s => s.Get<LessonType>(It.IsAny<string>(), It.IsAny<object>()))
                .ReturnsAsync(MockData.GetLessonTypeDetails());
            _dbService
                .Setup(s => s.Get<Teacher>(It.IsAny<string>(), It.IsAny<object>()))
                .ReturnsAsync(MockData.GetTeacherDetails());
            _dbService
                .Setup(s => s.Get<ScheduleSettings>(It.IsAny<string>(), It.IsAny<object>()))
                .ReturnsAsync(MockData.GetScheduleSettingsDetails());
            _dbService
                .Setup(s => s.Get<int>(It.IsAny<string>(), It.IsAny<object>()))
                .ReturnsAsync(5);

            var updateLessonBlockCommand = new UpdateLessonBlockCommand(
                new Guid(),
                MockData.GetUpdateLessonBlockDTODetails(),
                new Guid(),
                "User"
            );
            var updateLessonBlockHandler = new UpdateLessonBlockHandler(
                _dbService.Object,
                _str.Object
            );
            var result = await Assert.ThrowsAsync<ApiErrorException>(
                async () =>
                    await updateLessonBlockHandler.Handle(
                        updateLessonBlockCommand,
                        new CancellationToken()
                    )
            );

            Assert.Equal(
                JsonConvert.SerializeObject(
                    new ErrorObject[]
                    {
                        new ErrorObject(
                            "Classroom with id 00000000-0000-0000-0000-000000000000 was not found"
                        )
                    }
                ),
                JsonConvert.SerializeObject(result.errors)
            );
        }

        [Fact]
        public async Task ThrowsErrorWhenScheduleIdsFromLessonAndClassroomDontMatch()
        {
            var classroom = MockData.GetClassroomDetails();
            classroom.ScheduleId = Guid.NewGuid();
            _dbService
                .Setup(s => s.Get<LessonBlock>(It.IsAny<string>(), It.IsAny<object>()))
                .ReturnsAsync(MockData.GetLessonBlockDetails());
            _dbService
                .Setup(s => s.Get<Lesson>(It.IsAny<string>(), It.IsAny<object>()))
                .ReturnsAsync(MockData.GetLessonDetails());
            _dbService
                .Setup(s => s.Get<LessonType>(It.IsAny<string>(), It.IsAny<object>()))
                .ReturnsAsync(MockData.GetLessonTypeDetails());
            _dbService
                .Setup(s => s.Get<Teacher>(It.IsAny<string>(), It.IsAny<object>()))
                .ReturnsAsync(MockData.GetTeacherDetails());
            _dbService
                .Setup(s => s.Get<Classroom>(It.IsAny<string>(), It.IsAny<object>()))
                .ReturnsAsync(classroom);
            _dbService
                .Setup(s => s.Get<ScheduleSettings>(It.IsAny<string>(), It.IsAny<object>()))
                .ReturnsAsync(MockData.GetScheduleSettingsDetails());
            _dbService
                .Setup(s => s.Get<int>(It.IsAny<string>(), It.IsAny<object>()))
                .ReturnsAsync(5);

            var updateLessonBlockCommand = new UpdateLessonBlockCommand(
                new Guid(),
                MockData.GetUpdateLessonBlockDTODetails(),
                new Guid(),
                "User"
            );
            var updateLessonBlockHandler = new UpdateLessonBlockHandler(
                _dbService.Object,
                _str.Object
            );
            var result = await Assert.ThrowsAsync<ApiErrorException>(
                async () =>
                    await updateLessonBlockHandler.Handle(
                        updateLessonBlockCommand,
                        new CancellationToken()
                    )
            );

            Assert.Equal(
                JsonConvert.SerializeObject(
                    new ErrorObject[]
                    {
                        new ErrorObject("Classroom must be in the same Schedule as Lesson")
                    }
                ),
                JsonConvert.SerializeObject(result.errors)
            );
        }

        [Fact]
        public async Task ThrowsErrorWhenLessonStartIsAfterLessonEnd()
        {
            _dbService
                .Setup(s => s.Get<LessonBlock>(It.IsAny<string>(), It.IsAny<object>()))
                .ReturnsAsync(MockData.GetLessonBlockDetails());
            _dbService
                .Setup(s => s.Get<Lesson>(It.IsAny<string>(), It.IsAny<object>()))
                .ReturnsAsync(MockData.GetLessonDetails());
            _dbService
                .Setup(s => s.Get<LessonType>(It.IsAny<string>(), It.IsAny<object>()))
                .ReturnsAsync(MockData.GetLessonTypeDetails());
            _dbService
                .Setup(s => s.Get<Teacher>(It.IsAny<string>(), It.IsAny<object>()))
                .ReturnsAsync(MockData.GetTeacherDetails());
            _dbService
                .Setup(s => s.Get<Classroom>(It.IsAny<string>(), It.IsAny<object>()))
                .ReturnsAsync(MockData.GetClassroomDetails());
            _dbService
                .Setup(s => s.Get<ScheduleSettings>(It.IsAny<string>(), It.IsAny<object>()))
                .ReturnsAsync(MockData.GetScheduleSettingsDetails());
            _dbService
                .Setup(s => s.Get<int>(It.IsAny<string>(), It.IsAny<object>()))
                .ReturnsAsync(5);

            var updateLessonRequest = MockData.GetUpdateLessonBlockDTODetails();
            updateLessonRequest.LessonEnd = 3;
            updateLessonRequest.LessonStart = 4;
            var updateLessonBlockCommand = new UpdateLessonBlockCommand(
                new Guid(),
                updateLessonRequest,
                new Guid(),
                "User"
            );
            var updateLessonBlockHandler = new UpdateLessonBlockHandler(
                _dbService.Object,
                _str.Object
            );
            var result = await Assert.ThrowsAsync<ApiErrorException>(
                async () =>
                    await updateLessonBlockHandler.Handle(
                        updateLessonBlockCommand,
                        new CancellationToken()
                    )
            );

            Assert.Equal(
                JsonConvert.SerializeObject(
                    new ErrorObject[]
                    {
                        new ErrorObject("The end time cannot happen before the start time")
                    }
                ),
                JsonConvert.SerializeObject(result.errors)
            );
        }

        [Fact]
        public async Task ThrowsErrorWhenLessonStartIsLessThan1()
        {
            _dbService
                .Setup(s => s.Get<LessonBlock>(It.IsAny<string>(), It.IsAny<object>()))
                .ReturnsAsync(MockData.GetLessonBlockDetails());
            _dbService
                .Setup(s => s.Get<Lesson>(It.IsAny<string>(), It.IsAny<object>()))
                .ReturnsAsync(MockData.GetLessonDetails());
            _dbService
                .Setup(s => s.Get<LessonType>(It.IsAny<string>(), It.IsAny<object>()))
                .ReturnsAsync(MockData.GetLessonTypeDetails());
            _dbService
                .Setup(s => s.Get<Teacher>(It.IsAny<string>(), It.IsAny<object>()))
                .ReturnsAsync(MockData.GetTeacherDetails());
            _dbService
                .Setup(s => s.Get<Classroom>(It.IsAny<string>(), It.IsAny<object>()))
                .ReturnsAsync(MockData.GetClassroomDetails());
            _dbService
                .Setup(s => s.Get<ScheduleSettings>(It.IsAny<string>(), It.IsAny<object>()))
                .ReturnsAsync(MockData.GetScheduleSettingsDetails());
            _dbService
                .Setup(s => s.Get<int>(It.IsAny<string>(), It.IsAny<object>()))
                .ReturnsAsync(5);

            var updateLessonRequest = MockData.GetUpdateLessonBlockDTODetails();
            updateLessonRequest.LessonStart = 0;
            var updateLessonBlockCommand = new UpdateLessonBlockCommand(
                new Guid(),
                updateLessonRequest,
                new Guid(),
                "User"
            );
            var updateLessonBlockHandler = new UpdateLessonBlockHandler(
                _dbService.Object,
                _str.Object
            );
            var result = await Assert.ThrowsAsync<ApiErrorException>(
                async () =>
                    await updateLessonBlockHandler.Handle(
                        updateLessonBlockCommand,
                        new CancellationToken()
                    )
            );

            Assert.Equal(
                JsonConvert.SerializeObject(
                    new ErrorObject[] { new ErrorObject("LessonStart parameter is invalid") }
                ),
                JsonConvert.SerializeObject(result.errors)
            );
        }

        [Fact]
        public async Task ThrowsErrorWhenLessonEndIsMoreThanTheAmountOfLessonPeriods()
        {
            _dbService
                .Setup(s => s.Get<LessonBlock>(It.IsAny<string>(), It.IsAny<object>()))
                .ReturnsAsync(MockData.GetLessonBlockDetails());
            _dbService
                .Setup(s => s.Get<Lesson>(It.IsAny<string>(), It.IsAny<object>()))
                .ReturnsAsync(MockData.GetLessonDetails());
            _dbService
                .Setup(s => s.Get<LessonType>(It.IsAny<string>(), It.IsAny<object>()))
                .ReturnsAsync(MockData.GetLessonTypeDetails());
            _dbService
                .Setup(s => s.Get<Teacher>(It.IsAny<string>(), It.IsAny<object>()))
                .ReturnsAsync(MockData.GetTeacherDetails());
            _dbService
                .Setup(s => s.Get<Classroom>(It.IsAny<string>(), It.IsAny<object>()))
                .ReturnsAsync(MockData.GetClassroomDetails());
            _dbService
                .Setup(s => s.Get<ScheduleSettings>(It.IsAny<string>(), It.IsAny<object>()))
                .ReturnsAsync(MockData.GetScheduleSettingsDetails());
            _dbService
                .Setup(s => s.Get<int>(It.IsAny<string>(), It.IsAny<object>()))
                .ReturnsAsync(5);

            var updateLessonRequest = MockData.GetUpdateLessonBlockDTODetails();
            updateLessonRequest.LessonEnd = 6;
            var updateLessonBlockCommand = new UpdateLessonBlockCommand(
                new Guid(),
                updateLessonRequest,
                new Guid(),
                "User"
            );
            var updateLessonBlockHandler = new UpdateLessonBlockHandler(
                _dbService.Object,
                _str.Object
            );
            var result = await Assert.ThrowsAsync<ApiErrorException>(
                async () =>
                    await updateLessonBlockHandler.Handle(
                        updateLessonBlockCommand,
                        new CancellationToken()
                    )
            );

            Assert.Equal(
                JsonConvert.SerializeObject(
                    new ErrorObject[] { new ErrorObject("LessonEnd parameter is invalid") }
                ),
                JsonConvert.SerializeObject(result.errors)
            );
        }

        [Fact]
        public async Task ThrowsErrorWhenLessonOccursOnADayOfTheWeekThatIsNotAllowedByScheduleSettings()
        {
            _dbService
                .Setup(s => s.Get<LessonBlock>(It.IsAny<string>(), It.IsAny<object>()))
                .ReturnsAsync(MockData.GetLessonBlockDetails());
            _dbService
                .Setup(s => s.Get<Lesson>(It.IsAny<string>(), It.IsAny<object>()))
                .ReturnsAsync(MockData.GetLessonDetails());
            _dbService
                .Setup(s => s.Get<LessonType>(It.IsAny<string>(), It.IsAny<object>()))
                .ReturnsAsync(MockData.GetLessonTypeDetails());
            _dbService
                .Setup(s => s.Get<Teacher>(It.IsAny<string>(), It.IsAny<object>()))
                .ReturnsAsync(MockData.GetTeacherDetails());
            _dbService
                .Setup(s => s.Get<Classroom>(It.IsAny<string>(), It.IsAny<object>()))
                .ReturnsAsync(MockData.GetClassroomDetails());
            _dbService
                .Setup(s => s.Get<ScheduleSettings>(It.IsAny<string>(), It.IsAny<object>()))
                .ReturnsAsync(MockData.GetScheduleSettingsDetails());
            _dbService
                .Setup(s => s.Get<int>(It.IsAny<string>(), It.IsAny<object>()))
                .ReturnsAsync(5);

            var updateLessonRequest = MockData.GetUpdateLessonBlockDTODetails();
            updateLessonRequest.WeekDay = 0;
            var updateLessonBlockCommand = new UpdateLessonBlockCommand(
                new Guid(),
                updateLessonRequest,
                new Guid(),
                "User"
            );
            var updateLessonBlockHandler = new UpdateLessonBlockHandler(
                _dbService.Object,
                _str.Object
            );
            var result = await Assert.ThrowsAsync<ApiErrorException>(
                async () =>
                    await updateLessonBlockHandler.Handle(
                        updateLessonBlockCommand,
                        new CancellationToken()
                    )
            );

            Assert.Equal(
                JsonConvert.SerializeObject(
                    new ErrorObject[] { new ErrorObject("Lessons cannot occur on Sunday") }
                ),
                JsonConvert.SerializeObject(result.errors)
            );
        }

        [Fact]
        public async Task ThrowsErrorWhenUpdatedLessonDateWouldOccurBeforeSchoolYearStart()
        {
            var lessonBlock = MockData.GetLessonBlockDetails();
            lessonBlock.LessonDate = new DateOnly(2024, 12, 16);
            var scheduleSettings = MockData.GetScheduleSettingsDetails();
            scheduleSettings.SchoolYearEnd = new DateOnly(2024, 12, 17);

            _dbService
                .Setup(s => s.Get<LessonBlock>(It.IsAny<string>(), It.IsAny<object>()))
                .ReturnsAsync(lessonBlock);
            _dbService
                .Setup(s => s.Get<Lesson>(It.IsAny<string>(), It.IsAny<object>()))
                .ReturnsAsync(MockData.GetLessonDetails());
            _dbService
                .Setup(s => s.Get<LessonType>(It.IsAny<string>(), It.IsAny<object>()))
                .ReturnsAsync(MockData.GetLessonTypeDetails());
            _dbService
                .Setup(s => s.Get<Teacher>(It.IsAny<string>(), It.IsAny<object>()))
                .ReturnsAsync(MockData.GetTeacherDetails());
            _dbService
                .Setup(s => s.Get<Classroom>(It.IsAny<string>(), It.IsAny<object>()))
                .ReturnsAsync(MockData.GetClassroomDetails());
            _dbService
                .Setup(s => s.Get<ScheduleSettings>(It.IsAny<string>(), It.IsAny<object>()))
                .ReturnsAsync(scheduleSettings);
            _dbService
                .Setup(s => s.Get<int>(It.IsAny<string>(), It.IsAny<object>()))
                .ReturnsAsync(5);

            var updateLessonRequest = MockData.GetUpdateLessonBlockDTODetails();
            updateLessonRequest.WeekDay = 5;
            var updateLessonBlockCommand = new UpdateLessonBlockCommand(
                new Guid(),
                updateLessonRequest,
                new Guid(),
                "User"
            );
            var updateLessonBlockHandler = new UpdateLessonBlockHandler(
                _dbService.Object,
                _str.Object
            );
            var result = await Assert.ThrowsAsync<ApiErrorException>(
                async () =>
                    await updateLessonBlockHandler.Handle(
                        updateLessonBlockCommand,
                        new CancellationToken()
                    )
            );

            Assert.Contains("Date must be in between", JsonConvert.SerializeObject(result.errors));
        }

        [Fact]
        public async Task ThrowsErrorWhenUpdatedLessonDateWouldOccurAfterSchoolYearEnd()
        {
            var lessonBlock = MockData.GetLessonBlockDetails();
            lessonBlock.LessonDate = new DateOnly(2024, 12, 13);
            var scheduleSettings = MockData.GetScheduleSettingsDetails();
            scheduleSettings.SchoolYearStart = new DateOnly(2024, 12, 12);
            _dbService
                .Setup(s => s.Get<LessonBlock>(It.IsAny<string>(), It.IsAny<object>()))
                .ReturnsAsync(lessonBlock);
            _dbService
                .Setup(s => s.Get<Lesson>(It.IsAny<string>(), It.IsAny<object>()))
                .ReturnsAsync(MockData.GetLessonDetails());
            _dbService
                .Setup(s => s.Get<LessonType>(It.IsAny<string>(), It.IsAny<object>()))
                .ReturnsAsync(MockData.GetLessonTypeDetails());
            _dbService
                .Setup(s => s.Get<Teacher>(It.IsAny<string>(), It.IsAny<object>()))
                .ReturnsAsync(MockData.GetTeacherDetails());
            _dbService
                .Setup(s => s.Get<Classroom>(It.IsAny<string>(), It.IsAny<object>()))
                .ReturnsAsync(MockData.GetClassroomDetails());
            _dbService
                .Setup(s => s.Get<ScheduleSettings>(It.IsAny<string>(), It.IsAny<object>()))
                .ReturnsAsync(scheduleSettings);
            _dbService
                .Setup(s => s.Get<int>(It.IsAny<string>(), It.IsAny<object>()))
                .ReturnsAsync(5);

            var updateLessonRequest = MockData.GetUpdateLessonBlockDTODetails();
            updateLessonRequest.WeekDay = 1;
            var updateLessonBlockCommand = new UpdateLessonBlockCommand(
                new Guid(),
                updateLessonRequest,
                new Guid(),
                "User"
            );
            var updateLessonBlockHandler = new UpdateLessonBlockHandler(
                _dbService.Object,
                _str.Object
            );
            var result = await Assert.ThrowsAsync<ApiErrorException>(
                async () =>
                    await updateLessonBlockHandler.Handle(
                        updateLessonBlockCommand,
                        new CancellationToken()
                    )
            );

            Assert.Contains("Date must be in between", JsonConvert.SerializeObject(result.errors));
        }
    }
}
