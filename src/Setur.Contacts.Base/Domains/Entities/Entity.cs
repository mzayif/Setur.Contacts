using Setur.Contacts.Base.Interfaces;

namespace Setur.Contacts.Base.Domains.Entities;

public abstract class Entity<T> : IEntity<T>
{
    public T Id { get; set; } = default!;
}