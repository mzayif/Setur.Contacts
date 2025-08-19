namespace Setur.Contacts.Base.Exceptions;

/// <summary>
/// Yetkisiz erişim hataları için exception sınıfı.
/// </summary>
public class UnauthorizedException : AppBaseException
{
    public UnauthorizedException()
        : base("Bu işlem için yetkiniz bulunmamaktadır", "UNAUTHORIZED")
    {
    }
} 