using AlpimiAPI.Auth;
using AlpimiAPI.Breed;
using AlpimiAPI.Dog;
using AlpimiAPI.User;
using Microsoft.EntityFrameworkCore;

namespace alpimi_planner_backend.API
{
    public class Context : DbContext
    {
        #region Entities

        public DbSet<Breed> Breed { get; set; }
        public DbSet<Dog> Dog { get; set; }
        public DbSet<User> User { get; set; }
        public DbSet<Auth> Auth { get; set; }
        #endregion
        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
            optionsBuilder.UseSqlServer(Utilities.Configuration.GetConnectionString());
        }
    }
}
