using System.ComponentModel;
using Setur.Contacts.Base.Domains.Responses;

namespace Setur.Contacts.Base.Extensions;

public static class EnumExtensions
{
    /// <summary>
    /// Enum tipindeki objelerin <see cref="DescriptionAttribute"/> için tanımlanmış olan açıklama bilgisini döndürür
    /// </summary>
    /// <typeparam name="T"></typeparam>
    /// <param name="enumValue"></param>
    /// <returns></returns>
    public static string GetDescription<T>(this T enumValue) where T : struct, IConvertible
    {
        if (!typeof(T).IsEnum)
            return "";

        var description = enumValue.ToString();
        var fieldInfo = enumValue.GetType().GetField(enumValue.ToString() ?? string.Empty);

        if (fieldInfo != null)
        {
            var attrs = fieldInfo.GetCustomAttributes(typeof(DescriptionAttribute), true);
            if (attrs is {Length: > 0})
            {
                description = ((DescriptionAttribute)attrs[0]).Description;
            }
        }

        return description ?? "";
    }

    /// <summary>
    /// Enum tipindeki bir kaydı <see cref="TypeResponse"/> tipinde döndürür.
    /// </summary>
    /// <typeparam name="T"></typeparam>
    /// <param name="enumValue"></param>
    /// <param name="id"></param>
    /// <returns></returns>
    public static TypeResponse? ConvertTypeResponse<T>(this T enumValue, string id)
        where T : struct, IConvertible
    {
        if (!typeof(T).IsEnum)
            return null;

        int.TryParse(id, out var rowNumber); 

        return new TypeResponse
        {
            Id = id,
            Description = GetDescription(enumValue),
            RowNumber = rowNumber,
            TypeName = GetDescription(enumValue)
        };
    }
}