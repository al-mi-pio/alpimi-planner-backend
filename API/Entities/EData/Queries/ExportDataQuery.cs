using System.Xml.Linq;
using AlpimiAPI.Database;
using AlpimiAPI.Entities.EAvailability;
using AlpimiAPI.Entities.EAvailability.Queries;
using AlpimiAPI.Entities.EClassroom.Queries;
using AlpimiAPI.Entities.EClassroomType.Queries;
using AlpimiAPI.Entities.EData.DTO;
using AlpimiAPI.Entities.EDayOff.Queries;
using AlpimiAPI.Entities.EGroup.Queries;
using AlpimiAPI.Entities.ELesson.Queries;
using AlpimiAPI.Entities.ELessonPeriod.Queries;
using AlpimiAPI.Entities.ELessonType.Queries;
using AlpimiAPI.Entities.ESchedule;
using AlpimiAPI.Entities.ESchedule.Queries;
using AlpimiAPI.Entities.EStudent.Queries;
using AlpimiAPI.Entities.ESubgroup.Queries;
using AlpimiAPI.Entities.ETeacher.Queries;
using AlpimiAPI.Locales;
using AlpimiAPI.Responses;
using AlpimiAPI.Utilities;
using MediatR;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Localization;
using Moq;

namespace AlpimiAPI.Entities.EData
{
    public record ExportDataQuery(Guid ScheduleId, Guid FilteredId, string Role)
        : IRequest<ExportDataDTO>;

    public class ExportDataHandler : IRequestHandler<ExportDataQuery, ExportDataDTO>
    {
        private readonly IDbService _dbService;
        private readonly IStringLocalizer<Errors> _str;
        private readonly IStringLocalizer<Fields> _strFields;
        private readonly IStringLocalizer<Data> _strData;

        public ExportDataHandler(
            IDbService dbService,
            IStringLocalizer<Errors> str,
            IStringLocalizer<Fields> strFields,
            IStringLocalizer<Data> strData
        )
        {
            _dbService = dbService;
            _str = str;
            _strFields = strFields;
            _strData = strData;
        }

        public async Task<ExportDataDTO> Handle(
            ExportDataQuery request,
            CancellationToken cancellationToken
        )
        {
            GetScheduleHandler getScheduleHandler = new GetScheduleHandler(_dbService);
            GetScheduleQuery getScheduleQuery = new GetScheduleQuery(
                request.ScheduleId,
                request.FilteredId,
                request.Role
            );
            ActionResult<Schedule?> schedule = await getScheduleHandler.Handle(
                getScheduleQuery,
                cancellationToken
            );

            if (schedule.Value == null)
            {
                throw new ApiErrorException(
                    [
                        new ErrorObject(
                            _str["resourceNotFound", _strFields["Schedule"], request.ScheduleId]
                        )
                    ]
                );
            }

            var pagination = new PaginationParams(int.MaxValue, 0, "Id", "ASC");

            var getAllSubgroupsQuery = new GetAllSubgroupsQuery(
                request.ScheduleId,
                new Guid(),
                "Admin",
                pagination
            );
            var getAllSubgroupsHandler = new GetAllSubgroupsHandler(
                _dbService,
                new Mock<IStringLocalizer<Errors>>().Object,
                new Mock<IStringLocalizer<Fields>>().Object
            );
            var subgroups = await getAllSubgroupsHandler.Handle(
                getAllSubgroupsQuery,
                CancellationToken.None
            );

            var getAllGroupsQuery = new GetAllGroupsByScheduleQuery(
                request.ScheduleId,
                new Guid(),
                "Admin",
                pagination
            );
            var getAllGroupsHandler = new GetAllGroupsByScheduleHandler(
                _dbService,
                new Mock<IStringLocalizer<Errors>>().Object,
                new Mock<IStringLocalizer<Fields>>().Object
            );
            var groups = await getAllGroupsHandler.Handle(
                getAllGroupsQuery,
                CancellationToken.None
            );

            var getAllLessonsQuery = new GetAllLessonsQuery(
                request.ScheduleId,
                new Guid(),
                "Admin",
                pagination
            );
            var getAllLessonsHandler = new GetAllLessonsHandler(
                _dbService,
                new Mock<IStringLocalizer<Errors>>().Object,
                new Mock<IStringLocalizer<Fields>>().Object
            );
            var lessons = await getAllLessonsHandler.Handle(
                getAllLessonsQuery,
                CancellationToken.None
            );

            var getAllTeachersQuery = new GetAllTeachersByScheduleQuery(
                request.ScheduleId,
                new Guid(),
                "Admin",
                pagination
            );
            var getAllTeachersHandler = new GetAllTeachersByScheduleHandler(
                _dbService,
                new Mock<IStringLocalizer<Errors>>().Object,
                new Mock<IStringLocalizer<Fields>>().Object
            );
            var teachers = await getAllTeachersHandler.Handle(
                getAllTeachersQuery,
                CancellationToken.None
            );

            var getAllStudentsQuery = new GetAllStudentsQuery(
                request.ScheduleId,
                new Guid(),
                "Admin",
                pagination
            );
            var getAllStudentsHandler = new GetAllStudentsHandler(
                _dbService,
                new Mock<IStringLocalizer<Errors>>().Object,
                new Mock<IStringLocalizer<Fields>>().Object
            );
            var students = await getAllStudentsHandler.Handle(
                getAllStudentsQuery,
                CancellationToken.None
            );

            var getAllClassroomsQuery = new GetAllClassroomsQuery(
                request.ScheduleId,
                new Guid(),
                "Admin",
                pagination
            );
            var getAllClassroomsHandler = new GetAllClassroomsHandler(
                _dbService,
                new Mock<IStringLocalizer<Errors>>().Object,
                new Mock<IStringLocalizer<Fields>>().Object
            );
            var classrooms = await getAllClassroomsHandler.Handle(
                getAllClassroomsQuery,
                CancellationToken.None
            );

            var getAllDaysOffQuery = new GetAllDayOffByScheduleQuery(
                request.ScheduleId,
                new Guid(),
                "Admin",
                pagination
            );
            var getAllDaysOffHandler = new GetAllDayOffByScheduleHandler(
                _dbService,
                new Mock<IStringLocalizer<Errors>>().Object,
                new Mock<IStringLocalizer<Fields>>().Object
            );
            var daysOff = await getAllDaysOffHandler.Handle(
                getAllDaysOffQuery,
                CancellationToken.None
            );

            var getAllLessonPeriodsQuery = new GetAllLessonPeriodByScheduleQuery(
                request.ScheduleId,
                new Guid(),
                "Admin",
                new PaginationParams(int.MaxValue, 0, "Start", "ASC")
            );
            var getAllLessonPeriodsHandler = new GetAllLessonPeriodByScheduleHandler(
                _dbService,
                new Mock<IStringLocalizer<Errors>>().Object,
                new Mock<IStringLocalizer<Fields>>().Object
            );
            var lessonPeriods = await getAllLessonPeriodsHandler.Handle(
                getAllLessonPeriodsQuery,
                CancellationToken.None
            );

            var getAllLessonTypesQuery = new GetAllLessonTypesQuery(
                request.ScheduleId,
                new Guid(),
                "Admin",
                pagination
            );
            var getAllLessonTypesHandler = new GetAllLessonTypesHandler(
                _dbService,
                new Mock<IStringLocalizer<Errors>>().Object,
                new Mock<IStringLocalizer<Fields>>().Object
            );
            var lessonTypes = await getAllLessonTypesHandler.Handle(
                getAllLessonTypesQuery,
                CancellationToken.None
            );

            var getAllClassroomTypesQuery = new GetAllClassroomTypesQuery(
                request.ScheduleId,
                new Guid(),
                "Admin",
                pagination
            );
            var getAllClassroomTypesHandler = new GetAllClassroomTypesHandler(
                _dbService,
                new Mock<IStringLocalizer<Errors>>().Object,
                new Mock<IStringLocalizer<Fields>>().Object
            );
            var classroomTypes = await getAllClassroomTypesHandler.Handle(
                getAllClassroomTypesQuery,
                CancellationToken.None
            );

            IEnumerable<Availability> availability = [];
            foreach (var teacher in teachers.Item1!)
            {
                var getAllAvailabilityQuery = new GetAllAvailabilityByTeacherQuery(
                    teacher.Id,
                    new Guid(),
                    "Admin",
                    pagination
                );
                var getAllAvailabilityHandler = new GetAllAvailabilityByTeacherHandler(
                    _dbService,
                    new Mock<IStringLocalizer<Errors>>().Object,
                    new Mock<IStringLocalizer<Fields>>().Object
                );
                var teacherAvailability = await getAllAvailabilityHandler.Handle(
                    getAllAvailabilityQuery,
                    CancellationToken.None
                );
                availability = availability.Concat(teacherAvailability.Item1!);
            }

            string XML = await CreateXMLFile.create(
                _dbService,
                _strData,
                daysOff.Item1!,
                lessonPeriods.Item1!,
                classroomTypes.Item1!,
                lessons.Item1!,
                groups.Item1!,
                subgroups.Item1!,
                availability,
                teachers.Item1!,
                lessonTypes.Item1!,
                students.Item1!,
                classrooms.Item1!
            );
            return new ExportDataDTO() { Payload = XML };
        }
    }
}
