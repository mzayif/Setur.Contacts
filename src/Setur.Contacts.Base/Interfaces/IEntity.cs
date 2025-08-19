namespace Setur.Contacts.Base.Interfaces;

/// <summary>
/// Bu İnterface, Entity sınıflarının Id property'lerini tanımlamak için kullanılır.
/// Bütün Entity sınıfları bu interface'i implement etmelidir. <br/>
///  <br/>
/// * Önemli: Id'nin Type ını belirlerken<br/>
///     - Tanım ve tip tablolarında int gibi ardışık bir tip kullanılması önerilir. <br/>
///     - Çok fazla veri içeren tablolarda Id tipi için Guid kullanılabilir. <br/>
///     - Guid kullanımlarında sıralı guid üreten <see cref="Guid.CreateVersion7"/> kullanılması önerilir. <br/>
/// </summary>
/// <typeparam name="T">Id alanının tipi</typeparam>
public interface IEntity<T>
{
    T Id { get; set; }
}
