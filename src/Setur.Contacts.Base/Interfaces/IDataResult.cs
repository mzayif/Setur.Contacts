namespace Setur.Contacts.Base.Interfaces;

/// <summary>
/// İşlem sonucunda veri/DB Kaydı dönülmesi gereken işlem sonuçlarında kullanılacak veri modelini tarif edecektir.
/// </summary>
/// <typeparam name="T"></typeparam>
public interface IDataResult<out T> : IResult
{
    T? Data { get; }
}