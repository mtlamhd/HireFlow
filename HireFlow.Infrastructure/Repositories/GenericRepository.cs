using System.Linq.Expressions;
using HireFlow.Domain.Abstractions;
using HireFlow.Domain.Entities;
using HireFlow.Domain.Interfaces.Repo;
using HireFlow.Infrastructure.Data;
using Microsoft.EntityFrameworkCore;

namespace HireFlow.Infrastructure.Repositories;

public class GenericRepository<T> : IGenericRepository<T> where T : BaseEntity
{
    protected readonly  AppDbContext _context;
    protected readonly DbSet<T> _dbSet;

    public GenericRepository(AppDbContext context)
    {
        _context = context;
        _dbSet = _context.Set<T>();
    }
    public async Task<T?> GetByIdAsync(Guid id, bool tracking = false)
    {
        var query = _dbSet.AsQueryable();

        if (!tracking)
        {
            query = query.AsNoTracking();
        }

        return await query.FirstOrDefaultAsync(x => x.Id == id);
    }

    public async Task<T?> GetFirstOrDefaultAsync(Expression<Func<T, bool>> predicate, bool tracking = false)
    {
        var query = _dbSet.AsQueryable();

        if (!tracking)
        {
            query = query.AsNoTracking();
        }

        return await query.FirstOrDefaultAsync(predicate);
    }

    public async Task<List<T>> QueryAsync(Expression<Func<T, bool>> predicate, Paging paging, bool tracking = false)
    {
        var query = _dbSet.AsQueryable();

        if (!tracking)
        {
            query = query.AsNoTracking();
        }

        return await query
            .Where(predicate)
            .OrderByDescending(x => x.CreatedAt)
            .Skip(paging.GetSkip())
            .Take(paging.PageSize)
            .ToListAsync();
    }

    public async Task<bool> AnyAsync(Expression<Func<T, bool>> predicate)
    {
        return await _dbSet.AnyAsync(predicate);
    }

   
    public async Task AddAsync(T entity) 
    {
        await _dbSet.AddAsync(entity);
    }
    

    public void Update(T entity)
    {
        _dbSet.Update(entity);
    }

    public void Delete(T entity)
    {
        _dbSet.Remove(entity);
    }

    public void SoftDelete(T entity, Guid requesterId)
    {
        entity.SetAsDeleted(requesterId);
    }
    
    public async Task<int> CountAsync(Expression<Func<T, bool>> predicate)
    {
        return await _dbSet.CountAsync(predicate);
    }
}