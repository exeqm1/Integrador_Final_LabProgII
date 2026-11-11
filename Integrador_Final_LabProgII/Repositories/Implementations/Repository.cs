using ITManager.Data;
using ITManager.Repositories.Interfaces;
using Microsoft.EntityFrameworkCore;

namespace ITManager.Repositories.Implementations;

public class Repository<T> : IRepository<T> where T : class
{
    protected readonly AppDbContext _context;
    protected readonly DbSet<T> _dbSet;

    public Repository(AppDbContext context)
    {
        _context = context;
        _dbSet = context.Set<T>();
    }

    public virtual async Task<T?> GetByIdAsync(int id)
    {
        return await _dbSet.FindAsync(id);
    }

    public virtual async Task<IEnumerable<T>> GetAllAsync()
    {
        return await _dbSet.AsNoTracking().ToListAsync();
    }

    public virtual async Task<int> CreateAsync(T entity)
    {
        await _dbSet.AddAsync(entity);
        await _context.SaveChangesAsync();

        var idProp = typeof(T).GetProperty("Id");
        if (idProp != null && idProp.PropertyType == typeof(int))
        {
            return (int)idProp.GetValue(entity)!;
        }

        return 1;
    }

    public virtual async Task<bool> UpdateAsync(T entity)
    {
        _dbSet.Update(entity);
        return await _context.SaveChangesAsync() > 0;
    }

    public virtual async Task<bool> DeleteAsync(int id)
    {
        var entity = await _dbSet.FindAsync(id);
        if (entity == null)
        {
            return false;
        }

        _dbSet.Remove(entity);
        return await _context.SaveChangesAsync() > 0;
    }
}

