﻿using AlpimiAPI.Entities.EAuth;
using AlpimiAPI.Entities.EDayOff;
using AlpimiAPI.Entities.ESchedule;
using AlpimiAPI.Entities.EScheduleSettings;
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
        #endregion
        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
            optionsBuilder.UseSqlServer(Configuration.GetConnectionString());
        }
    }
}
