namespace Setur.Contacts.Base.Exceptions;

/// <summary>
/// Uygulama içindeki kontrollü verilmek istenen bütün hatalarda kullanılacak temel Hata sınıfı.<br/>
/// Uygulama içinde başka hata sınıfları oluşturulacak ise bu sıfınfı miras alınmalıdır. <br/>
/// Exception handling yapılırken bu sınıfın kullanılarak merkezi hata yönetimi yapılması önerilir.
/// </summary>
public class AppBaseException : Exception
{
    public string ErrorCode { get; } = "0";

    public AppBaseException() : base()
    {

    }

    public AppBaseException(string errorMessage) : base(errorMessage)
    {

    }

    public AppBaseException(string messageCode, string message) : base(message)
    {
        ErrorCode = messageCode;
    }

    public AppBaseException((string messageCode, string message) errorMessage) : base(errorMessage.message)
    {
        ErrorCode = errorMessage.messageCode;
    }


    public AppBaseException(string errorMessage, Exception innerException) : base(errorMessage, innerException)
    {

    }

    public AppBaseException((string messageCode, string message) errorMessage, Exception innerException) : base(errorMessage.message, innerException)
    {
        ErrorCode = errorMessage.messageCode;
    }
}