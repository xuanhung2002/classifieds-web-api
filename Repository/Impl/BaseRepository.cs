using Classifieds.Data;
using Classifieds.Data.Entites;
using Microsoft.EntityFrameworkCore;
using Repository.Interface;
using System.Linq.Expressions;

namespace Classifieds.Repository.Impl
{
    public class BaseRepository<T> : IBaseRepository<T> where T : BaseEntity
    {
        protected readonly DataContext _context;
        public BaseRepository(DataContext context)
        {
            _context = context;
        }

        public async Task<IEnumerable<T>> GetAll(bool trackChanges = true)
        {
            if (!trackChanges)
            {
                return await _context.Set<T>().AsNoTracking().ToListAsync();
            }
            return await _context.Set<T>().ToListAsync();
        }

        public async Task Add(T entity)
        {
            entity.CreatedAt = DateTime.UtcNow;
            await _context.Set<T>().AddAsync(entity);
        }

        public async Task AddRange(IEnumerable<T> entities)
        {
            foreach (var item in entities)
            {
                item.CreatedAt = DateTime.UtcNow;
            }
            await _context.Set<T>().AddRangeAsync(entities);
        }

        public async Task<IEnumerable<T>> Find(Expression<Func<T, bool>> expression, bool trackChanges = true)
        {
            if (!trackChanges)
            {
                return await _context.Set<T>().Where(expression).AsNoTracking().ToListAsync();
            }
            return await _context.Set<T>().Where(expression).ToListAsync();
        }

        public async Task<T> GetById(Guid id, bool trackChanges = true)
        {
            if (!trackChanges)
            {
                return await _context.Set<T>().AsNoTracking().FirstOrDefaultAsync(s => s.Id == id);
            }
            return await _context.Set<T>().FindAsync(id);
        }

        public Task Remove(T entity)
        {
           return Task.Run(() => _context.Set<T>().Remove(entity));
        }

        public Task RemoveRange(IEnumerable<T> entities)
        {
            return Task.Run(() => _context.RemoveRange(entities));
        }

        public Task Update(T entity)
        {   
            entity.UpdatedAt = DateTime.UtcNow;
            return Task.Run(() => _context.Set<T>().Update(entity));
        }
    }
}
