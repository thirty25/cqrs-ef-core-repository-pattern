using System;
using System.Linq;
using Microsoft.EntityFrameworkCore;

namespace RepositoryPattern.Data
{
    public interface IQueryRepository<out TContext> where TContext : DbContext
    {
        IQueryable<T> Query<T>(
            [System.Runtime.CompilerServices.CallerMemberName] 
            string memberName = "", 
            [System.Runtime.CompilerServices.CallerFilePath]
            string sourceFilePath = "", 
            [System.Runtime.CompilerServices.CallerLineNumber]
            int sourceLineNumber = 0) where T : class;

        IQueryable<T> Query<T>(Func<TContext, DbSet<T>> action,
            [System.Runtime.CompilerServices.CallerMemberName]
            string memberName = "",
            [System.Runtime.CompilerServices.CallerFilePath]
            string sourceFilePath = "",
            [System.Runtime.CompilerServices.CallerLineNumber]
            int sourceLineNumber = 0) where T : class;
    }

    public class QueryRepository<TContext> : IQueryRepository<TContext> where TContext : DbContext
    {
        private readonly TContext _context;

        public QueryRepository(TContext context)
        {
            _context = context;
        }

        public IQueryable<T> Query<T>(Func<TContext, DbSet<T>> action,
            [System.Runtime.CompilerServices.CallerMemberName] 
            string memberName = "", 
            [System.Runtime.CompilerServices.CallerFilePath]
            string sourceFilePath = "", 
            [System.Runtime.CompilerServices.CallerLineNumber]
            int sourceLineNumber = 0) where T : class
        {
            return action.Invoke(_context)
                .AsNoTracking()
                .TagWith($"{memberName}() {sourceFilePath}:{sourceLineNumber}");
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
                .AsNoTracking()
                .TagWith($"{memberName}() {sourceFilePath}:{sourceLineNumber}");
        }
    }
}