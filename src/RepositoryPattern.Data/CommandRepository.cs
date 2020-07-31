using System;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.ChangeTracking;

namespace RepositoryPattern.Data
{
    public interface ICommandRepository<out TContext> where TContext : DbContext
    {
        IQueryable<T> Query<T>(
            [System.Runtime.CompilerServices.CallerMemberName] 
            string memberName = "", 
            [System.Runtime.CompilerServices.CallerFilePath]
            string sourceFilePath = "", 
            [System.Runtime.CompilerServices.CallerLineNumber]
            int sourceLineNumber = 0) where T : class;

        IQueryable<T> Query<T>(
            Func<TContext, DbSet<T>> action,
            [System.Runtime.CompilerServices.CallerMemberName] 
            string memberName = "", 
            [System.Runtime.CompilerServices.CallerFilePath]
            string sourceFilePath = "", 
            [System.Runtime.CompilerServices.CallerLineNumber]
            int sourceLineNumber = 0) where T : class;

        DbSet<T> Set<T>() where T : class;
        DbSet<T> Set<T>(Func<TContext, DbSet<T>> action) where T : class;
        Task SaveChangesAsync();
    }

    public class CommandRepository<TContext> : ICommandRepository<TContext> where TContext : DbContext
    {
        private readonly TContext _context;
        
        public CommandRepository(TContext context) => _context = context;

        public Task SaveChangesAsync() => _context.SaveChangesAsync();
        public DbSet<T> Set<T>() where T : class => _context.Set<T>();
        public DbSet<T> Set<T>(Func<TContext, DbSet<T>> action) where T : class => action.Invoke(_context);

        public IQueryable<T> Query<T>(
            Func<TContext, DbSet<T>> action, 
            [System.Runtime.CompilerServices.CallerMemberName]
            string memberName = "", 
            [System.Runtime.CompilerServices.CallerFilePath]
            string sourceFilePath = "", 
            [System.Runtime.CompilerServices.CallerLineNumber]
            int sourceLineNumber = 0) where T : class
        {
            return action.Invoke(_context)
                .TagWith($"{memberName} {sourceFilePath}:{sourceLineNumber}");
        }
        
        public IQueryable<T> Query<T>(
            [System.Runtime.CompilerServices.CallerMemberName]
            string memberName = "", 
            [System.Runtime.CompilerServices.CallerFilePath]
            string sourceFilePath = "", 
            [System.Runtime.CompilerServices.CallerLineNumber]
            int sourceLineNumber = 0) where T : class
        {
            return _context.Set<T>()
                .TagWith($"{memberName} {sourceFilePath}:{sourceLineNumber}");
        }

    }
}
