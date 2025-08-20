using System.ComponentModel.DataAnnotations;
using Setur.Contacts.Domain.Enums;

namespace Setur.Contacts.ContactApi.DTOs.Requests;

public class CreateCommunicationInfoRequest
{
    [Required(ErrorMessage = "Kişi ID'si zorunludur")]
    public Guid ContactId { get; set; }

    [Required(ErrorMessage = "İletişim tipi zorunludur")]
    public CommunicationType Type { get; set; }

    [Required(ErrorMessage = "İletişim bilgisi değeri zorunludur")]
    [StringLength(100, ErrorMessage = "İletişim bilgisi değeri en fazla 100 karakter olabilir")]
    public string Value { get; set; } = string.Empty;
}