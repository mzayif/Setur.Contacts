using System.ComponentModel.DataAnnotations;
using Setur.Contacts.Domain.Enums;

namespace Setur.Contacts.Domain.Requests;

public class CreateReportRequest
{
    [Required(ErrorMessage = "Rapor türü seçilmelidir")]
    public ReportType ReportType { get; set; }

    public string Parameters { get; set; } = string.Empty;
}
