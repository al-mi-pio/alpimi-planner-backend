using AlpimiAPI.Entities.EAuth;
using AlpimiAPI.Entities.ESchedule;
using AlpimiAPI.Entities.EUser;

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
                Password = "RPhZLnao+2lWH4JvwGZRLI/14QI=",
                Id = new Guid(),
                Salt = "zr+8L0dX4IBdGUgvHDM1Zw==",
                Role = "Admin",
                UserId = new Guid(),
                User = GetUserDetails(),
            };

            return auth;
        }
    }
}
