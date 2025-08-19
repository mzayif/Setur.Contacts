namespace Setur.Contacts.Base.Results;

/// <summary>
/// Hatalı işlem sonucunda döndürülecek veri modelidir. Bütün hatalarda bu model kullanılacaktır.
/// </summary>
public class ErrorResponse : SuccessResponse
{
    public List<string>? ErrorMessages;

    public ErrorResponse() : base(false)
    {
    }

    public ErrorResponse(string message) : base(message, false)
    {
        AddErrorMessage(message);
    }

    public ErrorResponse((string code, string message) messageWithCode) : base(messageWithCode, false)
    {
        AddErrorMessage(messageWithCode.message);
    }

    public ErrorResponse(string code, string message) : base(code, message, false)
    {
        AddErrorMessage(message);
    }

    public void AddErrorMessage(string message)
    {
        ErrorMessages ??= new List<string>();

        ErrorMessages.Add(message);
    }
}