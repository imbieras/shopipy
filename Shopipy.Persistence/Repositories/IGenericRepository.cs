using System.Collections.Generic;
using System.Linq.Expressions;
using System.Threading.Tasks;

namespace Shopipy.Persistence.Repositories;

public interface IGenericRepository<T> where T : class
{
    Task<IEnumerable<T>> GetAllAsync();
    Task<IEnumerable<T>> GetAllByConditionAsync(Expression<Func<T, bool>> predicate);
    Task<T> GetByConditionAsync(Expression<Func<T, bool>> predicate);
    Task<T> GetByIdAsync(int id);
    Task<T> AddAsync(T entity);
    Task<T> UpdateAsync(T entity);
    Task<bool> DeleteAsync(int id);
}