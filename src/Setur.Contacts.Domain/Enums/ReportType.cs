using System.ComponentModel;

namespace Setur.Contacts.Domain.Enums;

public enum ReportType
{
    [Description("Konum Bazlı")]
    LocationBased = 1,
    
    [Description("Şirket Bazlı")]
    CompanyBased = 2,
    
    [Description("Genel İstatistik")]
    General = 3
}
