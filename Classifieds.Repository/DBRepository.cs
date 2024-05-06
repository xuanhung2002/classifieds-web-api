using Classifieds.Data;
using Classifieds.Data.Entities;
using Microsoft.EntityFrameworkCore;
using System.Linq;
using System.Linq.Expressions;

namespace Classifieds.Repository
{
    public class DBRepository : IDBRepository
    {
        protected readonly DataContext _context;
        public DBRepository(DataContext context)
        {
            _context = context;
        }

        public async Task<T> AddAsync<T>(T entity, bool clearTracker = false) where T : class
        {
            if (entity is BaseEntity baseEntity)
            {
                baseEntity.CreatedAt = DateTime.Now;
            }
            var res = await _context.AddAsync(entity);
            await SaveChangesAsync(clearTracker);
            return res.Entity;
        }

        public async Task<int> AddRangeAsync<T>(IEnumerable<T> entities, bool clearTracker = false) where T : class
        {
            foreach (var entity in entities)
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

        public async Task<int> DeleteAsync<T>(T entity, bool clearTracker = false) where T : class
        {
            _context.Set<T>().Remove(entity);
            var res = await SaveChangesAsync(clearTracker);
            return res;

        }

        public async Task<int> DeleteRangeAsync<T>(IEnumerable<T> entities, bool clearTracker = false) where T : class
        {
            _context.Set<T>().RemoveRange(entities);
            var res = await SaveChangesAsync(clearTracker);
            return res;
        }

        public async Task<T?> FindAsync<T>(Expression<Func<T, bool>> predicate) where T : class
        {
            return await _context.Set<T>().AsNoTracking().FirstOrDefaultAsync(predicate);
        }

        public async Task<T?> FindForUpdateAsync<T>(Expression<Func<T, bool>> predicate) where T : class
        {
            return await _context.Set<T>().AsTracking().FirstOrDefaultAsync(predicate);
        }

        public async Task<List<T>> GetAsync<T>(Expression<Func<T, bool>> predicate = default) where T : class
        {
            if (predicate == null)
            {
                return await _context.Set<T>().ToListAsync();
            }
            return await _context.Set<T>().Where(predicate).ToListAsync();
        }

        public async Task<List<R>> GetAsync<T, R>(Expression<Func<T, R>> selector, Expression<Func<T, bool>> predicate = default) where T : class
        {
            if (predicate == null)
            {
                return await _context.Set<T>().Select(selector).ToListAsync();
            }
            return await _context.Set<T>().Where(predicate).Select(selector).ToListAsync();
        }

        public IQueryable<T> GetSet<T>(Expression<Func<T, bool>> predicate = null) where T : class
        {
            if (predicate == null)
            {
                return _context.Set<T>().AsNoTracking();
            }
            return _context.Set<T>().Where(predicate);
        }

        public IQueryable<T?> GetSetAsTracking<T>(Expression<Func<T, bool>> predicate = null) where T : class
        {
            if (predicate == null)
            {
                return _context.Set<T>();
            }
            return _context.Set<T>().Where(predicate);
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

        public async Task<T> UpdateAsync<T>(T entity, bool clearTracker = true) where T : class
        {
            if (entity is BaseEntity baseEntity)
            {
                baseEntity.UpdatedAt = DateTime.UtcNow;
            }
            var res = _context.Set<T>().Update(entity);
            await SaveChangesAsync(clearTracker);
            return res.Entity;
        }

        public async Task<int> UpdateRangeAsync<T>(IEnumerable<T> entities, bool clearTracker = false) where T : class
        {
            foreach (var entity in entities)
            {
                if (entity is BaseEntity baseEntity)
                {
                    baseEntity.UpdatedAt = DateTime.UtcNow;
                }
            }
            var res = await _context.SaveChangesAsync(clearTracker);
            if (clearTracker)
            {
                _context.ChangeTracker.Clear();
            }
            return res;
        }

        //public async Task<TableInfo<T>> GetWithPagingAsync<T>(TableQParameter<T> queryParameter) where T : class
        //{
        //    TableInfo<T> result = new TableInfo<T>();
        //    var internalResult = await GetWithPagingInternalAsync(queryParameter);
        //    result.PageCount = internalResult.PageCount;
        //    result.ItemsCount = internalResult.TotalCount;
        //    result.Items = await internalResult.ItemsQuery.ToListAsync();
        //    return result;
        //}

        //public async Task<TableInfo<R>> GetWithPagingAsync<T, R>(TableQParameter<T> queryParameter, Expression<Func<T, R>> selector) where T : class
        //{
        //    TableInfo<R> result = new TableInfo<R>();
        //    var internalResult = await GetWithPagingInternalAsync(queryParameter);
        //    result.PageCount = internalResult.PageCount;
        //    result.ItemsCount = internalResult.TotalCount;
        //    result.Items = await internalResult.ItemsQuery.Select(selector).ToListAsync();
        //    return result;
        //}
        //private async Task<(int PageCount, int TotalCount, IQueryable<T> ItemsQuery)> GetWithPagingInternalAsync<T>(TableQParameter<T> qParam, bool counting = true) where T : class
        //{
        //    int pageCount = 0;
        //    int totalCount = 0;
        //    var dbSet = _context.Set<T>();
        //    int skipCount = (qParam.PageIndex - 1) * qParam.PageSize;
        //    IOrderedQueryable<T> data;
        //    if(qParam.Sorter.IsAscending)
        //    {
        //        data = dbSet.OrderBy(qParam.Sorter.SortBy);
        //    }
        //    else
        //    {
        //        data = dbSet.OrderByDescending(qParam.Sorter.SortBy);

        //    }

        //    if(counting)
        //    {
        //        var allCount = totalCount = await data.CountAsync();
        //        if (allCount == 0)
        //        {
        //            pageCount = 1;
        //        }
        //        else
        //        {
        //            pageCount = allCount % qParam.PageSize == 0
        //                ? (allCount / qParam.PageSize)
        //                : (allCount / qParam.PageSize) + 1; ;
        //        }
        //    }

        //    IQueryable<T> query = skipCount == 0 && counting
        //        ? data.Take(qParam.PageSize)
        //        : data.Skip(skipCount).Take(qParam.PageSize);
                
        //    return new (pageCount, totalCount, query);
        //}


    }
}
