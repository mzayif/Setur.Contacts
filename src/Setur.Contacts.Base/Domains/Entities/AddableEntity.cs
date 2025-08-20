using Setur.Contacts.Base.Interfaces;

namespace Setur.Contacts.Base.Domains.Entities;

public abstract class AddableEntity : Entity<Guid>, IAddableEntity
{
    protected AddableEntity()
    {
        Id = Guid.CreateVersion7();
    }

    public DateTime CreateDate { get; set; } = DateTime.UtcNow;
    public string CreateUser { get; set; } = "Test User";
}