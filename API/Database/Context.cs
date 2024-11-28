using AlpimiAPI.Entities.EAuth;
using AlpimiAPI.Entities.EDayOff;
using AlpimiAPI.Entities.EGroup;
using AlpimiAPI.Entities.ELessonPerioid;
using AlpimiAPI.Entities.ESchedule;
using AlpimiAPI.Entities.EScheduleSettings;
using AlpimiAPI.Entities.EStudent;
using AlpimiAPI.Entities.ESubgroup;
using AlpimiAPI.Entities.ETeacher;
using AlpimiAPI.Entities.EUser;
using AlpimiAPI.Utilities;
using Microsoft.EntityFrameworkCore;

namespace AlpimiAPI.Database
{
    public class Context : DbContext
    {
        #region Entities


        public DbSet<User> User { get; set; }
        public DbSet<Auth> Auth { get; set; }
        public DbSet<Schedule> Schedule { get; set; }
        public DbSet<ScheduleSettings> ScheduleSettings { get; set; }
        public DbSet<DayOff> DayOff { get; set; }
        public DbSet<LessonPeriod> LessonPeriod { get; set; }
        public DbSet<Teacher> Teacher { get; set; }
        public DbSet<Group> Group { get; set; }
        public DbSet<Subgroup> Subgroup { get; set; }
        public DbSet<Student> Student { get; set; }
        #endregion
        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
            optionsBuilder.UseSqlServer(Configuration.GetConnectionString());
        }
    }
}
