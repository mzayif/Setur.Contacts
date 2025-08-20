using System.ComponentModel;

namespace Setur.Contacts.Domain.Enums;

public enum ReportStatus
{
    [Description("Rapor Haz�rlan�yor")]
    Preparing = 1,
    [Description("Rapor Tamamland�")]
    Completed = 2
}
