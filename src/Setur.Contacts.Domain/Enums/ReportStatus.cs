using System.ComponentModel;

namespace Setur.Contacts.Domain.Enums;

public enum ReportStatus
{
    [Description("Rapor Hazýrlanýyor")]
    Preparing = 1,
    [Description("Rapor Tamamlandý")]
    Completed = 2
}
