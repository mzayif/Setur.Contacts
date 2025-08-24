using Microsoft.AspNetCore.Mvc;
using Setur.Contacts.Domain.Requests;
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
    /// Belirtilen ID'ye sahip raporun sonuçlarını getirir.
    /// 
    /// Öncelik Sırası:
    /// 1. Cache'den kontrol eder (24 saat içinde oluşturulmuşsa)
    /// 2. Database'den ReportDetail tablosundan kontrol eder (kalıcı kaydedilmişse)
    /// 3. Sadece Report metadata'sını döner (detay yoksa)
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

    /// <summary>
    /// Raporu kalıcı olarak kaydeder (Cache'den ReportDetail tablosuna)
    /// </summary>
    [HttpPost("{reportId}/save-permanently")]
    public async Task<IActionResult> SaveReportPermanently(Guid reportId)
    {
        var result = await _reportService.SaveReportPermanentlyAsync(reportId);
        return Ok(result);
    }

    /// <summary>
    /// Başarısız raporu yeniden işlemeye gönderir
    /// </summary>
    [HttpPost("{reportId}/retry")]
    public async Task<IActionResult> RetryReport(Guid reportId)
    {
        var result = await _reportService.RetryReportAsync(reportId);
        return Ok(result);
    }
}
