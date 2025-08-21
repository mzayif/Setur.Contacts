using System.ComponentModel.DataAnnotations;
using Setur.Contacts.Domain.Enums;

namespace Setur.Contacts.ReportApi.DTOs.Requests;

public class CreateReportRequest
{
    [Required(ErrorMessage = "Rapor türü seçilmelidir")]
    public ReportType ReportType { get; set; }

    public object Parameters { get; set; } = new();
}
