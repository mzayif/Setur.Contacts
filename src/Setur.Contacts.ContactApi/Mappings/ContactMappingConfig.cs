using Mapster;
using Setur.Contacts.ContactApi.DTOs.Requests;
using Setur.Contacts.ContactApi.DTOs.Responses;
using Setur.Contacts.Domain.Entities;

namespace Setur.Contacts.ContactApi.Mappings
{
    public class ContactMappingConfig : IRegister
    {
        public void Register(TypeAdapterConfig config)
        {
            // CreateContactRequest -> Contact
            config.NewConfig<CreateContactRequest, Contact>()
                .Map(dest => dest.FirstName, src => src.FirstName)
                .Map(dest => dest.LastName, src => src.LastName)
                .Map(dest => dest.Company, src => src.Company)
                .Map(dest => dest.CommunicationInfos, src => new List<CommunicationInfo>());

            // UpdateContactRequest -> Contact
            config.NewConfig<UpdateContactRequest, Contact>()
                .Map(dest => dest.FirstName, src => src.FirstName)
                .Map(dest => dest.LastName, src => src.LastName)
                .Map(dest => dest.Company, src => src.Company);

            // Contact -> ContactResponse
            config.NewConfig<Contact, ContactResponse>()
                .Map(dest => dest.Id, src => src.Id)
                .Map(dest => dest.FirstName, src => src.FirstName)
                .Map(dest => dest.LastName, src => src.LastName)
                .Map(dest => dest.Company, src => src.Company)
                .Map(dest => dest.CommunicationInfos, src => src.CommunicationInfos);

            // Contact -> ContactDetailResponse
            config.NewConfig<Contact, ContactDetailResponse>()
                .Map(dest => dest.Id, src => src.Id)
                .Map(dest => dest.FirstName, src => src.FirstName)
                .Map(dest => dest.LastName, src => src.LastName)
                .Map(dest => dest.Company, src => src.Company)
                .Map(dest => dest.CommunicationInfos, src => src.CommunicationInfos);
        }
    }
}
