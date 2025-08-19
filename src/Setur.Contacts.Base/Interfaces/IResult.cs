namespace Setur.Contacts.Base.Interfaces;


/// <summary>
/// Data dönülmeyen işlem sonuçlarında dönülecek data örneğidir. Daha çok Kaydet, Güncelle vb. sadece sonuç tamam, güncellendi. veya Hata durumunda hata nedenini dönecektir.
/// </summary>
public interface IResult
{
    bool Success { get; }
    string Message { get; }
    string Code { get; }
}