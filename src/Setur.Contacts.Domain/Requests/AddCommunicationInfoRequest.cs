using System.ComponentModel.DataAnnotations;
using Setur.Contacts.Domain.Enums;

namespace Setur.Contacts.Domain.Requests;

/// <summary>
/// İletişim bilgisi ekleme isteği
/// </summary>
public class AddCommunicationInfoRequest
{
    /// <summary>
    /// İletişim türü
    /// </summary>
    [Required(ErrorMessage = "İletişim türü zorunludur")]
    public CommunicationType Type { get; set; }

    /// <summary>
    /// İletişim değeri
    /// </summary>
    [Required(ErrorMessage = "İletişim değeri zorunludur")]
    [StringLength(100, ErrorMessage = "İletişim değeri en fazla 100 karakter olabilir")]
    public string Value { get; set; } = string.Empty;
}
