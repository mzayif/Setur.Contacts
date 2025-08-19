namespace Setur.Contacts.Base.Domains.Responses;

/// <summary>
/// ComboBox gibi nesnelerde seçim için kullanılacak tiplerin standart bir yapıda kullanılması için bu class oluşturulmuştur.
/// </summary>
public class TypeResponse
{
    /// <summary>
    /// Tipe ait DB deki ID
    /// </summary>
    public string Id { get; set; } = default!;
    /// <summary>
    /// Bir tipe bağlı olarak alt tip ise bağlı olduğu üst tipin ID'si<br/>
    /// Örneğin: İle bağlı ilçeler de bu alana İl ID verilmelidir.<br/>
    /// </summary>
    public string SubTypeId { get; set; } = default!;
    /// <summary>
    /// Tipe ait DB deki Tanım adı
    /// </summary>
    public string TypeName { get; set; } = default!;
    /// <summary>
    /// Listede görünecek sıra numarası<br/>
    /// </summary>
    public int RowNumber { get; set; }
    /// <summary>
    /// Listede varsayılan olarak seçilecek mi?<br/>
    /// Her listede en fazla bir tane kayıt varsayılan olarak seçilmelidir.<br/>
    /// </summary>
    public bool DefaultType { get; set; }
    /// <summary>
    /// Kayıt ile ilgili açıklama bilgisi<br/>
    /// Title gibi alanlarda kullanılmak için kullanılabilir.<br/>
    /// </summary>
    public string Description { get; set; } = default!;
}