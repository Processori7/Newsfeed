using System;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;

namespace PostgresDb.Data
{
    public class ApiDbContext : DbContext
    {
        public ApiDbContext(DbContextOptions<ApiDbContext> options) : base(options)
        {
            Database.EnsureCreated();
        }

        //For tests
        public ApiDbContext()
        {

        }

        public DbSet<RegisterUser> V1_create_users_table { get; set; }

        public DbSet<NewsModel> V3_create_news_table { get; set; }
    }
}
