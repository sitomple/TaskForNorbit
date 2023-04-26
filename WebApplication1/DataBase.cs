using Microsoft.EntityFrameworkCore;

namespace WebApplication1
{
    public class DataBase : DbContext
    {
        public DbSet<UserModel> User { get; set; }
        public DbSet<MBI_userModel> MBI_user { get; set; }
        public DbSet<GroupByCategoryModel> GroupByCategory { get; set; }
        public DataBase()
        {
            Database.EnsureCreated();
        }
        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
            optionsBuilder.UseNpgsql("Host=localhost;Port=5432;Database=Norbit;Username=postgres;Password=Qwerty1qaz");
        }
    }
}
