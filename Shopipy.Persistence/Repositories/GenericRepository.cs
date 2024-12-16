using System.Linq.Expressions;
using Microsoft.EntityFrameworkCore;
using Shopipy.Persistence.Data;

namespace Shopipy.Persistence.Repositories;

public class GenericRepository<T> : IGenericRepository<T> where T : class
{
    private readonly AppDbContext _context;

    public GenericRepository(AppDbContext context)
    {
        _context = context;
    }

    public async Task<IEnumerable<T>> GetAllAsync()
    {
        return await _context.Set<T>().ToListAsync();
    }
    
    public async Task<IEnumerable<T>> GetAllByConditionAsync(Expression<Func<T, bool>> predicate)
    {
        return await _context.Set<T>()
            .Where(predicate) 
            .ToListAsync();
    }

    public async Task<IEnumerable<T>> GetAllByConditionWithPaginationAsync(Expression<Func<T, bool>> predicate, int skip, int take)
    {
        return await _context.Set<T>().Where(predicate).Skip(skip).Take(take).ToListAsync();           
    }

    public async Task<int> GetCountByConditionAsync(Expression<Func<T, bool>> predicate)
    {
        return await _context.Set<T>().Where(predicate).CountAsync();           
    }

    public async Task<T?> GetByConditionAsync(Expression<Func<T, bool>> predicate)
    {
        return await _context.Set<T>().FirstOrDefaultAsync(predicate);
    }
    
    public async Task<T> GetByIdAsync(int id)
    {
        return await _context.Set<T>().FindAsync(id);
    }

    public async Task<T> AddAsync(T entity)
    {
        _context.Set<T>().Add(entity);
        await _context.SaveChangesAsync();
        return entity;
    }

    public async Task<T> AddWithoutSavingChangesAsync(T entity)
    {
        var entry = await _context.Set<T>().AddAsync(entity);
        return entry.Entity;
    }

    public async Task<T> UpdateAsync(T entity)
    {
        _context.Set<T>().Update(entity);
        await _context.SaveChangesAsync();
        return entity;
    }

    public async Task<bool> DeleteAsync(int id)
    {
        var entity = await _context.Set<T>().FindAsync(id);
        if (entity == null) return false;

        _context.Set<T>().Remove(entity);
        await _context.SaveChangesAsync();
        return true;
    }
    
    public async Task<bool> DeleteByConditionAsync(Expression<Func<T, bool>> predicate)
    {
        var entity = _context.Set<T>().FirstOrDefault(predicate);
        if (entity == null) return false;

        _context.Set<T>().Remove(entity);
        await _context.SaveChangesAsync();
        return true;
    }

    public Task SaveChangesAsync()
    {
        return _context.SaveChangesAsync();
    }
}
