using AlpimiAPI.Entities.EAuth;
using AlpimiAPI.Entities.EAuth.DTO;
using AlpimiAPI.Entities.EClassroom;
using AlpimiAPI.Entities.EClassroom.DTO;
using AlpimiAPI.Entities.EClassroomType;
using AlpimiAPI.Entities.EClassroomType.DTO;
using AlpimiAPI.Entities.EDayOff;
using AlpimiAPI.Entities.EDayOff.DTO;
using AlpimiAPI.Entities.EGroup;
using AlpimiAPI.Entities.EGroup.DTO;
using AlpimiAPI.Entities.ELesson;
using AlpimiAPI.Entities.ELesson.DTO;
using AlpimiAPI.Entities.ELessonPeriod.DTO;
using AlpimiAPI.Entities.ELessonPerioid;
using AlpimiAPI.Entities.ELessonType;
using AlpimiAPI.Entities.ELessonType.DTO;
using AlpimiAPI.Entities.ESchedule;
using AlpimiAPI.Entities.ESchedule.DTO;
using AlpimiAPI.Entities.EScheduleSettings;
using AlpimiAPI.Entities.EScheduleSettings.DTO;
using AlpimiAPI.Entities.EStudent;
using AlpimiAPI.Entities.EStudent.DTO;
using AlpimiAPI.Entities.ESubgroup;
using AlpimiAPI.Entities.ESubgroup.DTO;
using AlpimiAPI.Entities.ETeacher;
using AlpimiAPI.Entities.ETeacher.DTO;
using AlpimiAPI.Entities.EUser;
using AlpimiAPI.Entities.EUser.DTO;

namespace AlpimiTest.TestUtilities
{
    public static class MockData
    {
        public static User GetUserDetails()
        {
            return new User()
            {
                Id = new Guid(),
                Login = "Marek",
                CustomURL = "44f"
            };
        }

        public static Schedule GetScheduleDetails()
        {
            return new Schedule()
            {
                Id = new Guid(),
                Name = "Plan_Marka",
                UserId = new Guid(),
                User = GetUserDetails()
            };
        }

        public static Auth GetAuthDetails()
        {
            return new Auth()
            {
                Password = "sssSSS1!",
                Id = new Guid(),
                Salt = "cSnOW17u464QVvXSjMr0wQ==",
                Role = "User",
                UserId = new Guid(),
                User = GetUserDetails()
            };
        }

        public static ScheduleSettings GetScheduleSettingsDetails()
        {
            return new ScheduleSettings()
            {
                Id = new Guid(),
                SchoolHour = 10,
                SchoolYearStart = new DateOnly(2020, 11, 19),
                SchoolYearEnd = new DateOnly(2025, 11, 19),
                ScheduleId = new Guid(),
                Schedule = GetScheduleDetails()
            };
        }

        public static DayOff GetDayOffDetails()
        {
            return new DayOff()
            {
                Id = new Guid(),
                Name = "Marek_Fest",
                From = new DateOnly(2022, 12, 12),
                To = new DateOnly(2023, 12, 12),
                ScheduleSettingsId = new Guid(),
                ScheduleSettings = null!
            };
        }

        public static LessonPeriod GetLessonPeriodDetails()
        {
            return new LessonPeriod()
            {
                Id = new Guid(),
                Start = new TimeOnly(10, 00, 00),
                Finish = new TimeOnly(11, 00, 00),
                ScheduleSettingsId = new Guid(),
                ScheduleSettings = GetScheduleSettingsDetails()
            };
        }

        public static Teacher GetTeacherDetails()
        {
            return new Teacher()
            {
                Name = "Jac",
                Surname = "Pie",
                ScheduleId = new Guid(),
                Schedule = GetScheduleDetails()
            };
        }

        public static Group GetGroupDetails()
        {
            return new Group()
            {
                Name = "13K2",
                StudentCount = 32,
                ScheduleId = new Guid(),
                Schedule = GetScheduleDetails()
            };
        }

        public static Subgroup GetSubgroupDetails()
        {
            return new Subgroup()
            {
                Name = "K03",
                StudentCount = 16,
                GroupId = new Guid(),
                Group = GetGroupDetails()
            };
        }

        public static Student GetStudentDetails()
        {
            return new Student()
            {
                AlbumNumber = "88776655",
                GroupId = new Guid(),
                Group = GetGroupDetails()
            };
        }

        public static ClassroomType GetClassroomTypeDetails()
        {
            return new ClassroomType()
            {
                Name = "Komputerowa",
                ScheduleId = new Guid(),
                Schedule = GetScheduleDetails()
            };
        }

        public static Classroom GetClassroomDetails()
        {
            return new Classroom()
            {
                Name = "G120",
                Capacity = 100,
                ScheduleId = new Guid(),
                Schedule = GetScheduleDetails()
            };
        }

        public static LessonType GetLessonTypeDetails()
        {
            return new LessonType()
            {
                Name = "Komputerowa",
                Color = 10,
                ScheduleId = new Guid(),
                Schedule = GetScheduleDetails()
            };
        }

        public static Lesson GetLessonDetails()
        {
            return new Lesson()
            {
                Name = "Niski poziom",
                AmountOfHours = 10,
                SubgroupId = new Guid(),
                LessonTypeId = new Guid(),
                LessonType = GetLessonTypeDetails()
            };
        }

        public static CreateUserDTO GetCreateUserDTODetails()
        {
            var user = GetUserDetails();
            return new CreateUserDTO()
            {
                Login = user.Login,
                CustomURL = user.CustomURL!,
                Password = "sssSSS1!"
            };
        }

        public static UpdateUserDTO GetUpdateUserDTODetails()
        {
            return new UpdateUserDTO() { Login = "Updated_Marek", CustomURL = "Updated_URL" };
        }

        public static CreateScheduleDTO GetCreateScheduleDTODetails()
        {
            var scheduleSettings = GetScheduleSettingsDetails();
            return new CreateScheduleDTO()
            {
                Name = scheduleSettings.Schedule.Name,
                SchoolHour = scheduleSettings.SchoolHour,
                SchoolYearStart = scheduleSettings.SchoolYearStart,
                SchoolYearEnd = scheduleSettings.SchoolYearEnd
            };
        }

        public static UpdateScheduleSettingsDTO GetUpdateScheduleSettingsDTO()
        {
            return new UpdateScheduleSettingsDTO()
            {
                SchoolHour = 29,
                SchoolYearStart = new DateOnly(2020, 10, 1),
                SchoolYearEnd = new DateOnly(2022, 1, 10)
            };
        }

        public static UpdateScheduleDTO GetUpdateScheduleDTODetails()
        {
            return new UpdateScheduleDTO() { Name = "Updated_plan" };
        }

        public static CreateScheduleDTO GetCreateSecondScheduleDTODetails()
        {
            return new CreateScheduleDTO()
            {
                Name = "Second_Name",
                SchoolHour = 431,
                SchoolYearStart = new DateOnly(2022, 11, 19),
                SchoolYearEnd = new DateOnly(2025, 11, 19),
            };
        }

        public static CreateUserDTO GetCreateSecondUserDTODetails()
        {
            return new CreateUserDTO()
            {
                Login = "Second_User",
                CustomURL = "Second_URL",
                Password = "sssSSS1!"
            };
        }

        public static LoginDTO GetLoginDTODetails()
        {
            var auth = GetAuthDetails();
            return new LoginDTO() { Login = auth.User.Login, Password = auth.Password };
        }

        public static CreateDayOffDTO GetCreateDayOffDTODetails(Guid scheduleId)
        {
            var dayOff = GetDayOffDetails();
            return new CreateDayOffDTO()
            {
                Name = dayOff.Name,
                From = dayOff.From,
                To = dayOff.To,
                ScheduleId = scheduleId
            };
        }

        public static CreateDayOffDTO GetCreateSecondDayOffDTODetails(Guid scheduleId)
        {
            return new CreateDayOffDTO()
            {
                Name = "second_name",
                From = new DateOnly(2021, 8, 8),
                ScheduleId = scheduleId
            };
        }

        public static UpdateDayOffDTO GetUpdateDayOffDTODetails()
        {
            return new UpdateDayOffDTO()
            {
                From = new DateOnly(2023, 11, 22),
                To = new DateOnly(2023, 11, 23),
                Name = "Inny_fest"
            };
        }

        public static CreateLessonPeriodDTO GetCreateLessonPeriodDTODetails(Guid scheduleId)
        {
            var lessonPeriod = GetLessonPeriodDetails();
            return new CreateLessonPeriodDTO()
            {
                Start = lessonPeriod.Start,
                Finish = lessonPeriod.Finish,
                ScheduleId = scheduleId
            };
        }

        public static CreateLessonPeriodDTO GetCreateSecondLessonPeriodDTODetails(Guid scheduleId)
        {
            return new CreateLessonPeriodDTO()
            {
                Start = new TimeOnly(11, 00, 00),
                Finish = new TimeOnly(12, 00, 00),
                ScheduleId = scheduleId
            };
        }

        public static UpdateLessonPeriodDTO GetUpdateLessonPeriodDTODetails()
        {
            return new UpdateLessonPeriodDTO()
            {
                Start = new TimeOnly(8, 00, 00),
                Finish = new TimeOnly(9, 00, 00),
            };
        }

        public static CreateTeacherDTO GetCreateTeacherDTODetails(Guid scheduleId)
        {
            var teacher = GetTeacherDetails();
            return new CreateTeacherDTO()
            {
                Name = teacher.Name,
                Surname = teacher.Surname,
                ScheduleId = scheduleId
            };
        }

        public static CreateTeacherDTO GetCreateSecondTeacherDTODetails(Guid scheduleId)
        {
            return new CreateTeacherDTO()
            {
                Name = "Mariusz",
                Surname = "Mazeowski",
                ScheduleId = scheduleId
            };
        }

        public static UpdateTeacherDTO GetUpdateTeacherDTODetails()
        {
            return new UpdateTeacherDTO() { Name = "Pan", Surname = "Jan" };
        }

        public static CreateGroupDTO GetCreateGroupDTODetails(Guid scheduleId)
        {
            var group = GetGroupDetails();
            return new CreateGroupDTO()
            {
                Name = group.Name,
                StudentCount = group.StudentCount,
                ScheduleId = scheduleId
            };
        }

        public static CreateGroupDTO GetCreateSecondGroupDTODetails(Guid scheduleId)
        {
            return new CreateGroupDTO()
            {
                Name = "11C2",
                StudentCount = 1,
                ScheduleId = scheduleId
            };
        }

        public static UpdateGroupDTO GetUpdateGroupDTODetails()
        {
            return new UpdateGroupDTO() { Name = "24L4", StudentCount = 8 };
        }

        public static CreateSubgroupDTO GetCreateSubgroupDTODetails(Guid groupId)
        {
            var subgroup = GetSubgroupDetails();
            return new CreateSubgroupDTO()
            {
                Name = subgroup.Name,
                StudentCount = subgroup.StudentCount,
                GroupId = groupId
            };
        }

        public static CreateSubgroupDTO GetCreateSecondSubgroupDTODetails(Guid groupId)
        {
            return new CreateSubgroupDTO()
            {
                Name = "K04",
                StudentCount = 3,
                GroupId = groupId
            };
        }

        public static UpdateSubgroupDTO GetUpdateSubgroupDTODetails()
        {
            return new UpdateSubgroupDTO() { Name = "L02", StudentCount = 8 };
        }

        public static CreateStudentDTO GetCreateStudentDTODetails(Guid groupId)
        {
            var student = GetStudentDetails();
            return new CreateStudentDTO() { AlbumNumber = student.AlbumNumber, GroupId = groupId };
        }

        public static CreateStudentDTO GetCreateSecondStudentDTODetails(Guid groupId)
        {
            return new CreateStudentDTO() { AlbumNumber = "11223344", GroupId = groupId };
        }

        public static UpdateStudentDTO GetUpdateStudentDTODetails()
        {
            return new UpdateStudentDTO() { AlbumNumber = "newNumber" };
        }

        public static CreateClassroomTypeDTO GetCreateClassroomTypeDTODetails(Guid scheduleId)
        {
            var classroomType = GetClassroomTypeDetails();
            return new CreateClassroomTypeDTO()
            {
                Name = classroomType.Name,
                ScheduleId = scheduleId
            };
        }

        public static CreateClassroomTypeDTO GetCreateSecondClassroomTypeDTODetails(Guid scheduleId)
        {
            return new CreateClassroomTypeDTO() { Name = "Wykładowa", ScheduleId = scheduleId };
        }

        public static UpdateClassroomTypeDTO GetUpdateClassroomTypeDTODetails()
        {
            return new UpdateClassroomTypeDTO() { Name = "Hartowanie" };
        }

        public static CreateClassroomDTO GetCreateClassroomDTODetails(Guid scheduleId)
        {
            var classroom = GetClassroomDetails();
            return new CreateClassroomDTO()
            {
                Name = classroom.Name,
                Capacity = classroom.Capacity,
                ScheduleId = scheduleId
            };
        }

        public static CreateClassroomDTO GetCreateSecondClassroomDTODetails(Guid scheduleId)
        {
            return new CreateClassroomDTO()
            {
                Name = "G119",
                Capacity = 10,
                ScheduleId = scheduleId
            };
        }

        public static UpdateClassroomDTO GetUpdateClassroomDTODetails()
        {
            return new UpdateClassroomDTO() { Name = "E5", Capacity = 25 };
        }

        public static CreateLessonTypeDTO GetCreateLessonTypeDTODetails(Guid scheduleId)
        {
            var lessonType = GetLessonTypeDetails();
            return new CreateLessonTypeDTO()
            {
                Name = lessonType.Name,
                Color = lessonType.Color,
                ScheduleId = scheduleId
            };
        }

        public static CreateLessonTypeDTO GetCreateSecondLessonTypeDTODetails(Guid scheduleId)
        {
            return new CreateLessonTypeDTO()
            {
                Name = "G119",
                Color = 10,
                ScheduleId = scheduleId
            };
        }

        public static UpdateLessonTypeDTO GetUpdateLessonTypeDTODetails()
        {
            return new UpdateLessonTypeDTO() { Name = "E5", Color = 2 };
        }

        public static CreateLessonDTO GetCreateLessonDTODetails(Guid subgroupId, Guid lessonTypeId)
        {
            var lesson = GetLessonDetails();
            return new CreateLessonDTO()
            {
                Name = lesson.Name,
                AmountOfHours = lesson.AmountOfHours,
                LessonTypeId = lessonTypeId,
                SubgroupId = subgroupId
            };
        }

        public static CreateLessonDTO GetCreateSecondLessonDTODetails(
            Guid subgroupId,
            Guid lessonTypeId
        )
        {
            return new CreateLessonDTO()
            {
                Name = "podstawy testowania",
                AmountOfHours = 2,
                LessonTypeId = lessonTypeId,
                SubgroupId = subgroupId
            };
        }

        public static UpdateLessonDTO GetUpdateLessonDTODetails()
        {
            return new UpdateLessonDTO() { Name = "Bazie Danych", AmountOfHours = 9 };
        }
    }
}
