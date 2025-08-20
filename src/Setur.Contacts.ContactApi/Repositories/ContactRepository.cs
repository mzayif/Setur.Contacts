using System.Linq.Expressions;
using Microsoft.EntityFrameworkCore;
using Setur.Contacts.Base.Repositories;
using Setur.Contacts.ContactApi.Data;
using Setur.Contacts.Domain.Entities;

namespace Setur.Contacts.ContactApi.Repositories;

public class ContactRepository : IRepository<Contact, Guid>
{
    private readonly ContactDbContext _context;

    public ContactRepository(ContactDbContext context)
    {
        _context = context;
    }

    public DbSet<Contact> Table => _context.Contacts;

    public async Task<bool> AddAsync(Contact entity)
    {
        await _context.Contacts.AddAsync(entity);
        return true;
    }

    public async Task<bool> AddAsync(List<Contact> entities)
    {
        await _context.Contacts.AddRangeAsync(entities);
        return true;
    }

    public bool Update(Contact entity)
    {
        _context.Contacts.Update(entity);
        return true;
    }

    public bool Cancel(Contact entity)
    {
        // AddableEntity'den inherit ettiği için Cancel işlemi mevcut
        _context.Contacts.Update(entity);
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

    public bool Activate(Contact entity)
    {
        _context.Contacts.Update(entity);
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

    public bool Remove(Contact entity)
    {
        _context.Contacts.Remove(entity);
        return true;
    }

    public bool Remove(List<Contact> entities)
    {
        _context.Contacts.RemoveRange(entities);
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

    public IQueryable<Contact> GetAll(bool isTracking = true)
    {
        return isTracking ? _context.Contacts : _context.Contacts.AsNoTracking();
    }

    public IQueryable<Contact> GetWhere(Expression<Func<Contact, bool>>? method = null, bool isTracking = true)
    {
        var query = isTracking ? _context.Contacts : _context.Contacts.AsNoTracking();
        return method != null ? query.Where(method) : query;
    }

    public IQueryable<Contact> GetWhere(Expression<Func<Contact, bool>>? method = null, string? includeProperties = null, bool isTracking = true)
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

    public async Task<Contact?> GetSingleAsync(Expression<Func<Contact, bool>> method, bool isTracking = true)
    {
        return await GetWhere(method, isTracking).FirstOrDefaultAsync();
    }

    public async Task<Contact?> GetByIdAsync(Guid id, bool throwException = true, bool isTracking = true)
    {
        var entity = await GetWhere(x => x.Id == id, isTracking).FirstOrDefaultAsync();
            
        if (entity == null && throwException)
        {
            throw new FileNotFoundException($"Contact with id {id} not found");
        }
            
        return entity;
    }

    public bool Exists(Expression<Func<Contact, bool>> filter)
    {
        return _context.Contacts.Any(filter);
    }

    public async Task<bool> ExistsAsync(Expression<Func<Contact, bool>> filter)
    {
        return await _context.Contacts.AnyAsync(filter);
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