namespace Setur.Contacts.Base.Interfaces;

public interface IDeletableEntity 
{
    DateTime? DeleteDate { get; set; }
    string? DeleteUser { get; set; }
    bool IsDeleted { get; set; }
}