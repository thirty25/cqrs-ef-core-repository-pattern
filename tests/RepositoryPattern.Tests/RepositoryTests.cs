using System.Linq;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using RepositoryPattern.Data;
using Xunit;
using Xunit.Abstractions;

namespace RepositoryPattern.Tests
{
    public class RepositoryTests
    {
        private readonly ITestOutputHelper _output;

        public RepositoryTests(ITestOutputHelper output)
        {
            _output = output;
        }
        
        [Fact]
        public async Task Can_add_and_query_a_blog()
        {
            var container = new ContainerBuilder(_output)
                .Build();
            
            var commandRepository = container.GetInstance<ICommandRepository<BloggingContext>>();
            
            var blog = new Blog()
            {
                Url = "http://example.com/rss.xml",
                BlogId = 123
            };

            await commandRepository.Set<Blog>().AddAsync(blog);
            await commandRepository.SaveChangesAsync();
            
            var queryRepository = container.GetInstance<IQueryRepository<BloggingContext>>();
            var results = await queryRepository
                .Query<Blog>()
                .Where(b => b.Url.Contains("example.com"))
                .ToListAsync();
            
            Assert.NotEmpty(results);
            Assert.Equal(123, results[0].BlogId);
        }
        
        [Fact]
        public async Task Can_add_and_query_a_blog_with_a_lambda()
        {
            var container = new ContainerBuilder(_output)
                .Build();
            
            var commandRepository = container.GetInstance<ICommandRepository<BloggingContext>>();
            
            var blog = new Blog()
            {
                Url = "http://example.com/rss.xml",
                BlogId = 123
            };

            await commandRepository.Set(i => i.Blogs).AddAsync(blog);
            await commandRepository.SaveChangesAsync();
            
            var queryRepository = container.GetInstance<IQueryRepository<BloggingContext>>();
            var results = await queryRepository
                .Query(i => i.Blogs)
                .Where(b => b.Url.Contains("example.com"))
                .ToListAsync();
            
            Assert.NotEmpty(results);
            Assert.Equal(123, results[0].BlogId);
        }
    }
}
