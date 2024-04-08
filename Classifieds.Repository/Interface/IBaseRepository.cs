using System.Linq.Expressions;

namespace Classifieds.Repository.Interface
{
    public interface IBaseRepository<T> where T : class
    {
        Task<List<T>> GetAsync(Expression<Func<T, bool>> predicate = null);
        Task<List<R>> GetAsync<R>(Expression<Func<T, R>> selector, Expression<Func<T, bool>> predicate = null);
        IQueryable<T?> GetSet(Expression<Func<T, bool>> predicate = null);
        IQueryable<T?> GetSetAsTracking(Expression<Func<T, bool>> predicate = null);
        Task<T?> FindAsync(Expression<Func<T, bool>> predicate);
        Task<T?> FindForUpdateAsync(Expression<Func<T, bool>> predicate);
        Task<T> AddAsync(T entity, bool clearTracker = false);
        Task<int> AddRangeAsync(IEnumerable<T> entities, bool clearTracker = false);
        Task<T> UpdateAsync(T entity, bool clearTracker = false);
        Task<int> UpdateRangeAsync(IEnumerable<T> entities, bool clearTracker = false);
        Task<int> DeleteAsync(T entity, bool clearTracker = false);
        Task<int> DeleteRangeAsync(IEnumerable<T> entities, bool clearTracker = false);
        Task<int> SaveChangesAsync(bool clearTracker = false);
    }
}
