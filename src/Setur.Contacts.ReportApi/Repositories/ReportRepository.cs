using Setur.Contacts.Base.Repositories;
using Setur.Contacts.Domain.Entities;
using Setur.Contacts.ReportApi.Data;

namespace Setur.Contacts.ReportApi.Repositories;

public class ReportRepository : Repository<Report, Guid>
{
    public ReportRepository(ReportDbContext context) : base(context)
    {
    }
}
