using System.Linq.Expressions;
using Microsoft.EntityFrameworkCore;

namespace Setur.Contacts.Base.Repositories;

/// <summary>
/// Generic Repository sınıfı. Tüm entity'ler için ortak repository işlemlerini sağlar.
/// </summary>
/// <typeparam name="TEntity">İşlem yapılacak Entity</typeparam>
/// <typeparam name="TId">Entity'ye uygun Uniq Id Tipi</typeparam>
public abstract class Repository<TEntity, TId> : IRepository<TEntity, TId> where TEntity : class
{
    protected readonly DbContext _context;
    protected readonly DbSet<TEntity> _dbSet;

    protected Repository(DbContext context)
    {
        _context = context;
        _dbSet = context.Set<TEntity>();
    }

    public DbSet<TEntity> Table => _dbSet;

    public virtual async Task<bool> AddAsync(TEntity entity)
    {
        await _dbSet.AddAsync(entity);
        return true;
    }

    public virtual async Task<bool> AddAsync(List<TEntity> entities)
    {
        await _dbSet.AddRangeAsync(entities);
        return true;
    }

    public virtual bool Update(TEntity entity)
    {
        _dbSet.Update(entity);
        return true;
    }

    public virtual bool Cancel(TEntity entity)
    {
        _dbSet.Update(entity);
        return true;
    }

    public virtual async Task<bool> CancelAsync(TId id)
    {
        var entity = await GetByIdAsync(id);
        if (entity != null)
        {
            return Cancel(entity);
        }
        return false;
    }

    public virtual bool Activate(TEntity entity)
    {
        _dbSet.Update(entity);
        return true;
    }

    public virtual async Task<bool> ActivateAsync(TId id)
    {
        var entity = await GetByIdAsync(id);
        if (entity != null)
        {
            return Activate(entity);
        }
        return false;
    }

    public virtual bool Remove(TEntity entity)
    {
        _dbSet.Remove(entity);
        return true;
    }

    public virtual bool Remove(List<TEntity> entities)
    {
        _dbSet.RemoveRange(entities);
        return true;
    }

    public virtual async Task<bool> RemoveAsync(TId id)
    {
        var entity = await GetByIdAsync(id);
        if (entity != null)
        {
            return Remove(entity);
        }
        return false;
    }

    public virtual async Task<int> SaveAsync()
    {
        return await _context.SaveChangesAsync();
    }

    public virtual IQueryable<TEntity> GetAll(bool isTracking = true)
    {
        return isTracking ? _dbSet : _dbSet.AsNoTracking();
    }

    public virtual IQueryable<TEntity> GetWhere(Expression<Func<TEntity, bool>>? method = null, bool isTracking = true)
    {
        var query = isTracking ? _dbSet : _dbSet.AsNoTracking();
        return method != null ? query.Where(method) : query;
    }

    public virtual IQueryable<TEntity> GetWhere(Expression<Func<TEntity, bool>>? method = null, string? includeProperties = null, bool isTracking = true)
    {
        var query = GetWhere(method, isTracking);
            
        if (!string.IsNullOrEmpty(includeProperties))
        {
            foreach (var includeProperty in includeProperties.Split(new char[] { ',' }, StringSplitOptions.RemoveEmptyEntries))
            {
                query = query.Include(includeProperty.Trim());
            }
        }
            
        return query;
    }

    public virtual async Task<TEntity?> GetSingleAsync(Expression<Func<TEntity, bool>> method, bool isTracking = true)
    {
        return await GetWhere(method, isTracking).FirstOrDefaultAsync();
    }

    public virtual async Task<TEntity?> GetByIdAsync(TId id, bool throwException = true, bool isTracking = true)
    {
        var entity = await GetWhere(x => EF.Property<TId>(x, "Id").Equals(id), isTracking).FirstOrDefaultAsync();
            
        if (entity == null && throwException)
        {
            throw new FileNotFoundException($"{typeof(TEntity).Name} with id {id} not found");
        }
            
        return entity;
    }

    public virtual bool Exists(Expression<Func<TEntity, bool>> filter)
    {
        return _dbSet.Any(filter);
    }

    public virtual async Task<bool> ExistsAsync(Expression<Func<TEntity, bool>> filter)
    {
        return await _dbSet.AnyAsync(filter);
    }

    public virtual async Task<int> ExecuteSqlCommandAsync(string query)
    {
        return await _context.Database.ExecuteSqlRawAsync(query);
    }

    public virtual async Task<List<TReturnEntity>> ExecuteQueryAsync<TReturnEntity>(string query, params object[] parameters) where TReturnEntity : class, new()
    {
        return await _context.Set<TReturnEntity>().FromSqlRaw(query, parameters).ToListAsync();
    }
}
