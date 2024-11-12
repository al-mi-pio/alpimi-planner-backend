using AlpimiAPI.Entities.EAuth;
using AlpimiAPI.Entities.EAuth.DTO;
using AlpimiAPI.Entities.ESchedule;
using AlpimiAPI.Entities.ESchedule.DTO;
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
                SchoolHour = 60,
                UserId = new Guid(),
                User = GetUserDetails()
            };

            return schedule;
        }

        public static IEnumerable<Schedule> GetSchedulesDetails()
        {
            var schedule = new Schedule()
            {
                Id = new Guid(),
                Name = "Plan_Marka",
                SchoolHour = 60,
                UserId = new Guid(),
                User = GetUserDetails()
            };

            return [schedule, schedule];
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
                User = GetUserDetails(),
            };

            return auth;
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
            var schedule = GetScheduleDetails();
            return new CreateScheduleDTO()
            {
                Name = schedule.Name,
                SchoolHour = schedule.SchoolHour
            };
        }

        public static UpdateScheduleDTO GetUpdateScheduleDTODetails()
        {
            return new UpdateScheduleDTO() { Name = "Updated_plan", SchoolHour = 409 };
        }

        public static CreateScheduleDTO GetCreateSecondScheduleDTODetails()
        {
            return new CreateScheduleDTO() { Name = "Second_Name", SchoolHour = 431 };
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
            var auth = MockData.GetAuthDetails();
            return new LoginDTO() { Login = auth.User.Login, Password = auth.Password };
        }
    }
}
