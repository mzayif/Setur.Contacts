
namespace Setur.Contacts.Base.Interfaces;

public interface IAddableEntity: IEntity<Guid>
{
    DateTime CreateDate { get; set; }
    string CreateUser { get; set; }
}