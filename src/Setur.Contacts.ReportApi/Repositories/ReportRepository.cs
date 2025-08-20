using Microsoft.EntityFrameworkCore;
using Setur.Contacts.Base.Repositories;
using Setur.Contacts.Domain.Entities;
using Setur.Contacts.ReportApi.Data;
using System.Linq.Expressions;

namespace Setur.Contacts.ReportApi.Repositories;

public class ReportRepository : IRepository<Report, Guid>
{
    private readonly ReportDbContext _context;

    public ReportRepository(ReportDbContext context)
    {
        _context = context;
    }

    public DbSet<Report> Table => _context.Reports;

    public async Task<bool> AddAsync(Report entity)
    {
        await _context.Reports.AddAsync(entity);
        return true;
    }

    public async Task<bool> AddAsync(List<Report> entities)
    {
        await _context.Reports.AddRangeAsync(entities);
        return true;
    }

    public bool Update(Report entity)
    {
        _context.Reports.Update(entity);
        return true;
    }

    public bool Cancel(Report entity)
    {
        _context.Reports.Update(entity);
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

    public bool Activate(Report entity)
    {
        _context.Reports.Update(entity);
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

    public bool Remove(Report entity)
    {
        _context.Reports.Remove(entity);
        return true;
    }

    public bool Remove(List<Report> entities)
    {
        _context.Reports.RemoveRange(entities);
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

    public IQueryable<Report> GetAll(bool isTracking = true)
    {
        return isTracking ? _context.Reports : _context.Reports.AsNoTracking();
    }

    public IQueryable<Report> GetWhere(Expression<Func<Report, bool>>? method = null, bool isTracking = true)
    {
        var query = isTracking ? _context.Reports : _context.Reports.AsNoTracking();
        return method != null ? query.Where(method) : query;
    }

    public IQueryable<Report> GetWhere(Expression<Func<Report, bool>>? method = null, string? includeProperties = null, bool isTracking = true)
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

    public async Task<Report?> GetSingleAsync(Expression<Func<Report, bool>> method, bool isTracking = true)
    {
        return await GetWhere(method, isTracking).FirstOrDefaultAsync();
    }

    public async Task<Report?> GetByIdAsync(Guid id, bool throwException = true, bool isTracking = true)
    {
        var entity = await GetWhere(x => x.Id == id, isTracking).FirstOrDefaultAsync();

        if (entity == null && throwException)
        {
            throw new FileNotFoundException($"Report with id {id} not found");
        }

        return entity;
    }

    public bool Exists(Expression<Func<Report, bool>> filter)
    {
        return _context.Reports.Any(filter);
    }

    public async Task<bool> ExistsAsync(Expression<Func<Report, bool>> filter)
    {
        return await _context.Reports.AnyAsync(filter);
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
