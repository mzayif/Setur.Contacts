using System.Linq.Expressions;
using Microsoft.EntityFrameworkCore;
using Setur.Contacts.Base.Repositories;
using Setur.Contacts.ContactApi.Data;
using Setur.Contacts.Domain.Entities;

namespace Setur.Contacts.ContactApi.Repositories;

public class CommunicationInfoRepository : IRepository<CommunicationInfo, Guid>
{
    private readonly ContactDbContext _context;

    public CommunicationInfoRepository(ContactDbContext context)
    {
        _context = context;
    }

    public DbSet<CommunicationInfo> Table => _context.CommunicationInfos;

    public async Task<bool> AddAsync(CommunicationInfo entity)
    {
        await _context.CommunicationInfos.AddAsync(entity);
        return true;
    }

    public async Task<bool> AddAsync(List<CommunicationInfo> entities)
    {
        await _context.CommunicationInfos.AddRangeAsync(entities);
        return true;
    }

    public bool Update(CommunicationInfo entity)
    {
        _context.CommunicationInfos.Update(entity);
        return true;
    }

    public bool Cancel(CommunicationInfo entity)
    {
        _context.CommunicationInfos.Update(entity);
        return true;
    }

    public async Task<bool> CancelAsync(Guid id)
    {
        var entity = await GetByIdAsync(id);
        if (entity != null)
        {
            return Cancel(entity);
        }
        return false;
    }

    public bool Activate(CommunicationInfo entity)
    {
        _context.CommunicationInfos.Update(entity);
        return true;
    }

    public async Task<bool> ActivateAsync(Guid id)
    {
        var entity = await GetByIdAsync(id);
        if (entity != null)
        {
            return Activate(entity);
        }
        return false;
    }

    public bool Remove(CommunicationInfo entity)
    {
        _context.CommunicationInfos.Remove(entity);
        return true;
    }

    public bool Remove(List<CommunicationInfo> entities)
    {
        _context.CommunicationInfos.RemoveRange(entities);
        return true;
    }

    public async Task<bool> RemoveAsync(Guid id)
    {
        var entity = await GetByIdAsync(id);
        if (entity != null)
        {
            return Remove(entity);
        }
        return false;
    }

    public async Task<int> SaveAsync()
    {
        return await _context.SaveChangesAsync();
    }

    public IQueryable<CommunicationInfo> GetAll(bool isTracking = true)
    {
        return isTracking ? _context.CommunicationInfos : _context.CommunicationInfos.AsNoTracking();
    }

    public IQueryable<CommunicationInfo> GetWhere(Expression<Func<CommunicationInfo, bool>>? method = null, bool isTracking = true)
    {
        var query = isTracking ? _context.CommunicationInfos : _context.CommunicationInfos.AsNoTracking();
        return method != null ? query.Where(method) : query;
    }

    public IQueryable<CommunicationInfo> GetWhere(Expression<Func<CommunicationInfo, bool>>? method = null, string? includeProperties = null, bool isTracking = true)
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

    public async Task<CommunicationInfo?> GetSingleAsync(Expression<Func<CommunicationInfo, bool>> method, bool isTracking = true)
    {
        return await GetWhere(method, isTracking).FirstOrDefaultAsync();
    }

    public async Task<CommunicationInfo?> GetByIdAsync(Guid id, bool throwException = true, bool isTracking = true)
    {
        var entity = await GetWhere(x => x.Id == id, isTracking).FirstOrDefaultAsync();
            
        if (entity == null && throwException)
        {
            throw new FileNotFoundException($"CommunicationInfo with id {id} not found");
        }
            
        return entity;
    }

    public bool Exists(Expression<Func<CommunicationInfo, bool>> filter)
    {
        return _context.CommunicationInfos.Any(filter);
    }

    public async Task<bool> ExistsAsync(Expression<Func<CommunicationInfo, bool>> filter)
    {
        return await _context.CommunicationInfos.AnyAsync(filter);
    }

    public async Task<int> ExecuteSqlCommandAsync(string query)
    {
        return await _context.Database.ExecuteSqlRawAsync(query);
    }

    public async Task<List<TReturnEntity>> ExecuteQueryAsync<TReturnEntity>(string query, params object[] parameters) where TReturnEntity : class, new()
    {
        return await _context.Set<TReturnEntity>().FromSqlRaw(query, parameters).ToListAsync();
    }
}