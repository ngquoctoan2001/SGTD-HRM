using System.Linq.Expressions;
using HRM.Domain.Entities;

namespace HRM.Domain.Interfaces;

/// <summary>
/// Generic repository interface for CRUD operations.
/// </summary>
public interface IRepository<T> where T : BaseEntity
{
    Task<T?> GetByIdAsync(int id);
    Task<IEnumerable<T>> GetAllAsync();
    Task<IEnumerable<T>> FindAsync(Expression<Func<T, bool>> predicate);
    Task<(IEnumerable<T> Items, int TotalCount)> GetPagedAsync(
        int pageNumber, int pageSize,
        Expression<Func<T, bool>>? filter = null,
        Func<IQueryable<T>, IOrderedQueryable<T>>? orderBy = null);
    Task AddAsync(T entity);
    void Update(T entity);
    void Delete(T entity);
    void SoftDelete(T entity);
    Task<int> CountAsync(Expression<Func<T, bool>>? filter = null);
}
