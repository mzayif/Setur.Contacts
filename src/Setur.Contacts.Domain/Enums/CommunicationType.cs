using System.ComponentModel;

namespace Setur.Contacts.Domain.Enums;

public enum CommunicationType
{
    [Description("Telefon")]
    Phone = 1,
    [Description("Email")]
    Email = 2,
    [Description("Lokasyon")]
    Location = 3
}
