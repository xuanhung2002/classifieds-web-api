using Classifieds.Data;
using Classifieds.Data.Entites;
using Classifieds.Repository.Interface;
using Microsoft.EntityFrameworkCore;
using System.Linq;
using System.Linq.Expressions;

namespace Classifieds.Repository.Impl
{
    public class BaseRepository<T> : IBaseRepository<T> where T : class
    {
        protected readonly DataContext _context;
        public BaseRepository(DataContext context)
        {
            _context = context;
        }

        public async Task<T> AddAsync(T entity, bool clearTracker = false)
        {   
            if(entity is BaseEntity baseEntity)
            {
                baseEntity.CreatedAt = DateTime.Now;
            }
            var res = await _context.AddAsync(entity);
            await SaveChangesAsync(clearTracker);
            return res.Entity;
        }

        public async Task<int> AddRangeAsync(IEnumerable<T> entities, bool clearTracker = false)
        {
            foreach(var entity in entities)
            {
                if (entity is BaseEntity baseEntity)
                {
                    baseEntity.CreatedAt = DateTime.Now;
                }
            }
            await _context.Set<T>().AddRangeAsync(entities);
            var res = await SaveChangesAsync(clearTracker);
            return res;
        }

        public async Task<int> DeleteAsync(T entity, bool clearTracker = false)
        {
            _context.Set<T>().Remove(entity);
            var res = await SaveChangesAsync(clearTracker);
            return res;

        }

        public async Task<int> DeleteRangeAsync(IEnumerable<T> entities, bool clearTracker = false)
        {
            _context.Set<T>().RemoveRange(entities);
            var res = await SaveChangesAsync(clearTracker);
            return res;
        }

        public async Task<T?> FindAsync(Expression<Func<T, bool>> predicate)
        {
            return await _context.Set<T>().FirstOrDefaultAsync(predicate);
        }

        public async Task<T?> FindForUpdateAsync(Expression<Func<T, bool>> predicate)
        {
            return await _context.Set<T>().AsTracking().FirstOrDefaultAsync(predicate);
        }

        public async Task<List<T>> GetAsync(Expression<Func<T, bool>> predicate = default)
        {
            if(predicate == null)
            {
                return await _context.Set<T>().ToListAsync();
            }
            return await _context.Set<T>().Where(predicate).ToListAsync();
        }

        public async Task<List<R>> GetAsync<R>(Expression<Func<T, R>> selector, Expression<Func<T, bool>> predicate = default)
        {
            if(predicate == null)
            {
                return await _context.Set<T>().Select(selector).ToListAsync();
            }
            return await _context.Set<T>().Where(predicate).Select(selector).ToListAsync();
        }

        public IQueryable<T> GetSet(Expression<Func<T, bool>> predicate = null)
        {
            if (predicate == null)
            {
                return _context.Set<T>();
            }
            return _context.Set<T>().Where(predicate);
        }

        public IQueryable<T?> GetSetAsTracking(Expression<Func<T, bool>> predicate = null)
        {
            if (predicate == null)
            {
                return _context.Set<T>().AsTracking();
            }
            return _context.Set<T>().Where(predicate).AsTracking();
        }

        public async Task<int> SaveChangesAsync(bool clearTracker = false)
        {
            var res = await _context.SaveChangesAsync();
            if (clearTracker)
            {
                _context.ChangeTracker.Clear();
            }
            return res;
        }

        public async Task<T> UpdateAsync(T entity, bool clearTracker = false)
        {
            if(entity is BaseEntity baseEntity)
            {
                baseEntity.UpdatedAt = DateTime.UtcNow;
            }
            var res = _context.Set<T>().Update(entity);
            await SaveChangesAsync(clearTracker);
            return res.Entity;
        }

        public async Task<int> UpdateRangeAsync(IEnumerable<T> entities, bool clearTracker = false)
        {   
            foreach(var entity in entities)
            {
                if (entity is BaseEntity baseEntity)
                {
                    baseEntity.UpdatedAt = DateTime.UtcNow;
                }
            }
            var res = await _context.SaveChangesAsync(clearTracker);
            if(clearTracker)
            {
                _context.ChangeTracker.Clear();
            }
            return res;
        }

    }
}
