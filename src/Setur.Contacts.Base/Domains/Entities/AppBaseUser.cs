using Setur.Contacts.Base.Interfaces;

namespace Setur.Contacts.Base.Domains.Entities;

public class AppBaseUser : IEntity<int>
{
    public AppBaseUser()
    {
        Id = Guid.CreateVersion7().GetHashCode();
    }

    public int Id { get; set; }
    public string UserName { get; set; } = default!;
    public string Email { get; set; } = default!;
    public string FirstName { get; set; } = default!;
    public string LastName { get; set; } = default!;
    public string FullName => $"{FirstName} {LastName}";
}