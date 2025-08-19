using Setur.Contacts.Base.Interfaces;


namespace Setur.Contacts.Base.Results;

/// <summary>
/// Başarılı işlem sonuçlarında sadece işlem sonuç mesajının dönülmesi gereken işlemlerde kullanılacaktır. CQRS Command request işlemleri sonucunda döndürülecek response nesnesi olarak kullanılacak veya bu nesneden türetilecektir.
/// </summary>
public class SuccessResponse : IResult
{
    public SuccessResponse(bool success = true)
    {
        Success = success;
    }

    public SuccessResponse(string message, bool success = true) : this(success)
    {
        Message = message;
    }

    public SuccessResponse(string code, string message, bool success = true) : this(message, success)
    {
        Code = code;
    }


    public SuccessResponse((string code, string message) messageWithCode, bool success = true) : this(messageWithCode.message, success)
    {
        Code = messageWithCode.code;
    }

    public SuccessResponse((string code, string message) messageWithCode, string responseId, bool success = true) : this(messageWithCode, success)
    {
        ResponseId = responseId;
    }

    public bool Success { get; } = true;
    public string Message { get; } = "";
    public string Code { get; } = "0";
    public string? ResponseId { get; set; }

}