using AlpimiAPI.Entities.EAuth;
using AlpimiAPI.Entities.EAuth.DTO;
using AlpimiAPI.Entities.EDayOff;
using AlpimiAPI.Entities.EDayOff.DTO;
using AlpimiAPI.Entities.ESchedule;
using AlpimiAPI.Entities.ESchedule.DTO;
using AlpimiAPI.Entities.EScheduleSettings;
using AlpimiAPI.Entities.EScheduleSettings.DTO;
using AlpimiAPI.Entities.EUser;
using AlpimiAPI.Entities.EUser.DTO;

namespace AlpimiTest.TestUtilities
{
    public static class MockData
    {
        public static User GetUserDetails()
        {
            var user = new User()
            {
                Id = new Guid(),
                Login = "Marek",
                CustomURL = "44f"
            };

            return user;
        }

        public static Schedule GetScheduleDetails()
        {
            var schedule = new Schedule()
            {
                Id = new Guid(),
                Name = "Plan_Marka",
                UserId = new Guid(),
                User = GetUserDetails()
            };

            return schedule;
        }

        public static Auth GetAuthDetails()
        {
            var auth = new Auth()
            {
                Password = "sssSSS1!",
                Id = new Guid(),
                Salt = "cSnOW17u464QVvXSjMr0wQ==",
                Role = "User",
                UserId = new Guid(),
                User = GetUserDetails()
            };

            return auth;
        }

        public static ScheduleSettings GetScheduleSettingsDetails()
        {
            var scheduleSettings = new ScheduleSettings()
            {
                Id = new Guid(),
                SchoolHour = 10,
                SchoolYearStart = new DateTime(2020, 11, 19),
                SchoolYearEnd = new DateTime(2025, 11, 19),
                ScheduleId = new Guid(),
                Schedule = GetScheduleDetails()
            };

            return scheduleSettings;
        }

        public static DayOff GetDayOffDetails()
        {
            var dayOff = new DayOff()
            {
                Id = new Guid(),
                Name = "Marek_Fest",
                From = new DateTime(2022, 12, 12),
                To = new DateTime(2025, 12, 12),
                ScheduleSettingsId = new Guid(),
                ScheduleSettings = null!
            };
            return dayOff;
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
                SchoolYearStart = new DateTime(2020, 10, 1),
                SchoolYearEnd = new DateTime(2022, 1, 10)
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
                SchoolYearStart = new DateTime(2022, 11, 19),
                SchoolYearEnd = new DateTime(2025, 11, 19),
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
                Date = dayOff.From,
                NumberOfDays = 2,
                ScheduleId = scheduleId
            };
        }

        public static CreateDayOffDTO GetCreateSecondDayOffDTODetails(Guid scheduleId)
        {
            var dayOff = GetDayOffDetails();
            return new CreateDayOffDTO()
            {
                Name = "second_name",
                Date = new DateTime(2021, 8, 8),
                ScheduleId = scheduleId
            };
        }

        public static UpdateDayOffDTO GetUpdateDayOffDTODetails()
        {
            return new UpdateDayOffDTO()
            {
                From = new DateTime(2023, 11, 22),
                To = new DateTime(2023, 11, 23),
                Name = "Inny_fest"
            };
        }
    }
}
