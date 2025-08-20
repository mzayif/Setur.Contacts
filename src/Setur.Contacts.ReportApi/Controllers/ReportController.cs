using Microsoft.AspNetCore.Mvc;
using Setur.Contacts.ReportApi.DTOs.Requests;
using Setur.Contacts.ReportApi.Services;

namespace Setur.Contacts.ReportApi.Controllers;

[ApiController]
[Route("api/[controller]")]
public class ReportController : ControllerBase
{
    private readonly IReportService _reportService;

    public ReportController(IReportService reportService)
    {
        _reportService = reportService;
    }

    /// <summary>
    /// Tüm raporları getirir
    /// </summary>
    [HttpGet]
    public async Task<IActionResult> GetReports()
    {
        var result = await _reportService.GetAllReportsAsync();
        return Ok(result);
    }

    /// <summary>
    /// Belirtilen ID'ye sahip raporu detaylarıyla birlikte getirir
    /// </summary>
    [HttpGet("{id}")]
    public async Task<IActionResult> GetReport(Guid id)
    {
        var result = await _reportService.GetReportByIdAsync(id);
        return Ok(result);
    }

    /// <summary>
    /// Yeni bir rapor oluşturur
    /// </summary>
    [HttpPost]
    public async Task<IActionResult> CreateReport(CreateReportRequest request)
    {
        var result = await _reportService.CreateReportAsync(request);
        return Ok(result);
    }

    /// <summary>
    /// Belirtilen ID'ye sahip raporu siler
    /// </summary>
    [HttpDelete("{id}")]
    public async Task<IActionResult> DeleteReport(Guid id)
    {
        var result = await _reportService.DeleteReportAsync(id);
        return Ok(result);
    }
}
