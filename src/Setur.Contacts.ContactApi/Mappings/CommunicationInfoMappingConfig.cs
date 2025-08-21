using Mapster;
using Setur.Contacts.ContactApi.DTOs.Requests;
using Setur.Contacts.ContactApi.DTOs.Responses;
using Setur.Contacts.Domain.Entities;

namespace Setur.Contacts.ContactApi.Mappings;

public class CommunicationInfoMappingConfig : IRegister
{
    public void Register(TypeAdapterConfig config)
    {
        // CreateCommunicationInfoRequest -> CommunicationInfo
        config.NewConfig<CreateCommunicationInfoRequest, CommunicationInfo>()
            .Map(dest => dest.ContactId, src => src.ContactId)
            .Map(dest => dest.Type, src => src.Type)
            .Map(dest => dest.Value, src => src.Value);

        // UpdateCommunicationInfoRequest -> CommunicationInfo
        config.NewConfig<UpdateCommunicationInfoRequest, CommunicationInfo>()
            .Map(dest => dest.Type, src => src.Type)
            .Map(dest => dest.Value, src => src.Value);

        // CommunicationInfo -> CommunicationInfoResponse
        config.NewConfig<CommunicationInfo, CommunicationInfoResponse>()
            .Map(dest => dest.Id, src => src.Id)
            .Map(dest => dest.Type, src => src.Type)
            .Map(dest => dest.Value, src => src.Value);
    }
}