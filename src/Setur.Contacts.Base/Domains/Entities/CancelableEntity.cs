using Setur.Contacts.Base.Interfaces;

namespace Setur.Contacts.Base.Domains.Entities;

public abstract class CancelableEntity : AddableEntity, IDeletableEntity
{
    public DateTime? DeleteDate { get; set; }
    public string? DeleteUser { get; set; }
    public bool IsDeleted { get; set; } = false;
}