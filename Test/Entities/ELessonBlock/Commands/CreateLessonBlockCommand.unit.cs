using AlpimiAPI.Database;
using AlpimiAPI.Entities.EClassroom;
using AlpimiAPI.Entities.ELesson;
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
    public class CreateLessonBlockCommandUnit
    {
        private readonly Mock<IDbService> _dbService = new();
        private readonly Mock<IStringLocalizer<Errors>> _str;

        public CreateLessonBlockCommandUnit()
        {
            _str = ResourceSetup.Setup();
        }

        [Fact]
        public async Task ThrowsErrorWhenWrongLessonIdIsGiven()
        {
            _dbService
                .Setup(s => s.Get<Teacher>(It.IsAny<string>(), It.IsAny<object>()))
                .ReturnsAsync(MockData.GetTeacherDetails());
            _dbService
                .Setup(s => s.Get<Classroom>(It.IsAny<string>(), It.IsAny<object>()))
                .ReturnsAsync(MockData.GetClassroomDetails());

            var createLessonBlockCommand = new CreateLessonBlockCommand(
                new Guid(),
                new Guid(),
                MockData.GetCreateLessonBlockDTODetails(new Guid(), new Guid(), new Guid()),
                new Guid(),
                "User"
            );
            var createLessonBlockHandler = new CreateLessonBlockHandler(
                _dbService.Object,
                _str.Object
            );
            var result = await Assert.ThrowsAsync<ApiErrorException>(
                async () =>
                    await createLessonBlockHandler.Handle(
                        createLessonBlockCommand,
                        new CancellationToken()
                    )
            );

            Assert.Equal(
                JsonConvert.SerializeObject(
                    new ErrorObject[]
                    {
                        new ErrorObject(
                            "Lesson with id 00000000-0000-0000-0000-000000000000 was not found"
                        )
                    }
                ),
                JsonConvert.SerializeObject(result.errors)
            );
        }

        [Fact]
        public async Task ThrowsErrorWhenWrongTeacherIdIsGiven()
        {
            _dbService
                .Setup(s => s.Get<Lesson>(It.IsAny<string>(), It.IsAny<object>()))
                .ReturnsAsync(MockData.GetLessonDetails());
            _dbService
                .Setup(s => s.Get<LessonType>(It.IsAny<string>(), It.IsAny<object>()))
                .ReturnsAsync(MockData.GetLessonTypeDetails());
            _dbService
                .Setup(s => s.Get<Classroom>(It.IsAny<string>(), It.IsAny<object>()))
                .ReturnsAsync(MockData.GetClassroomDetails());

            var createLessonBlockCommand = new CreateLessonBlockCommand(
                new Guid(),
                new Guid(),
                MockData.GetCreateLessonBlockDTODetails(new Guid(), new Guid(), new Guid()),
                new Guid(),
                "User"
            );
            var createLessonBlockHandler = new CreateLessonBlockHandler(
                _dbService.Object,
                _str.Object
            );
            var result = await Assert.ThrowsAsync<ApiErrorException>(
                async () =>
                    await createLessonBlockHandler.Handle(
                        createLessonBlockCommand,
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

            var createLessonBlockCommand = new CreateLessonBlockCommand(
                new Guid(),
                new Guid(),
                MockData.GetCreateLessonBlockDTODetails(new Guid(), new Guid(), new Guid()),
                new Guid(),
                "User"
            );
            var createLessonBlockHandler = new CreateLessonBlockHandler(
                _dbService.Object,
                _str.Object
            );
            var result = await Assert.ThrowsAsync<ApiErrorException>(
                async () =>
                    await createLessonBlockHandler.Handle(
                        createLessonBlockCommand,
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
                .Setup(s => s.Get<Lesson>(It.IsAny<string>(), It.IsAny<object>()))
                .ReturnsAsync(MockData.GetLessonDetails());
            _dbService
                .Setup(s => s.Get<LessonType>(It.IsAny<string>(), It.IsAny<object>()))
                .ReturnsAsync(MockData.GetLessonTypeDetails());
            _dbService
                .Setup(s => s.Get<Teacher>(It.IsAny<string>(), It.IsAny<object>()))
                .ReturnsAsync(MockData.GetTeacherDetails());

            var createLessonBlockCommand = new CreateLessonBlockCommand(
                new Guid(),
                new Guid(),
                MockData.GetCreateLessonBlockDTODetails(new Guid(), new Guid(), new Guid()),
                new Guid(),
                "User"
            );
            var createLessonBlockHandler = new CreateLessonBlockHandler(
                _dbService.Object,
                _str.Object
            );
            var result = await Assert.ThrowsAsync<ApiErrorException>(
                async () =>
                    await createLessonBlockHandler.Handle(
                        createLessonBlockCommand,
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

            var createLessonBlockCommand = new CreateLessonBlockCommand(
                new Guid(),
                new Guid(),
                MockData.GetCreateLessonBlockDTODetails(new Guid(), new Guid(), new Guid()),
                new Guid(),
                "User"
            );
            var createLessonBlockHandler = new CreateLessonBlockHandler(
                _dbService.Object,
                _str.Object
            );
            var result = await Assert.ThrowsAsync<ApiErrorException>(
                async () =>
                    await createLessonBlockHandler.Handle(
                        createLessonBlockCommand,
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

            var createLessonRequest = MockData.GetCreateLessonBlockDTODetails(
                new Guid(),
                new Guid(),
                new Guid()
            );
            createLessonRequest.LessonEnd = 3;
            createLessonRequest.LessonStart = 4;
            var createLessonBlockCommand = new CreateLessonBlockCommand(
                new Guid(),
                new Guid(),
                createLessonRequest,
                new Guid(),
                "User"
            );
            var createLessonBlockHandler = new CreateLessonBlockHandler(
                _dbService.Object,
                _str.Object
            );
            var result = await Assert.ThrowsAsync<ApiErrorException>(
                async () =>
                    await createLessonBlockHandler.Handle(
                        createLessonBlockCommand,
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

            var createLessonRequest = MockData.GetCreateLessonBlockDTODetails(
                new Guid(),
                new Guid(),
                new Guid()
            );
            createLessonRequest.LessonStart = 0;
            var createLessonBlockCommand = new CreateLessonBlockCommand(
                new Guid(),
                new Guid(),
                createLessonRequest,
                new Guid(),
                "User"
            );
            var createLessonBlockHandler = new CreateLessonBlockHandler(
                _dbService.Object,
                _str.Object
            );
            var result = await Assert.ThrowsAsync<ApiErrorException>(
                async () =>
                    await createLessonBlockHandler.Handle(
                        createLessonBlockCommand,
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

            var createLessonRequest = MockData.GetCreateLessonBlockDTODetails(
                new Guid(),
                new Guid(),
                new Guid()
            );
            createLessonRequest.LessonEnd = 6;
            var createLessonBlockCommand = new CreateLessonBlockCommand(
                new Guid(),
                new Guid(),
                createLessonRequest,
                new Guid(),
                "User"
            );
            var createLessonBlockHandler = new CreateLessonBlockHandler(
                _dbService.Object,
                _str.Object
            );
            var result = await Assert.ThrowsAsync<ApiErrorException>(
                async () =>
                    await createLessonBlockHandler.Handle(
                        createLessonBlockCommand,
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
        public async Task ThrowsErrorWhenLessonDateIsBeforeSchoolYearStart()
        {
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

            var createLessonRequest = MockData.GetCreateLessonBlockDTODetails(
                new Guid(),
                new Guid(),
                new Guid()
            );
            createLessonRequest.LessonDate = new DateOnly(1950, 1, 2);
            var createLessonBlockCommand = new CreateLessonBlockCommand(
                new Guid(),
                new Guid(),
                createLessonRequest,
                new Guid(),
                "User"
            );
            var createLessonBlockHandler = new CreateLessonBlockHandler(
                _dbService.Object,
                _str.Object
            );
            var result = await Assert.ThrowsAsync<ApiErrorException>(
                async () =>
                    await createLessonBlockHandler.Handle(
                        createLessonBlockCommand,
                        new CancellationToken()
                    )
            );

            Assert.Contains("Date must be in between", JsonConvert.SerializeObject(result.errors));
        }

        [Fact]
        public async Task ThrowsErrorWhenLessonDateIsAfterSchoolYearEnd()
        {
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

            var createLessonRequest = MockData.GetCreateLessonBlockDTODetails(
                new Guid(),
                new Guid(),
                new Guid()
            );
            createLessonRequest.LessonDate = new DateOnly(2050, 1, 3);
            var createLessonBlockCommand = new CreateLessonBlockCommand(
                new Guid(),
                new Guid(),
                createLessonRequest,
                new Guid(),
                "User"
            );
            var createLessonBlockHandler = new CreateLessonBlockHandler(
                _dbService.Object,
                _str.Object
            );
            var result = await Assert.ThrowsAsync<ApiErrorException>(
                async () =>
                    await createLessonBlockHandler.Handle(
                        createLessonBlockCommand,
                        new CancellationToken()
                    )
            );

            Assert.Contains("Date must be in between", JsonConvert.SerializeObject(result.errors));
        }

        [Fact]
        public async Task ThrowsErrorWhenLessonOccursOnADayOfTheWeekThatIsNotAllowedByScheduleSettings()
        {
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

            var createLessonRequest = MockData.GetCreateLessonBlockDTODetails(
                new Guid(),
                new Guid(),
                new Guid()
            );
            createLessonRequest.LessonDate = new DateOnly(2024, 12, 15);
            var createLessonBlockCommand = new CreateLessonBlockCommand(
                new Guid(),
                new Guid(),
                createLessonRequest,
                new Guid(),
                "User"
            );
            var createLessonBlockHandler = new CreateLessonBlockHandler(
                _dbService.Object,
                _str.Object
            );
            var result = await Assert.ThrowsAsync<ApiErrorException>(
                async () =>
                    await createLessonBlockHandler.Handle(
                        createLessonBlockCommand,
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
        public async Task ThrowsErrorWhenWeekIntervalIsLessThan1()
        {
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

            var createLessonRequest = MockData.GetCreateLessonBlockDTODetails(
                new Guid(),
                new Guid(),
                new Guid()
            );
            createLessonRequest.WeekInterval = 0;
            var createLessonBlockCommand = new CreateLessonBlockCommand(
                new Guid(),
                new Guid(),
                createLessonRequest,
                new Guid(),
                "User"
            );
            var createLessonBlockHandler = new CreateLessonBlockHandler(
                _dbService.Object,
                _str.Object
            );
            var result = await Assert.ThrowsAsync<ApiErrorException>(
                async () =>
                    await createLessonBlockHandler.Handle(
                        createLessonBlockCommand,
                        new CancellationToken()
                    )
            );

            Assert.Equal(
                JsonConvert.SerializeObject(
                    new ErrorObject[] { new ErrorObject("WeekInterval parameter is invalid") }
                ),
                JsonConvert.SerializeObject(result.errors)
            );
        }
    }
}
