using Microsoft.EntityFrameworkCore;
using VK_TEST.VK_T_Objects.Entity;

namespace VK_TEST.DataBase
{
    public class AppContext : DbContext
    {
        public DbSet<User> Users { get; set; }
        public DbSet<UserState> UserStates { get; set; }
        public DbSet<UserGroup> UserGroups { get; set; }
        public DbSet<LimiterCount> LimiterCounts { get; set; }

        /// <summary>
        /// Add base objects in db
        /// </summary>
        private void Init()
        {
            if (LimiterCounts.Count() == 0)
            {
                LimiterCounts.AddRange(LimiterCount.AllLimiters);
            }

            if (UserGroups.Count() == 0)
            {
                UserGroups.AddRange(UserGroup.AllGroups);
            }

            if (UserStates.Count() == 0)
            {
                UserStates.AddRange(UserState.AllStates);
            }
            this.SaveChanges();
        }

        public AppContext(DbContextOptions<AppContext> options)
            : base(options)
        {
            try
            {
                Database.EnsureCreated();
                if (LimiterCounts.AsEnumerable().Where(x => x.Header == LimiterCount.AllLimiters[0].Header).First().Count == 0) Init();
            }
            catch (Exception ex)
            {
                Init();
            }
        }
    }
}
