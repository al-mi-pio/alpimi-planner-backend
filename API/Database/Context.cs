using AlpimiAPI.Entities.EAuth;
using AlpimiAPI.Entities.ESchedule;
using AlpimiAPI.Entities.EUser;
using AlpimiAPI.Utilities;
using Castle.Components.DictionaryAdapter.Xml;
using Microsoft.EntityFrameworkCore;

namespace AlpimiAPI.Database
{
    public class Context(DbContextOptions<Context> options) : DbContext(options)
    {
        #region Entities


        public DbSet<User> User { get; set; }
        public DbSet<Auth> Auth { get; set; }
        public DbSet<Schedule> Schedule { get; set; }
        #endregion
    }
}
