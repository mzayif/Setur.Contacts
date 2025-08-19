using Setur.Contacts.Base.Interfaces;

namespace Setur.Contacts.Base.Results;

/// <summary>
/// İşlem sonucunda bir DB verisi döndürülecek ise bu model kullanılacaktır. Bütün CQRS response nesneleri bu modeli kullanacaktır.
/// </summary>
/// <typeparam name="T">Geri döndürülecek response nesnesi</typeparam>
public class SuccessDataResult<T> : SuccessResponse, IDataResult<T>
{
    public T Data { get; }
    public int DataCount { get; }

    public SuccessDataResult(T data) : base(true)
    {
        Data = data;
    }

    public SuccessDataResult(T data, int dataCount) : base(true)
    {
        Data = data;
        DataCount = dataCount;
    }

    public SuccessDataResult(T data, string message) : base(message, true)
    {
        Data = data;
    }

    public SuccessDataResult(T data, string message, int dataCount) : base(message, true)
    {
        Data = data;
        DataCount = dataCount;
    }

    public SuccessDataResult(T data, (string code, string message) messageWithCode) : base(messageWithCode, true)
    {
        Data = data;
    }

    public SuccessDataResult(T data, (string code, string message) messageWithCode, string responseId ) : base(messageWithCode, responseId, true)
    {
        Data = data;
    }

    public SuccessDataResult(T data, (string code, string message) messageWithCode, int dataCount) : base(messageWithCode, true)
    {
        Data = data;
        DataCount = dataCount;
    }
}