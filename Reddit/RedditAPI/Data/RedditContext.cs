using Microsoft.EntityFrameworkCore;
using RedditDomain.Entities;

namespace RedditAPI.Data
{
    public class RedditContext : DbContext
    {
        public string DbPath { get; }

        public RedditContext(DbContextOptions options) : base(options)
        {
            DbPath = "bin/RedditDatabase.db";
        }

        public DbSet<Comment> Comments { get; set; }
        public DbSet<RedditDomain.Entities.Thread> Threads { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);
        }
        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
           
        }

    }
}
