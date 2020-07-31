using System;
using Lamar;
using Microsoft.Data.Sqlite;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using RepositoryPattern.Data;
using Xunit.Abstractions;

namespace RepositoryPattern.Tests
{
    public class ContainerBuilder : IDisposable
    {
        private readonly ITestOutputHelper _output;
        private readonly SqliteConnection _connection;

        public ContainerBuilder(ITestOutputHelper output)
        {
            _output = output;
            
            // for now an in-memory sqlite db connection needs to be babysat
            // a bit more than usual. we'll open and close the connection manually
            // see https://github.com/dotnet/efcore/issues/16103 for future changes
            _connection = new SqliteConnection("Filename=:memory:");
            _connection.Open();
        }

        public IContainer Build()
        {
            var container = new Container(x =>
            {
                x.For<BloggingContext>().Use(i =>
                    {
                        var options = new DbContextOptionsBuilder<DbContext>()
                            .UseSqlite(_connection)
                            .UseLoggerFactory(LoggerFactory.Create(builder => builder.AddXUnit(_output)))
                            .Options;
            
                        var context = new BloggingContext(options);
            
                        context.Database.EnsureDeleted();
                        context.Database.EnsureCreated();
                        return context;
                    }
                ).Singleton();
                x.For(typeof(IQueryRepository<>)).Use(typeof(QueryRepository<>));
                x.For(typeof(ICommandRepository<>)).Use(typeof(CommandRepository<>));
            });
            
            return container;
        }

        public void Dispose()
        {
            _connection?.Dispose();
        }
    }
}