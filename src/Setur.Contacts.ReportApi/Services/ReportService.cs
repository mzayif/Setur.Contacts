using Mapster;
using Microsoft.EntityFrameworkCore;
using Setur.Contacts.Base.Results;
using Setur.Contacts.Domain.Entities;
using Setur.Contacts.Domain.Enums;
using Setur.Contacts.ReportApi.DTOs.Requests;
using Setur.Contacts.ReportApi.DTOs.Responses;
using Setur.Contacts.ReportApi.Repositories;

namespace Setur.Contacts.ReportApi.Services;

public class ReportService : IReportService
{
    private readonly ReportRepository _reportRepository;
    private readonly ReportDetailRepository _reportDetailRepository;

    public ReportService(ReportRepository reportRepository, ReportDetailRepository reportDetailRepository)
    {
        _reportRepository = reportRepository;
        _reportDetailRepository = reportDetailRepository;
    }

    public async Task<SuccessDataResult<IEnumerable<ReportListResponse>>> GetAllReportsAsync()
    {
        var reports = await _reportRepository.GetAll(isTracking: false)
            .ProjectToType<ReportListResponse>()
            .ToListAsync();

        return new SuccessDataResult<IEnumerable<ReportListResponse>>(reports, reports.Count);
    }

    public async Task<SuccessDataResult<ReportResponse?>> GetReportByIdAsync(Guid id)
    {
        var report = await _reportRepository.GetWhere(x => x.Id == id, includeProperties: "ReportDetails", isTracking: false)
            .FirstOrDefaultAsync();

        if (report == null)
            throw new Setur.Contacts.Base.Exceptions.NotFoundException("Rapor bulunamadı");

        var reportResponse = report.Adapt<ReportResponse>();

        return new SuccessDataResult<ReportResponse?>(reportResponse);
    }

    public async Task<SuccessResponse> CreateReportAsync(CreateReportRequest request)
    {
        var report = new Report
        {
            Status = ReportStatus.Preparing
        };

        await _reportRepository.AddAsync(report);
        await _reportRepository.SaveAsync();

        // TODO: Burada asenkron rapor oluşturma işlemi yapılacak
        // Kafka message gönderilecek ve background service raporu hazırlayacak

        return new SuccessResponse("Rapor başarıyla oluşturuldu");
    }

    public async Task<SuccessResponse> DeleteReportAsync(Guid id)
    {
        var report = await _reportRepository.GetByIdAsync(id, throwException: false);
        if (report == null)
            throw new Setur.Contacts.Base.Exceptions.NotFoundException("Rapor bulunamadı");

        _reportRepository.Remove(report);
        await _reportRepository.SaveAsync();

        return new SuccessResponse("Rapor başarıyla silindi");
    }
}
