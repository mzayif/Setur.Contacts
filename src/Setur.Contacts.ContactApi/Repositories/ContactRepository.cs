using Setur.Contacts.Base.Repositories;
using Setur.Contacts.ContactApi.Data;
using Setur.Contacts.Domain.Entities;

namespace Setur.Contacts.ContactApi.Repositories;

public class ContactRepository : Repository<Contact, Guid>
{
    public ContactRepository(ContactDbContext context) : base(context)
    {
    }
}