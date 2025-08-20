using Mapster;
using Setur.Contacts.Domain.Entities;
using Setur.Contacts.ReportApi.DTOs.Responses;

namespace Setur.Contacts.ReportApi.Mappings;

public class ReportMappingConfig : IRegister
{
    public void Register(TypeAdapterConfig config)
    {
        // Report -> ReportListResponse
        config.NewConfig<Report, ReportListResponse>()
            .Map(dest => dest.Id, src => src.Id)
            .Map(dest => dest.RequestedAt, src => src.RequestedAt)
            .Map(dest => dest.Status, src => src.Status);

        // Report -> ReportResponse
        config.NewConfig<Report, ReportResponse>()
            .Map(dest => dest.Id, src => src.Id)
            .Map(dest => dest.RequestedAt, src => src.RequestedAt)
            .Map(dest => dest.Status, src => src.Status)
            .Map(dest => dest.ReportDetails, src => src.ReportDetails);

        // ReportDetail -> ReportDetailResponse
        config.NewConfig<ReportDetail, ReportDetailResponse>()
            .Map(dest => dest.Id, src => src.Id)
            .Map(dest => dest.Location, src => src.Location)
            .Map(dest => dest.PersonCount, src => src.PersonCount)
            .Map(dest => dest.PhoneCount, src => src.PhoneCount);
    }
}
