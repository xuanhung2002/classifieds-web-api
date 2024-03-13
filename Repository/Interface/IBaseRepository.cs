using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using System.Threading.Tasks;

namespace Repository.Interface
{
    public interface IBaseRepository<T> where T : class
    {
        Task<T> GetById(Guid id, bool trackChanges = true);
        Task<IEnumerable<T>> GetAll(bool trackChanges = true);
        Task<IEnumerable<T>> Find(Expression<Func<T, bool>> expression, bool trackChanges = true);
        Task Add(T entity);
        Task AddRange(IEnumerable<T> entities);
        Task Update(T entity);
        Task Remove(T entity);
        Task RemoveRange(IEnumerable<T> entities);
    }
}
