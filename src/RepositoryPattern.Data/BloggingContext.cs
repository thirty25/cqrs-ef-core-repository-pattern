using Microsoft.EntityFrameworkCore;

namespace RepositoryPattern.Data
{
    public class BloggingContext : DbContext
    {
        public BloggingContext(DbContextOptions options) : base(options)
        {
        }

        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
        }

        public DbSet<Blog> Blogs { get; set; }
    }
}