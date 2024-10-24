using AlpimiAPI.Entities.EAuth;
using AlpimiAPI.Entities.EUser;
using AlpimiAPI.Utilities;
using Microsoft.EntityFrameworkCore;

namespace AlpimiAPI
{
    public class Context : DbContext
    {
        #region Entities

        public DbSet<AlpimiAPI.Breed.Breed> Breed { get; set; }
        public DbSet<AlpimiAPI.Dog.Dog> Dog { get; set; }
        public DbSet<User> User { get; set; }
        public DbSet<Auth> Auth { get; set; }
        #endregion
        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
            optionsBuilder.UseSqlServer(Configuration.GetConnectionString());
        }
    }
}
