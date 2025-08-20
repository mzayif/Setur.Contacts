using System.ComponentModel.DataAnnotations;

namespace Setur.Contacts.ReportApi.DTOs.Requests;

public class CreateReportRequest
{
    [Required(ErrorMessage = "Lokasyon alanı boş olamaz")]
    [StringLength(100, ErrorMessage = "Lokasyon en fazla 100 karakter olabilir")]
    public string Location { get; set; } = string.Empty;
}
