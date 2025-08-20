using Microsoft.EntityFrameworkCore;
using Setur.Contacts.Base.Repositories;
using Setur.Contacts.Domain.Entities;
using Setur.Contacts.ReportApi.Data;
using System.Linq.Expressions;

namespace Setur.Contacts.ReportApi.Repositories;

public class ReportDetailRepository : IRepository<ReportDetail, Guid>
{
    private readonly ReportDbContext _context;

    public ReportDetailRepository(ReportDbContext context)
    {
        _context = context;
    }

    public DbSet<ReportDetail> Table => _context.ReportDetails;

    public async Task<bool> AddAsync(ReportDetail entity)
    {
        await _context.ReportDetails.AddAsync(entity);
        return true;
    }

    public async Task<bool> AddAsync(List<ReportDetail> entities)
    {
        await _context.ReportDetails.AddRangeAsync(entities);
        return true;
    }

    public bool Update(ReportDetail entity)
    {
        _context.ReportDetails.Update(entity);
        return true;
    }

    public bool Cancel(ReportDetail entity)
    {
        _context.ReportDetails.Update(entity);
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

    public bool Activate(ReportDetail entity)
    {
        _context.ReportDetails.Update(entity);
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

    public bool Remove(ReportDetail entity)
    {
        _context.ReportDetails.Remove(entity);
        return true;
    }

    public bool Remove(List<ReportDetail> entities)
    {
        _context.ReportDetails.RemoveRange(entities);
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

    public IQueryable<ReportDetail> GetAll(bool isTracking = true)
    {
        return isTracking ? _context.ReportDetails : _context.ReportDetails.AsNoTracking();
    }

    public IQueryable<ReportDetail> GetWhere(Expression<Func<ReportDetail, bool>>? method = null, bool isTracking = true)
    {
        var query = isTracking ? _context.ReportDetails : _context.ReportDetails.AsNoTracking();
        return method != null ? query.Where(method) : query;
    }

    public IQueryable<ReportDetail> GetWhere(Expression<Func<ReportDetail, bool>>? method = null, string? includeProperties = null, bool isTracking = true)
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

    public async Task<ReportDetail?> GetSingleAsync(Expression<Func<ReportDetail, bool>> method, bool isTracking = true)
    {
        return await GetWhere(method, isTracking).FirstOrDefaultAsync();
    }

    public async Task<ReportDetail?> GetByIdAsync(Guid id, bool throwException = true, bool isTracking = true)
    {
        var entity = await GetWhere(x => x.Id == id, isTracking).FirstOrDefaultAsync();

        if (entity == null && throwException)
        {
            throw new FileNotFoundException($"ReportDetail with id {id} not found");
        }

        return entity;
    }

    public bool Exists(Expression<Func<ReportDetail, bool>> filter)
    {
        return _context.ReportDetails.Any(filter);
    }

    public async Task<bool> ExistsAsync(Expression<Func<ReportDetail, bool>> filter)
    {
        return await _context.ReportDetails.AnyAsync(filter);
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
