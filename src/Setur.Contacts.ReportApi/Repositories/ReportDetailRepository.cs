using Setur.Contacts.Base.Repositories;
using Setur.Contacts.Domain.Entities;
using Setur.Contacts.ReportApi.Data;

namespace Setur.Contacts.ReportApi.Repositories;

public class ReportDetailRepository : Repository<ReportDetail, Guid>
{
    public ReportDetailRepository(ReportDbContext context) : base(context)
    {
    }
}
