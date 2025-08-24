namespace Setur.Contacts.Base.Exceptions;

/// <summary>
/// İş kuralı hataları için exception sınıfı.
/// </summary>
public class BusinessException : AppBaseException
{
    public BusinessException(string message, string errorCode = "BUSINESS_ERROR")
        : base(errorCode, message)
    {
    }
} 