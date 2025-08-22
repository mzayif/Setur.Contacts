using System.ComponentModel.DataAnnotations;

namespace Setur.Contacts.Domain.Requests;

public class CreateContactRequest
{
    [Required(ErrorMessage = "Ad alanı zorunludur")]
    [StringLength(50, ErrorMessage = "Ad en fazla 50 karakter olabilir")]
    public string FirstName { get; set; } = string.Empty;

    [Required(ErrorMessage = "Soyad alanı zorunludur")]
    [StringLength(50, ErrorMessage = "Soyad en fazla 50 karakter olabilir")]
    public string LastName { get; set; } = string.Empty;

    [StringLength(100, ErrorMessage = "Firma adı en fazla 100 karakter olabilir")]
    public string Company { get; set; } = string.Empty;
}