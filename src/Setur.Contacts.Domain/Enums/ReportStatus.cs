using System.ComponentModel;

namespace Setur.Contacts.Domain.Enums;

public enum ReportStatus
{
    [Description("Rapor Hazırlanıyor")]
    Preparing = 1,
    [Description("Rapor Tamamlandı")]
    Completed = 2,
    [Description("Rapor Başarısız")]
    Failed = 3
}
