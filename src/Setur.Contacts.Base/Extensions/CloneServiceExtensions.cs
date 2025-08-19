using System.Text.Json;

namespace Setur.Contacts.Base.Extensions;

public static class CloneServiceExtensions
{
    /// <summary>
    /// Bu metod ile bir nesneyi derin kopyalayabilirsiniz. Bu metod, JsonSerializer kullanarak nesneyi JSON formatına dönüştürür ve ardından tekrar nesneye dönüştürür.<br/>
    /// Önemli Not: Bu metod, <br/>
    ///  - Yalnızca JSON serileştirmesi desteklenen nesneler için çalışır. <br/>
    ///  - Döngüsel referanslar veya özel serileştirme gerektiren nesneler için çalışmayabilir. <br/>
    ///  - Performans açısından büyük nesneler için önerilmez. <br/>
    /// </summary>
    /// <typeparam name="T"></typeparam>
    /// <param name="obj"></param>
    /// <returns></returns>
    public static T DeepClone<T>(T obj)
    {
        var json = JsonSerializer.Serialize(obj);
        return JsonSerializer.Deserialize<T>(json) ?? throw new InvalidOperationException();
    }
}

