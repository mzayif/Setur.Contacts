using Microsoft.AspNetCore.Mvc;
using Setur.Contacts.Base.Results;
using Setur.Contacts.Domain.Models;
using Setur.Contacts.ContactApi.Services;
using Setur.Contacts.Domain.Enums;

namespace Setur.Contacts.ContactApi.Controllers;

[ApiController]
[Route("api/[controller]")]
public class ReportDataController : ControllerBase
{
    private readonly IContactService _contactService;

    public ReportDataController(IContactService contactService)
    {
        _contactService = contactService;
    }

    /// <summary>
    /// Lokasyon bazlı rapor verilerini getirir
    /// </summary>
    /// <param name="locations">Lokasyon listesi (virgülle ayrılmış)</param>
    [HttpGet("location")]
    public async Task<SuccessDataResult<ReportDataResponse>> GetLocationReportDataAsync([FromQuery] string locations)
    {
        var locationList = locations?.Split(',', StringSplitOptions.RemoveEmptyEntries)
            .Select(l => l.Trim())
            .Where(l => !string.IsNullOrEmpty(l))
            .ToList() ?? new List<string>();

        var result = await _contactService.GetReportDataAsync(ReportType.LocationBased, locationList);
        return result;
    }

    /// <summary>
    /// Şirket bazlı rapor verilerini getirir
    /// </summary>
    /// <param name="companies">Şirket listesi (virgülle ayrılmış)</param>
    [HttpGet("company")]
    public async Task<SuccessDataResult<ReportDataResponse>> GetCompanyReportDataAsync([FromQuery] string companies)
    {
        var companyList = companies?.Split(',', StringSplitOptions.RemoveEmptyEntries)
            .Select(c => c.Trim())
            .Where(c => !string.IsNullOrEmpty(c))
            .ToList() ?? new List<string>();

        var result = await _contactService.GetReportDataAsync(ReportType.CompanyBased, companyList);
        return result;
    }

    /// <summary>
    /// Genel rapor verilerini getirir
    /// </summary>
    [HttpGet("general")]
    public async Task<SuccessDataResult<ReportDataResponse>> GetGeneralReportDataAsync()
    {
        var result = await _contactService.GetReportDataAsync(ReportType.General, new List<string>());
        return result;
    }
}
