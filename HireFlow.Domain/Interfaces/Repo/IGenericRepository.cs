using System.Linq.Expressions;
using HireFlow.Domain.Abstractions;
using HireFlow.Domain.Entities;

namespace HireFlow.Domain.Interfaces.Repo;

public interface IGenericRepository<T> where T : BaseEntity
{   
    Task<T?> GetByIdAsync(Guid id, bool tracking = false);
    
    Task<T?> GetFirstOrDefaultAsync(Expression<Func<T, bool>> predicate, bool tracking = false);
    
    Task<List<T>> QueryAsync(Expression<Func<T, bool>> predicate, Paging paging, bool tracking = false);
    
    Task<bool> AnyAsync(Expression<Func<T, bool>> predicate);
    
    Task AddAsync(T entity);
    
    void Update(T entity);
    
    void Delete(T entity);
    
    void SoftDelete(T entity, Guid requesterId);
    
    Task<int> CountAsync(Expression<Func<T, bool>> predicate);
    
}