using Setur.Contacts.Base.Repositories;
using Setur.Contacts.ContactApi.Data;
using Setur.Contacts.Domain.Entities;

namespace Setur.Contacts.ContactApi.Repositories;

public class CommunicationInfoRepository : Repository<CommunicationInfo, Guid>
{
    public CommunicationInfoRepository(ContactDbContext context) : base(context)
    {
    }
}