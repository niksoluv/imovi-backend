using imovi_backend.Core.IRepositories;
using imovi_backend.Models;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace imovi_backend.Core.Repositories
{
    public class GenericRepository<T>: IGenericRepository<T> where T : class
    {
        protected ApplicationContext _context;
        protected DbSet<T> dbSet;
        protected readonly ILogger _logger;

        public GenericRepository(
            ApplicationContext context,
            ILogger logger)
        {
            _context = context;
            _logger = logger;
            dbSet = _context.Set<T>();
        }

        public virtual async Task<IEnumerable<T>> All(Guid userId)
        {
            return await dbSet.ToListAsync();
        }

        public virtual async Task<T> GetById(Guid id)
        {
            return await dbSet.FindAsync(id);
        }

        public virtual bool Add(T entity)
        {
            dbSet.Add(entity);
            return true;
        }

        public virtual Task<bool> Upsert(T entity)
        {
            throw new NotImplementedException();
        }

        public virtual async Task<bool> Delete(Guid id)
        {
            var user = await dbSet.FindAsync(id);
            dbSet.Remove(user);
            return true;
        }
    }
}
